using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class ClothSimulation : MonoBehaviour
{
    [Header("Simulation")]
    public Vector3 gravity = new Vector3(0.0f, -10.0f, 0.0f);
    public float simulationDt = 1.0f / 60.0f;
    [Range(1, 50)] public int numSubsteps = 15;
    [Range(0.0f, 10.0f)] public float bendingCompliance = 1.0f;
    public float stretchingCompliance = 0.0f;
    [Header("Damping")]
    [Tooltip("Damps only relative velocity along cloth edges. 0 disables damping. This preserves rigid translation much better than global velocity damping.")]
    [Range(0.0f, 20.0f)] public float velocityDamping = 2.0f;
    public bool paused = true;
    public bool showEdges = false;
    [Range(1, 8)] public int normalUpdateInterval = 2;

    [Header("Cloth geometry")]
    public int columns = 41;
    public int rows = 81;
    public float spacing = 0.01f;
    public float topY = 1.145859f;

    [Header("Interaction")]
    public float orbitSpeed = 0.25f;
    public float panSpeed = 0.0025f;
    public float zoomSpeed = 0.15f;

    private Vector3[] pos;
    private Vector3[] prevPos;
    private Vector3[] restPos;
    private Vector3[] vel;
    private Vector3[] meshNormals;
    private float[] invMass;
    private int[] triangles;
    private int[] stretchingIds;
    private int[] bendingIds;
    private float[] stretchingLengths;
    private float[] bendingLengths;
    private Mesh clothMesh;
    private Mesh edgeMesh;
    private MeshRenderer clothRenderer;
    private MeshRenderer edgeRenderer;
    private GameObject clothObject;
    private GameObject edgeObject;
    private Camera sceneCamera;
    private Vector3 cameraTarget = new Vector3(0.0f, 0.6f, 0.0f);
    private Vector3 initialCameraPosition = new Vector3(0.0f, 1.0f, 1.0f);
    private Quaternion initialCameraRotation;
    private int grabId = -1;
    private float grabInvMass;
    private float grabDistance;
    private Vector3 grabPrevPos;
    private Vector3 grabVelocity;
    private float grabTime;
    private float accumulator;
    private readonly Stopwatch stopwatch = new Stopwatch();
    private int timeFrames;
    private double timeSum;
    private double averageMs;
    private bool initialized;
    private int physicsStepCounter;
    private static readonly MeshUpdateFlags vertexUpdateFlags = MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices;
    private const string topologyHex = "04000800000C00010000090007000007000E00000E000800C00C0000000008000000000001000000000FF00300003EC00F000018E0070000E0E0010000003C18F001001FFCFF03003FFCFF07003E18E00E008800000C00C000000800C00300008000010000F00F000000C007000000C003000000F80F0000838F780000600E7E0000E800F400000C000000F00C0000F03F0C008001000C000003000E80033E000E181E1E000C2078000008001800000000A0010008C0C1000008000C00100E000C00E00D0010008203004000E00F000000E00C000000D001C0C70100007007000000002F000000000E000000386C1E00001CC00900000000080000000008001C0000010060033801000000FF01000000FC77E0400F38F1D90000001D000000000E000000000F000000000F003800010600008003000000C001000000800100100100C001300700C003200E00E4010003008C010000007C00000000FF000000007F000000007E001000000818100000087E10000008FF100000083C1000C00F003000700F0020000007000001801F0000";

    private struct EdgeKey : IEquatable<EdgeKey>
    {
        public int a;
        public int b;

        public EdgeKey(int id0, int id1)
        {
            a = Mathf.Min(id0, id1);
            b = Mathf.Max(id0, id1);
        }

        public bool Equals(EdgeKey other)
        {
            return a == other.a && b == other.b;
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return unchecked((a * 397) ^ b);
        }
    }

    private void Start()
    {
        CreateDemoScene();
        CreateCloth();
        initialized = true;
        ApplyVisibility();
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }
        HandleKeyboard();
        HandleCamera();
        HandleGrabber();
        if (paused)
        {
            accumulator = 0.0f;
            return;
        }
        accumulator += Mathf.Min(Time.deltaTime, 0.05f);
        while (accumulator >= simulationDt)
        {
            SimulateStep();
            accumulator -= simulationDt;
        }
    }

    private void OnDestroy()
    {
        if (clothMesh != null)
        {
            Destroy(clothMesh);
        }
        if (edgeMesh != null)
        {
            Destroy(edgeMesh);
        }
    }

    private void OnGUI()
    {
        if (!initialized)
        {
            return;
        }
        GUI.Box(new Rect(10.0f, 10.0f, 420.0f, 165.0f), "Cloth Simulation V3");
        if (GUI.Button(new Rect(20.0f, 40.0f, 80.0f, 30.0f), paused ? "Run" : "Stop"))
        {
            paused = !paused;
        }
        if (GUI.Button(new Rect(110.0f, 40.0f, 80.0f, 30.0f), "Restart"))
        {
            RestartSimulation();
        }
        bool nextShowEdges = GUI.Toggle(new Rect(205.0f, 45.0f, 120.0f, 25.0f), showEdges, "Show edges");
        if (nextShowEdges != showEdges)
        {
            showEdges = nextShowEdges;
            ApplyVisibility();
        }
        GUI.Label(new Rect(20.0f, 76.0f, 400.0f, 20.0f), $"{triangles.Length / 3} tris    {pos.Length} verts    {averageMs:F3} ms / physics step (incl. mesh)");
        GUI.Label(new Rect(20.0f, 101.0f, 150.0f, 20.0f), "Bending compliance:");
        bendingCompliance = GUI.HorizontalSlider(new Rect(160.0f, 106.0f, 150.0f, 20.0f), bendingCompliance, 0.0f, 10.0f);
        GUI.Label(new Rect(20.0f, 125.0f, 400.0f, 20.0f), $"{bendingCompliance:F3}   velocity damping: {velocityDamping:F2}");
        GUI.Label(new Rect(20.0f, 145.0f, 400.0f, 20.0f), "LMB: grab   RMB: orbit   MMB: pan   wheel: zoom");
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartSimulation();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            showEdges = !showEdges;
            ApplyVisibility();
        }
    }

    private void CreateDemoScene()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.31f, 0.31f, 0.31f, 1.0f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 0.0f;
        RenderSettings.fogEndDistance = 15.0f;
        SetupCamera();
        CreateLights();
        CreateGround();
        CreateGrid();
    }

    private void SetupCamera()
    {
        sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            sceneCamera = cameraObject.AddComponent<Camera>();
        }
        sceneCamera.fieldOfView = 70.0f;
        sceneCamera.nearClipPlane = 0.01f;
        sceneCamera.farClipPlane = 100.0f;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = Color.black;
        sceneCamera.transform.position = initialCameraPosition;
        sceneCamera.transform.LookAt(cameraTarget);
        initialCameraRotation = sceneCamera.transform.rotation;
    }

    private void CreateLights()
    {
        GameObject spotObject = new GameObject("Cloth Spot Light");
        Light spot = spotObject.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.color = Color.white;
        spot.intensity = 1.0f;
        spot.spotAngle = 36.0f;
        spot.transform.position = new Vector3(2.0f, 3.0f, 3.0f);
        spot.transform.LookAt(new Vector3(0.0f, 0.6f, 0.0f));
        spot.shadows = LightShadows.Soft;
        GameObject dirObject = new GameObject("Cloth Directional Light");
        Light dir = dirObject.AddComponent<Light>();
        dir.type = LightType.Directional;
        dir.color = new Color(0.33f, 0.31f, 0.35f, 1.0f);
        dir.intensity = 1.0f;
        dir.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        dir.shadows = LightShadows.Soft;
    }

    private void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Cloth Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2.0f, 1.0f, 2.0f);
        Renderer renderer = ground.GetComponent<Renderer>();
        renderer.sharedMaterial = CreateLitMaterial(new Color(0.63f, 0.68f, 0.69f, 1.0f));
        renderer.receiveShadows = true;
    }

    private void CreateGrid()
    {
        GameObject gridObject = new GameObject("Cloth Grid");
        MeshFilter filter = gridObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gridObject.AddComponent<MeshRenderer>();
        Mesh gridMesh = new Mesh();
        List<Vector3> gridVertices = new List<Vector3>();
        List<int> gridIndices = new List<int>();
        int divisions = 20;
        float half = 10.0f;
        for (int i = 0; i <= divisions; i++)
        {
            float t = -half + 2.0f * half * i / divisions;
            gridVertices.Add(new Vector3(-half, 0.002f, t));
            gridVertices.Add(new Vector3(half, 0.002f, t));
            gridVertices.Add(new Vector3(t, 0.002f, -half));
            gridVertices.Add(new Vector3(t, 0.002f, half));
            int baseIndex = i * 4;
            gridIndices.Add(baseIndex);
            gridIndices.Add(baseIndex + 1);
            gridIndices.Add(baseIndex + 2);
            gridIndices.Add(baseIndex + 3);
        }
        gridMesh.SetVertices(gridVertices);
        gridMesh.SetIndices(gridIndices, MeshTopology.Lines, 0);
        filter.sharedMesh = gridMesh;
        renderer.sharedMaterial = CreateUnlitMaterial(new Color(0.18f, 0.18f, 0.18f, 1.0f));
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private void CreateCloth()
    {
        BuildGridGeometry();
        BuildConstraints();
        InitializePhysics();
        clothObject = new GameObject("XPBD Cloth");
        MeshFilter clothFilter = clothObject.AddComponent<MeshFilter>();
        clothRenderer = clothObject.AddComponent<MeshRenderer>();
        clothMesh = new Mesh();
        clothMesh.name = "XPBD Cloth Mesh";
        clothMesh.indexFormat = IndexFormat.UInt16;
        clothMesh.MarkDynamic();
        clothMesh.vertices = pos;
        clothMesh.triangles = triangles;
        meshNormals = new Vector3[pos.Length];
        RecalculateNormalsFast();
        clothMesh.normals = meshNormals;
        clothMesh.bounds = new Bounds(new Vector3(0.0f, 0.6f, 0.0f), new Vector3(4.0f, 4.0f, 4.0f));
        clothFilter.sharedMesh = clothMesh;
        clothRenderer.sharedMaterial = CreateLitMaterial(Color.red);
        clothRenderer.shadowCastingMode = ShadowCastingMode.On;
        clothRenderer.receiveShadows = true;
        edgeObject = new GameObject("XPBD Cloth Edges");
        MeshFilter edgeFilter = edgeObject.AddComponent<MeshFilter>();
        edgeRenderer = edgeObject.AddComponent<MeshRenderer>();
        edgeMesh = new Mesh();
        edgeMesh.name = "XPBD Cloth Edge Mesh";
        edgeMesh.indexFormat = IndexFormat.UInt16;
        edgeMesh.MarkDynamic();
        edgeMesh.vertices = pos;
        edgeMesh.SetIndices(stretchingIds, MeshTopology.Lines, 0);
        edgeMesh.bounds = clothMesh.bounds;
        edgeFilter.sharedMesh = edgeMesh;
        edgeRenderer.sharedMaterial = CreateUnlitMaterial(Color.red);
        edgeRenderer.shadowCastingMode = ShadowCastingMode.Off;
        edgeRenderer.receiveShadows = false;
    }

    private void BuildGridGeometry()
    {
        columns = Mathf.Max(2, columns);
        rows = Mathf.Max(2, rows);
        int vertexCount = columns * rows;
        pos = new Vector3[vertexCount];
        prevPos = new Vector3[vertexCount];
        restPos = new Vector3[vertexCount];
        vel = new Vector3[vertexCount];
        invMass = new float[vertexCount];
        float width = (columns - 1) * spacing;
        float minX = -0.5f * width;
        for (int row = 0; row < rows; row++)
        {
            float y = topY - row * spacing;
            for (int col = 0; col < columns; col++)
            {
                int id = row * columns + col;
                float x = minX + col * spacing;
                pos[id] = new Vector3(x, y, 0.0f);
                prevPos[id] = pos[id];
                restPos[id] = pos[id];
                vel[id] = Vector3.zero;
            }
        }
        int quadCount = (columns - 1) * (rows - 1);
        triangles = new int[quadCount * 6];
        int ti = 0;
        for (int row = 0; row < rows - 1; row++)
        {
            for (int col = 0; col < columns - 1; col++)
            {
                int a = row * columns + col;
                int b = a + 1;
                int c = a + columns;
                int d = c + 1;
                int cellIndex = row * (columns - 1) + col;
                bool backslashDiagonal = UsesBackslashDiagonal(cellIndex);
                if (backslashDiagonal)
                {
                    triangles[ti++] = a;
                    triangles[ti++] = c;
                    triangles[ti++] = d;
                    triangles[ti++] = a;
                    triangles[ti++] = d;
                    triangles[ti++] = b;
                }
                else
                {
                    triangles[ti++] = a;
                    triangles[ti++] = c;
                    triangles[ti++] = b;
                    triangles[ti++] = b;
                    triangles[ti++] = c;
                    triangles[ti++] = d;
                }
            }
        }
    }

    private bool UsesBackslashDiagonal(int cellIndex)
    {
        if (columns != 41 || rows != 81)
        {
            return false;
        }
        int byteIndex = cellIndex >> 3;
        int bitIndex = cellIndex & 7;
        int charIndex = byteIndex * 2;
        int high = HexNibble(topologyHex[charIndex]);
        int low = HexNibble(topologyHex[charIndex + 1]);
        int value = high * 16 + low;
        return (value & (1 << bitIndex)) != 0;
    }

    private static int HexNibble(char c)
    {
        if (c >= '0' && c <= '9')
        {
            return c - '0';
        }
        return 10 + c - 'A';
    }

    private void BuildConstraints()
    {
        int edgeCount = triangles.Length;
        int[] neighbors = new int[edgeCount];
        for (int i = 0; i < neighbors.Length; i++)
        {
            neighbors[i] = -1;
        }
        Dictionary<EdgeKey, int> firstEdges = new Dictionary<EdgeKey, int>(edgeCount);
        for (int triangleIndex = 0; triangleIndex < triangles.Length / 3; triangleIndex++)
        {
            for (int localEdge = 0; localEdge < 3; localEdge++)
            {
                int edgeNr = triangleIndex * 3 + localEdge;
                int id0 = triangles[triangleIndex * 3 + localEdge];
                int id1 = triangles[triangleIndex * 3 + (localEdge + 1) % 3];
                EdgeKey key = new EdgeKey(id0, id1);
                if (firstEdges.TryGetValue(key, out int otherEdge))
                {
                    neighbors[edgeNr] = otherEdge;
                    neighbors[otherEdge] = edgeNr;
                }
                else
                {
                    firstEdges.Add(key, edgeNr);
                }
            }
        }
        List<int> edgeIds = new List<int>();
        List<int> triPairIds = new List<int>();
        for (int triangleIndex = 0; triangleIndex < triangles.Length / 3; triangleIndex++)
        {
            for (int localEdge = 0; localEdge < 3; localEdge++)
            {
                int edgeNr = triangleIndex * 3 + localEdge;
                int id0 = triangles[triangleIndex * 3 + localEdge];
                int id1 = triangles[triangleIndex * 3 + (localEdge + 1) % 3];
                int neighbor = neighbors[edgeNr];
                if (neighbor < 0 || id0 < id1)
                {
                    edgeIds.Add(id0);
                    edgeIds.Add(id1);
                }
                if (neighbor >= 0)
                {
                    int neighborTriangle = neighbor / 3;
                    int neighborLocalEdge = neighbor % 3;
                    int id2 = triangles[triangleIndex * 3 + (localEdge + 2) % 3];
                    int id3 = triangles[neighborTriangle * 3 + (neighborLocalEdge + 2) % 3];
                    triPairIds.Add(id0);
                    triPairIds.Add(id1);
                    triPairIds.Add(id2);
                    triPairIds.Add(id3);
                }
            }
        }
        stretchingIds = edgeIds.ToArray();
        bendingIds = triPairIds.ToArray();
        stretchingLengths = new float[stretchingIds.Length / 2];
        bendingLengths = new float[bendingIds.Length / 4];
    }

    private void InitializePhysics()
    {
        Array.Clear(invMass, 0, invMass.Length);
        Array.Clear(vel, 0, vel.Length);
        Array.Copy(restPos, pos, restPos.Length);
        Array.Copy(restPos, prevPos, restPos.Length);
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            int id0 = triangles[3 * i];
            int id1 = triangles[3 * i + 1];
            int id2 = triangles[3 * i + 2];
            Vector3 e0 = pos[id1] - pos[id0];
            Vector3 e1 = pos[id2] - pos[id0];
            Vector3 cross = Vector3.Cross(e0, e1);
            float area = 0.5f * cross.magnitude;
            float particleInvMass = area > 0.0f ? 1.0f / area / 3.0f : 0.0f;
            invMass[id0] += particleInvMass;
            invMass[id1] += particleInvMass;
            invMass[id2] += particleInvMass;
        }
        for (int i = 0; i < stretchingLengths.Length; i++)
        {
            int id0 = stretchingIds[2 * i];
            int id1 = stretchingIds[2 * i + 1];
            stretchingLengths[i] = Vector3.Distance(pos[id0], pos[id1]);
        }
        for (int i = 0; i < bendingLengths.Length; i++)
        {
            int id0 = bendingIds[4 * i + 2];
            int id1 = bendingIds[4 * i + 3];
            bendingLengths[i] = Vector3.Distance(pos[id0], pos[id1]);
        }
        float minX = float.MaxValue;
        float maxX = -float.MaxValue;
        float maxY = -float.MaxValue;
        for (int i = 0; i < pos.Length; i++)
        {
            minX = Mathf.Min(minX, pos[i].x);
            maxX = Mathf.Max(maxX, pos[i].x);
            maxY = Mathf.Max(maxY, pos[i].y);
        }
        float eps = 0.0001f;
        for (int i = 0; i < pos.Length; i++)
        {
            float x = pos[i].x;
            float y = pos[i].y;
            if (y > maxY - eps && (x < minX + eps || x > maxX - eps))
            {
                invMass[i] = 0.0f;
            }
        }
        grabId = -1;
        grabTime = 0.0f;
        physicsStepCounter = 0;
    }

    private void SimulateStep()
    {
        stopwatch.Restart();
        float subDt = simulationDt / numSubsteps;
        float stretchAlpha = stretchingCompliance / subDt / subDt;
        float bendAlpha = bendingCompliance / subDt / subDt;
        float gravityX = gravity.x * subDt;
        float gravityY = gravity.y * subDt;
        float gravityZ = gravity.z * subDt;
        float inverseSubDt = 1.0f / subDt;
        for (int step = 0; step < numSubsteps; step++)
        {
            PreSolve(subDt, gravityX, gravityY, gravityZ);
            SolveStretching(stretchAlpha);
            SolveBending(bendAlpha);
            PostSolve(inverseSubDt);
        }
        ApplyRelativeVelocityDamping(simulationDt);
        physicsStepCounter++;
        UpdateMeshes(false);
        grabTime += simulationDt;
        stopwatch.Stop();
        timeSum += stopwatch.Elapsed.TotalMilliseconds;
        timeFrames++;
        if (timeFrames > 10)
        {
            averageMs = timeSum / timeFrames;
            timeFrames = 0;
            timeSum = 0.0;
        }
    }

    private void PreSolve(float dt, float gravityX, float gravityY, float gravityZ)
    {
        Vector3[] localPos = pos;
        Vector3[] localPrevPos = prevPos;
        Vector3[] localVel = vel;
        float[] localInvMass = invMass;
        int count = localPos.Length;
        for (int i = 0; i < count; i++)
        {
            if (localInvMass[i] == 0.0f)
            {
                continue;
            }
            Vector3 velocity = localVel[i];
            velocity.x += gravityX;
            velocity.y += gravityY;
            velocity.z += gravityZ;
            Vector3 position = localPos[i];
            localPrevPos[i] = position;
            position.x += velocity.x * dt;
            position.y += velocity.y * dt;
            position.z += velocity.z * dt;
            if (position.y < 0.0f)
            {
                position = localPrevPos[i];
                position.y = 0.0f;
            }
            localVel[i] = velocity;
            localPos[i] = position;
        }
    }

    private void PostSolve(float inverseDt)
    {
        Vector3[] localPos = pos;
        Vector3[] localPrevPos = prevPos;
        Vector3[] localVel = vel;
        float[] localInvMass = invMass;
        int count = localPos.Length;
        for (int i = 0; i < count; i++)
        {
            if (localInvMass[i] == 0.0f)
            {
                continue;
            }
            Vector3 position = localPos[i];
            Vector3 previous = localPrevPos[i];
            Vector3 velocity;
            velocity.x = (position.x - previous.x) * inverseDt;
            velocity.y = (position.y - previous.y) * inverseDt;
            velocity.z = (position.z - previous.z) * inverseDt;
            localVel[i] = velocity;
        }
    }

    private void ApplyRelativeVelocityDamping(float dt)
    {
        float dampingRate = velocityDamping;
        if (dampingRate <= 0.0f)
        {
            return;
        }
        float damping = 1.0f - Mathf.Exp(-dampingRate * dt);
        Vector3[] localPos = pos;
        Vector3[] localVel = vel;
        float[] localInvMass = invMass;
        int[] localIds = stretchingIds;
        int edgeCount = localIds.Length >> 1;
        for (int i = 0; i < edgeCount; i++)
        {
            int baseIndex = i << 1;
            int id0 = localIds[baseIndex];
            int id1 = localIds[baseIndex + 1];
            float w0 = localInvMass[id0];
            float w1 = localInvMass[id1];
            float w = w0 + w1;
            if (w == 0.0f)
            {
                continue;
            }
            Vector3 p0 = localPos[id0];
            Vector3 p1 = localPos[id1];
            float dx = p1.x - p0.x;
            float dy = p1.y - p0.y;
            float dz = p1.z - p0.z;
            float lenSquared = dx * dx + dy * dy + dz * dz;
            if (lenSquared <= 0.000000000001f)
            {
                continue;
            }
            float inverseLength = 1.0f / Mathf.Sqrt(lenSquared);
            float nx = dx * inverseLength;
            float ny = dy * inverseLength;
            float nz = dz * inverseLength;
            Vector3 v0 = localVel[id0];
            Vector3 v1 = localVel[id1];
            float relativeX = v0.x - v1.x;
            float relativeY = v0.y - v1.y;
            float relativeZ = v0.z - v1.z;
            float relativeAlongEdge = relativeX * nx + relativeY * ny + relativeZ * nz;
            float impulse = damping * relativeAlongEdge / w;
            float correction0 = impulse * w0;
            float correction1 = impulse * w1;
            v0.x -= nx * correction0;
            v0.y -= ny * correction0;
            v0.z -= nz * correction0;
            v1.x += nx * correction1;
            v1.y += ny * correction1;
            v1.z += nz * correction1;
            localVel[id0] = v0;
            localVel[id1] = v1;
        }
    }

    private void SolveStretching(float alpha)
    {
        Vector3[] localPos = pos;
        float[] localInvMass = invMass;
        int[] localIds = stretchingIds;
        float[] localRestLengths = stretchingLengths;
        int count = localRestLengths.Length;
        for (int i = 0; i < count; i++)
        {
            int baseIndex = i << 1;
            int id0 = localIds[baseIndex];
            int id1 = localIds[baseIndex + 1];
            float w0 = localInvMass[id0];
            float w1 = localInvMass[id1];
            float w = w0 + w1;
            if (w == 0.0f)
            {
                continue;
            }
            Vector3 p0 = localPos[id0];
            Vector3 p1 = localPos[id1];
            float dx = p0.x - p1.x;
            float dy = p0.y - p1.y;
            float dz = p0.z - p1.z;
            float lenSquared = dx * dx + dy * dy + dz * dz;
            if (lenSquared <= 0.000000000001f)
            {
                continue;
            }
            float len = Mathf.Sqrt(lenSquared);
            float scale = -(len - localRestLengths[i]) / (w + alpha) / len;
            float correction0 = scale * w0;
            float correction1 = scale * w1;
            p0.x += dx * correction0;
            p0.y += dy * correction0;
            p0.z += dz * correction0;
            p1.x -= dx * correction1;
            p1.y -= dy * correction1;
            p1.z -= dz * correction1;
            localPos[id0] = p0;
            localPos[id1] = p1;
        }
    }

    private void SolveBending(float alpha)
    {
        Vector3[] localPos = pos;
        float[] localInvMass = invMass;
        int[] localIds = bendingIds;
        float[] localRestLengths = bendingLengths;
        int count = localRestLengths.Length;
        for (int i = 0; i < count; i++)
        {
            int baseIndex = i << 2;
            int id0 = localIds[baseIndex + 2];
            int id1 = localIds[baseIndex + 3];
            float w0 = localInvMass[id0];
            float w1 = localInvMass[id1];
            float w = w0 + w1;
            if (w == 0.0f)
            {
                continue;
            }
            Vector3 p0 = localPos[id0];
            Vector3 p1 = localPos[id1];
            float dx = p0.x - p1.x;
            float dy = p0.y - p1.y;
            float dz = p0.z - p1.z;
            float lenSquared = dx * dx + dy * dy + dz * dz;
            if (lenSquared <= 0.000000000001f)
            {
                continue;
            }
            float len = Mathf.Sqrt(lenSquared);
            float scale = -(len - localRestLengths[i]) / (w + alpha) / len;
            float correction0 = scale * w0;
            float correction1 = scale * w1;
            p0.x += dx * correction0;
            p0.y += dy * correction0;
            p0.z += dz * correction0;
            p1.x -= dx * correction1;
            p1.y -= dy * correction1;
            p1.z -= dz * correction1;
            localPos[id0] = p0;
            localPos[id1] = p1;
        }
    }

    private void UpdateMeshes(bool forceNormals)
    {
        clothMesh.SetVertices(pos, 0, pos.Length, vertexUpdateFlags);
        bool updateNormals = forceNormals || normalUpdateInterval <= 1 || physicsStepCounter % normalUpdateInterval == 0;
        if (updateNormals)
        {
            RecalculateNormalsFast();
            clothMesh.SetNormals(meshNormals, 0, meshNormals.Length, vertexUpdateFlags);
        }
        if (showEdges)
        {
            edgeMesh.SetVertices(pos, 0, pos.Length, vertexUpdateFlags);
        }
    }

    private void RecalculateNormalsFast()
    {
        Array.Clear(meshNormals, 0, meshNormals.Length);
        Vector3[] localPos = pos;
        Vector3[] localNormals = meshNormals;
        int[] localTriangles = triangles;
        int triangleIndexCount = localTriangles.Length;
        for (int i = 0; i < triangleIndexCount; i += 3)
        {
            int id0 = localTriangles[i];
            int id1 = localTriangles[i + 1];
            int id2 = localTriangles[i + 2];
            Vector3 p0 = localPos[id0];
            Vector3 p1 = localPos[id1];
            Vector3 p2 = localPos[id2];
            float e10x = p1.x - p0.x;
            float e10y = p1.y - p0.y;
            float e10z = p1.z - p0.z;
            float e20x = p2.x - p0.x;
            float e20y = p2.y - p0.y;
            float e20z = p2.z - p0.z;
            Vector3 normal;
            normal.x = e10y * e20z - e10z * e20y;
            normal.y = e10z * e20x - e10x * e20z;
            normal.z = e10x * e20y - e10y * e20x;
            localNormals[id0] += normal;
            localNormals[id1] += normal;
            localNormals[id2] += normal;
        }
        int vertexCount = localNormals.Length;
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 normal = localNormals[i];
            float lenSquared = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
            if (lenSquared > 0.000000000001f)
            {
                float inverseLength = 1.0f / Mathf.Sqrt(lenSquared);
                normal.x *= inverseLength;
                normal.y *= inverseLength;
                normal.z *= inverseLength;
            }
            else
            {
                normal = Vector3.back;
            }
            localNormals[i] = normal;
        }
    }

    private void RestartSimulation()
    {
        paused = true;
        showEdges = false;
        bendingCompliance = 1.0f;
        accumulator = 0.0f;
        timeFrames = 0;
        timeSum = 0.0;
        averageMs = 0.0;
        InitializePhysics();
        UpdateMeshes(true);
        ResetCamera();
        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (clothRenderer != null)
        {
            clothRenderer.enabled = !showEdges;
        }
        if (edgeRenderer != null)
        {
            edgeRenderer.enabled = showEdges;
        }
        if (showEdges && edgeMesh != null && pos != null)
        {
            edgeMesh.SetVertices(pos, 0, pos.Length, vertexUpdateFlags);
        }
    }

    private void HandleGrabber()
    {
        if (sceneCamera == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !IsPointerOverGui())
        {
            TryStartGrab();
        }
        if (Input.GetMouseButton(0) && grabId >= 0)
        {
            MoveGrabbed();
        }
        if (Input.GetMouseButtonUp(0) && grabId >= 0)
        {
            EndGrab();
        }
    }

    private bool IsPointerOverGui()
    {
        return Input.mousePosition.x < 440.0f && Input.mousePosition.y > Screen.height - 165.0f;
    }

    private void TryStartGrab()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (!RaycastCloth(ray, out float distance, out Vector3 hitPoint))
        {
            return;
        }
        grabDistance = distance;
        float minDistanceSquared = float.MaxValue;
        grabId = -1;
        for (int i = 0; i < pos.Length; i++)
        {
            float distanceSquared = (hitPoint - pos[i]).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                grabId = i;
            }
        }
        if (grabId < 0)
        {
            return;
        }
        grabInvMass = invMass[grabId];
        invMass[grabId] = 0.0f;
        pos[grabId] = hitPoint;
        grabPrevPos = hitPoint;
        grabVelocity = Vector3.zero;
        grabTime = 0.0f;
        paused = false;
    }

    private void MoveGrabbed()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 newPos = ray.origin + ray.direction * grabDistance;
        grabVelocity = newPos - grabPrevPos;
        if (grabTime > 0.0f)
        {
            grabVelocity /= grabTime;
        }
        else
        {
            grabVelocity = Vector3.zero;
        }
        grabPrevPos = newPos;
        grabTime = 0.0f;
        pos[grabId] = newPos;
    }

    private void EndGrab()
    {
        invMass[grabId] = grabInvMass;
        vel[grabId] = grabVelocity;
        grabId = -1;
    }

    private bool RaycastCloth(Ray ray, out float hitDistance, out Vector3 hitPoint)
    {
        hitDistance = float.MaxValue;
        hitPoint = Vector3.zero;
        bool hit = false;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = pos[triangles[i]];
            Vector3 b = pos[triangles[i + 1]];
            Vector3 c = pos[triangles[i + 2]];
            if (!RayTriangle(ray, a, b, c, out float distance))
            {
                continue;
            }
            if (distance >= hitDistance)
            {
                continue;
            }
            hitDistance = distance;
            hitPoint = ray.origin + ray.direction * distance;
            hit = true;
        }
        return hit;
    }

    private static bool RayTriangle(Ray ray, Vector3 a, Vector3 b, Vector3 c, out float distance)
    {
        const float epsilon = 0.0000001f;
        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;
        Vector3 p = Vector3.Cross(ray.direction, edge2);
        float det = Vector3.Dot(edge1, p);
        if (Mathf.Abs(det) < epsilon)
        {
            distance = 0.0f;
            return false;
        }
        float invDet = 1.0f / det;
        Vector3 t = ray.origin - a;
        float u = Vector3.Dot(t, p) * invDet;
        if (u < 0.0f || u > 1.0f)
        {
            distance = 0.0f;
            return false;
        }
        Vector3 q = Vector3.Cross(t, edge1);
        float v = Vector3.Dot(ray.direction, q) * invDet;
        if (v < 0.0f || u + v > 1.0f)
        {
            distance = 0.0f;
            return false;
        }
        distance = Vector3.Dot(edge2, q) * invDet;
        return distance > epsilon;
    }

    private void HandleCamera()
    {
        if (sceneCamera == null)
        {
            return;
        }
        if (grabId < 0 && Input.GetMouseButton(1))
        {
            float yaw = Input.GetAxis("Mouse X") * orbitSpeed * 10.0f;
            float pitch = -Input.GetAxis("Mouse Y") * orbitSpeed * 10.0f;
            Vector3 offset = sceneCamera.transform.position - cameraTarget;
            offset = Quaternion.AngleAxis(yaw, Vector3.up) * offset;
            offset = Quaternion.AngleAxis(pitch, sceneCamera.transform.right) * offset;
            sceneCamera.transform.position = cameraTarget + offset;
            sceneCamera.transform.LookAt(cameraTarget);
        }
        if (grabId < 0 && Input.GetMouseButton(2))
        {
            float dx = -Input.GetAxis("Mouse X");
            float dy = -Input.GetAxis("Mouse Y");
            float distance = Vector3.Distance(sceneCamera.transform.position, cameraTarget);
            Vector3 delta = (sceneCamera.transform.right * dx + sceneCamera.transform.up * dy) * panSpeed * distance * 100.0f;
            cameraTarget += delta;
            sceneCamera.transform.position += delta;
        }
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.0f)
        {
            Vector3 offset = sceneCamera.transform.position - cameraTarget;
            float distance = offset.magnitude;
            distance *= Mathf.Exp(-scroll * zoomSpeed);
            distance = Mathf.Clamp(distance, 0.2f, 10.0f);
            sceneCamera.transform.position = cameraTarget + offset.normalized * distance;
            sceneCamera.transform.LookAt(cameraTarget);
        }
    }

    private void ResetCamera()
    {
        cameraTarget = new Vector3(0.0f, 0.6f, 0.0f);
        sceneCamera.transform.position = initialCameraPosition;
        sceneCamera.transform.rotation = initialCameraRotation;
    }

    private static Material CreateLitMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        Material material = new Material(shader);
        material.color = color;
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Cull"))
        {
            material.SetInt("_Cull", (int)CullMode.Off);
        }
        return material;
    }

    private static Material CreateUnlitMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        Material material = new Material(shader);
        material.color = color;
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        return material;
    }
}
