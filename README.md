# C# scripts and systems catalog

A source-based index of [Unity3D-C-programming](https://github.com/przemyslawzaworski/Unity3D-C-programming), reviewed from the supplied local snapshot on 26 October 2026.

**Coverage:** all **209 repository files**: 199 C# files, 2 C++ files, 2 shaders, 3 text files and 3 Markdown files. The catalog contains **198 entries** grouped by topic. Companion sources and feature-specific readmes are listed together; the root [README](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/README.md) is linked here.

## Reading the collection

This repository is a collection of learning examples, utilities and experimental systems, rather than one ready-to-build Unity project. Choose the feature you need and bring its dependencies into a suitable project. Descriptions reflect source inspection; no Unity compilation or runtime tests were performed.

- **Complete systems are grouped:** the four character-controller scripts, TCP client/server pair, native C#/C++ pairs and editor/shader pairs each form one entry.
- **Collection folders contain alternatives:** Addressables, AssetBundle, Math and Editor hold independent examples. Similar filenames do not automatically indicate a dependency. In particular, Math/Parabola.cs and Math/ParabolicMovement.cs each implement their own path calculations.
- **A shared helper crosses feature boundaries:** DrippingParticles.cs needs the ClosestPoint class in ClosestPointDemo.cs. Keep that source available when extracting the dripping-particle example.
- **Some source is embedded:** AVX.cs, Library.cs, LoaderPPM.cs and NativeCombineString.cs contain native helper code in comments; Editor/MaterialInspector.cs contains a companion shader in a comment; Editor/Scatterplot.cs creates its shader from a source string.
- **Text files also contain code:** the two Addressables `.txt` files are complete alternative C# examples. The root notepad.txt contains many independent snippets and requires selective extraction.
- **Separate editor and runtime code:** several root scripts and AddressableToolsEditor.cs use UnityEditor despite being outside an Editor folder. Some files mix editor drawers and runtime demo components; split the editor portions or guard them for a player build.
- **Avoid combining all examples into one assembly unchanged:** LoadTextureAsync.cs, UnityEnjin.cs and UnityOpenAI.cs repeat global awaiter types. UnityEnjin.cs and UnityOpenAI.cs also repeat helper class names. Demo class names such as MaterialPropertyBlock and Tuple can shadow framework types; qualify those types or isolate the examples.
- **Check the version and configuration implied by each example:** the collection includes UNet, Post Processing Stack v1, WWW, BinaryFormatter, CodeDOM, Reflection.Emit and internal editor reflection. Windows/macOS/Android/native-graphics examples also require their stated platform. These are source observations, not current compatibility guarantees.

## License and external dependencies

The repository owner has specified **MIT** for the scripts and examples in this catalog. The supplied snapshot has no separate LICENSE file or license declaration in its root README; the MIT column records that owner declaration. Original third-party credits and license notices must remain with adapted code. A dependency or hosted service retains its own terms; the MIT label on a calling script does not relicense that dependency.

No paid third-party package binaries are bundled in this snapshot. Source inspection found the following external requirements or attribution points:

| Example | External requirement / finding |
| --- | --- |
| DemoFMOD.cs | Requires FMOD Studio/Unity integration. FMOD has its own licensing: qualifying projects can use the free tier, while other commercial projects require a paid license. See [FMOD licensing](https://www.fmod.com/licensing). The integration script is cataloged as MIT. |
| Addressables MEC text examples | Both source comments link specifically to [More Effective Coroutines FREE](https://assetstore-fallback.unity.com/packages/tools/animation/more-effective-coroutines-free-54975). That package is free of charge under its own Unity Asset Store terms; a paid MEC Pro dependency is not identified in these examples. |
| RaycastVR.cs | Uses the external Oculus/OVR APIs and rig objects. The SDK is not included. |
| Addressables / Jobs / editor coroutines / Draw3DText / legacy post-processing | Require the relevant Unity packages or components: Addressables, Jobs/Collections, Editor Coroutines, TextMeshPro or Post Processing Stack v1. Package implementations are not bundled here. |
| UnityOpenAI.cs / UnityJira.cs / UnityEnjin.cs / Translator.cs / UnitySQL.cs | Connect to external APIs/services or a database; accounts, credentials, endpoint availability and service fees are separate from the script license. The OpenAI and Enjin examples contain historical model/endpoint settings. |
| SoftBodyStanfordBunny.cs | Credits Matthias Müller's Ten Minute Physics source. The [original source header](https://raw.githubusercontent.com/matthias-research/pages/master/tenMinutePhysics/10-softBodies.html) grants MIT permission; retain its copyright and permission notice when redistributing the adaptation. |
| DisassemblerTest.cs / collections.cs and other reference-based examples | Preserve existing source links and credits. This catalog does not assert that every externally referenced page has been independently licensed/audited. |
| notepad.txt Steam networking snippets | Require Steamworks bindings and a configured Steamworks environment; this notebook does not include the SDK. |
| Native and GPU-compute examples | Require compiled helper DLLs, operating-system libraries and/or the appropriate CUDA/OpenCL/graphics runtime. The custom DLL source is either supplied separately or embedded in comments, as described below. |

## Contents

- [Movement and input](#movement-and-input) (14)
- [Physics and simulation](#physics-and-simulation) (10)
- [Geometry and spatial queries](#geometry-and-spatial-queries) (13)
- [Math and curves](#math-and-curves) (17)
- [Rendering and procedural content](#rendering-and-procedural-content) (20)
- [Terrain and vegetation](#terrain-and-vegetation) (9)
- [UI and interaction](#ui-and-interaction) (6)
- [Audio](#audio) (4)
- [Assets and scene loading](#assets-and-scene-loading) (11)
- [Scene objects and lifecycle](#scene-objects-and-lifecycle) (3)
- [Files and serialization](#files-and-serialization) (11)
- [Data and algorithms](#data-and-algorithms) (15)
- [Asynchronous work and performance](#asynchronous-work-and-performance) (6)
- [Reflection and compilation](#reflection-and-compilation) (3)
- [Editor and inspector tools](#editor-and-inspector-tools) (26)
- [Diagnostics and profiling](#diagnostics-and-profiling) (3)
- [Networking and services](#networking-and-services) (11)
- [Platform and native integration](#platform-and-native-integration) (15)
- [Notebook and reference material](#notebook-and-reference-material) (1)

## Movement and input

| Files | Short description | License |
| --- | --- | --- |
| [AnchorBasedScaler.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AnchorBasedScaler.cs) | Scales a box along a selected axis while keeping its minimum or maximum side fixed by compensating the transform position. | MIT |
| [car.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/car.cs) | WheelCollider-based car controller with acceleration, speed-dependent steering, wheel-mesh pose updates, friction and suspension settings. | MIT |
| [Character Controller/CharacterMotor.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Character%20Controller/CharacterMotor.cs)<br>[Character Controller/FPSInputController.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Character%20Controller/FPSInputController.cs)<br>[Character Controller/PlatformInputController.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Character%20Controller/PlatformInputController.cs)<br>[Character Controller/MouseLook.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Character%20Controller/MouseLook.cs)<br>[Character Controller/Readme.md](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Character%20Controller/Readme.md) | Complete legacy character-control system: CharacterMotor handles acceleration, gravity, jumping, slopes and moving platforms; FPSInputController and PlatformInputController are alternative input adapters; MouseLook controls the view. Uses Unity CharacterController and legacy Input axes. | MIT |
| [Controller.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Controller.cs) | Windows joystick/controller polling through joyGetPosEx, mapping analog axes to object translation and camera-style pitch/yaw. | MIT |
| [direction.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/direction.cs) | Rotates an object in 2D so it faces a target position. | MIT |
| [Elevator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Elevator.cs) | Rigidbody elevator demo with configurable upward/downward velocity; a keyboard control toggles motion and the kinematic state. | MIT |
| [fly.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/fly.cs) | Free-flying camera/object controller with WASD translation, mouse look, vertical movement and a faster movement modifier. | MIT |
| [MobileInput.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MobileInput.cs) | Multi-touch controller moving and rotating assigned scene objects from touch position/delta regions. | MIT |
| [MovementControl.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MovementControl.cs) | Detects whether an object is moving forward, backward or standing still by comparing position change with its forward direction. | MIT |
| [RaycastVR.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/RaycastVR.cs) | Oculus/OVR right-controller ray interaction with a visible beam and projectile spawning. Requires OVRInput/OVR rig setup and assigned scene objects. | MIT |
| [rotation.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/rotation.cs) | Steering-wheel-style rotation demo with Euler and quaternion approaches; source explains the quaternion limitation for large steering angles. | MIT |
| [speed.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/speed.cs) | Measures an object's speed from frame-to-frame world-position differences and displays it on screen. | MIT |
| [Touchpad.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Touchpad.cs) | Two on-screen UI touchpad regions controlling player movement and look rotation from pointer input. | MIT |
| [transform.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/transform.cs) | Minimal keyboard-driven Transform examples for resetting position, translating, rotating and changing scale; the class name itself can shadow common Unity identifiers. | MIT |

## Physics and simulation

| Files | Short description | License |
| --- | --- | --- |
| [ArmorPenetration.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ArmorPenetration.cs) | Educational kinetic-energy/steel-armor penetration calculation using projectile mass, diameter, velocity, armor thickness and impact angle. | MIT |
| [BlastWave.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/BlastWave.cs) | Educational Kingery-Bulmash TNT blast-parameter evaluator using scaled distance, including pressure, impulse, arrival/duration and shock velocity outputs. | MIT |
| [ClothSimulation.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ClothSimulation.cs) | Self-contained interactive cloth demo with generated grid geometry, XPBD-style stretching/bending constraints, compliance, damping, floor contact, mouse grabbing and orbit/pan/zoom camera controls. | MIT |
| [destruction.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/destruction.cs) | Creates a cube construction from smaller blocks and turns its pieces into rigidbodies when destruction is triggered, distributing the original mass. | MIT |
| [Displacement.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Displacement.cs) | Moves a sphere along X using the example displacement function 0.5 * t * sin(t), displaying elapsed time and displacement. | MIT |
| [DrippingParticles.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DrippingParticles.cs) | Emits blue particles at mouse hits and moves them along collider surfaces using closest-point queries. Requires the ClosestPoint class declared in [ClosestPointDemo.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ClosestPointDemo.cs), plus Unity Jobs/Collections and unsafe support. | MIT |
| [ProjectileMotion.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ProjectileMotion.cs) | Analytic 3D projectile-motion demonstration with gravity-only and drag variants, launch-angle helpers and a comparison against a Rigidbody projectile. | MIT |
| [SoftBodyStanfordBunny.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/SoftBodyStanfordBunny.cs) | Self-contained deformable Stanford bunny with embedded tetrahedral geometry, edge/volume constraints, gravity and ground collision. Port of Matthias Müller's [Ten Minute Physics soft-body example](https://raw.githubusercontent.com/matthias-research/pages/master/tenMinutePhysics/10-softBodies.html); Space applies an extra force. | MIT |
| [Verlet.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Verlet.cs) | Single-sphere Verlet integration demo with gravity, friction and bounce against box bounds; keyboard controls add motion along individual axes. | MIT |
| [VerletExtended.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/VerletExtended.cs) | Multi-sphere extension of the Verlet experiment with pairwise collision handling and shared boundary constraints; source notes that it needs improvement. | MIT |

## Geometry and spatial queries

| Files | Short description | License |
| --- | --- | --- |
| [AnimationBounds.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AnimationBounds.cs) | Samples an animation state to accumulate a bounding box covering its frames and displays the resulting size. | MIT |
| [barycentric_coordinates.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/barycentric_coordinates.cs) | Demonstrates several formulas for triangle barycentric coordinates and tests a sample point against a triangle. | MIT |
| [BoxBoxIntersection.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/BoxBoxIntersection.cs) | Oriented-box intersection test using the Separating Axis Theorem and the fifteen candidate box/edge axes. | MIT |
| [BoxColliderUtils.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/BoxColliderUtils.cs) | Calculates box-collider corners and their projected screen-space bounding rectangle. | MIT |
| [ClosestPointDemo.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ClosestPointDemo.cs) | Reusable Jobs-based ClosestPoint helper finding the nearest point on mesh triangles, plus a scene visualization demo. The same helper is required by DrippingParticles.cs. | MIT |
| [ClosestVertexDemo.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ClosestVertexDemo.cs) | Jobs-based nearest-mesh-vertex search with persistent NativeArray buffers and a scene visualization example; distinct from closest-point-on-triangle queries. | MIT |
| [DelaunayTriangulation.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DelaunayTriangulation.cs) | Bowyer-Watson triangulation of 2D points, with incremental triangle replacement and scene-line visualization. | MIT |
| [Frustum.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Frustum.cs) | Transforms mesh vertices through camera matrices and checks whether the entire mesh lies inside the camera frustum. | MIT |
| [intersection_tetrahedron.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/intersection_tetrahedron.cs) | Tests whether a point lies inside a tetrahedron by comparing determinant signs after replacing each vertex with the test point. | MIT |
| [LinearQuadtree.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LinearQuadtree.cs) | Experimental linear-quadtree address calculation using two-bit quadrant codes and a scene-point lookup; explicitly marked work in progress. | MIT |
| [MeshCentroid.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MeshCentroid.cs) | Calculates a mesh surface-area-weighted centroid by accumulating triangle centroids and triangle areas. | MIT |
| [MeshVolume.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MeshVolume.cs) | Calculates a polyhedron's signed/absolute volume from triangle contributions; intended for a closed, consistently wound mesh. | MIT |
| [PointInMesh.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/PointInMesh.cs) | Compares several point-inside-closed-mesh approaches, including ray/triangle intersection parity, closest-hit orientation and collider raycasts; requires closed geometry for meaningful inside/outside results. | MIT |

## Math and curves

| Files | Short description | License |
| --- | --- | --- |
| [Calculus.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Calculus.cs) | Visualizes displacement, velocity and acceleration functions, numerical first/second derivatives, and repeated trapezoidal integration with colored sample geometry. | MIT |
| [CartesianPolar.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/CartesianPolar.cs) | Cartesian/polar conversion helpers and a scene example moving an object relative to a center using polar coordinates. | MIT |
| [CatmullRom.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/CatmullRom.cs) | Evaluates a Catmull-Rom curve and places scene segments along the interpolated path. | MIT |
| [Derivatives.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Derivatives.cs) | Compares a finite-difference derivative against an analytic derivative by plotting both alongside the original function. | MIT |
| [fibonacci.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/fibonacci.cs) | Iterative Fibonacci-number calculation with a small logged usage example. | MIT |
| [Fourier.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Fourier.cs) | Straightforward discrete Fourier transform and inverse transform for complex samples represented as two-column double arrays. | MIT |
| [Integrals.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Integrals.cs) | Visualizes numerical trapezoidal integration and compares the accumulated result with an analytic antiderivative. | MIT |
| [Math/Parabola.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Math/Parabola.cs) | Computes a quadratic path through endpoints and a selected midpoint height/angle, then draws sampled positions in scene gizmos. | MIT |
| [Math/ParabolicMovement.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Math/ParabolicMovement.cs) | Independent animated version of the quadratic-path example: caches path coefficients and moves a projectile between source and destination over a specified duration. | MIT |
| [matrix_multiplication.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/matrix_multiplication.cs) | Multiplies rectangular float matrices with nested loops and logs a sample 4 x 4 result. | MIT |
| [Mercator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Mercator.cs) | Conversion helpers between Cartesian, spherical and Mercator-style coordinates. | MIT |
| [parabola.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/parabola.cs) | Simple parameterized parabolic motion example launched from a starting transform with keyboard input; independent of the richer Math-folder path tools. | MIT |
| [PrimeFactorization.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/PrimeFactorization.cs) | Returns the prime factors of an integer through repeated trial division. | MIT |
| [quaternion.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/quaternion.cs) | Implements Euler-to-quaternion and quaternion-to-Euler formulas and compares them with Unity rotation conversions. | MIT |
| [Splines.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Splines.cs) | Curve interpolation collection covering Catmull-Rom, Hermite, cubic Bezier, Kochanek-Bartels, B-spline and Akima variants, with plotted scene samples. | MIT |
| [Statistics.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Statistics.cs) | Statistical helpers for Pearson correlation, linear-regression prediction and normal-distribution cumulative probability. | MIT |
| [Tuple.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Tuple.cs) | Demonstrates returning multiple calculated values with tuples, including geometric measurements of a triangle. | MIT |

## Rendering and procedural content

| Files | Short description | License |
| --- | --- | --- |
| [blit.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/blit.cs) | Manual render-texture blit helpers using GL quads and a material, demonstrating a lower-level equivalent of Graphics.Blit. | MIT |
| [Capsule.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Capsule.cs) | Generates a grayscale 2D capsule-distance texture on the CPU and assigns it to a renderer. | MIT |
| [cubemap_generator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/cubemap_generator.cs) | Editor wizard rendering a scene from a chosen position into a cubemap and exporting its six faces as PNG images. | MIT |
| [Draw3DText.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Draw3DText.cs) | Creates many world-space TextMeshPro labels and updates their scene placement; includes a helper for enabling batching through editor reflection. Requires TextMeshPro. | MIT |
| [fullscreen.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/fullscreen.cs) | Minimal Built-in Render Pipeline camera post-process applying a material with Graphics.Blit inside OnRenderImage. | MIT |
| [image.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/image.cs) | Writes an animated RGBA texture through an IJob and the Texture2D raw-data NativeArray, then displays it on a target renderer. | MIT |
| [instantiate.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/instantiate.cs) | Instantiates a prefab in a regularly spaced two-dimensional grid. | MIT |
| [invisible_object.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/invisible_object.cs) | Disables the attached Renderer at startup to hide an object's visible geometry. | MIT |
| [LightMapManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LightMapManager.cs) | Assigns a lightmap texture and shared lightmap index/UV scale-offset to renderers in a hierarchy. | MIT |
| [MaterialPropertyBlock.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MaterialPropertyBlock.cs) | Changes a renderer's color through UnityEngine.MaterialPropertyBlock without creating a new material instance; explicitly qualifies the Unity type inside the same-named demo class. | MIT |
| [NativePluginOpenGL/NativePlugin.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativePluginOpenGL/NativePlugin.cs)<br>[NativePluginOpenGL/NativePlugin.cpp](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativePluginOpenGL/NativePlugin.cpp)<br>[NativePluginOpenGL/Readme.md](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativePluginOpenGL/Readme.md) | Native OpenGL rendering plugin: C# issues a render-thread callback; C++ compiles embedded GLSL and draws a procedural Unity-logo image. Includes build instructions; compile the DLL and use OpenGL. | MIT |
| [NativeTexture.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativeTexture.cs)<br>[NativeTexture.cpp](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativeTexture.cpp) | Native DirectX 11 texture example: C# reads an uncompressed 32-bit TGA, the C++ plugin creates a GPU texture, and Unity wraps it as an external Texture2D. Requires a separately compiled Windows DLL. | MIT |
| [noise_texture_generator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/noise_texture_generator.cs) | Generates a 256 x 256 grayscale pseudo-random noise texture on the CPU and saves it as PNG to a fixed path. | MIT |
| [Plasma.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Plasma.cs) | Animated procedural plasma texture calculated with IJobParallelFor directly into Texture2D raw pixel data. | MIT |
| [post_processing_stack_setup.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/post_processing_stack_setup.cs) | Programmatically adjusts legacy Post Processing Stack v1 color-grading settings. Requires the corresponding package/profile and source-specified scripting define. | MIT |
| [ProceduralTextureUnsafe.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ProceduralTextureUnsafe.cs) | Generates animated sand-wave pixel colors in unmanaged memory and uploads the buffer to Texture2D; requires unsafe C# compilation. | MIT |
| [raymarching.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/raymarching.cs) | CPU raymarcher rendering a lit signed-distance sphere into a 512 x 512 texture and exporting a PNG image. | MIT |
| [SoftwareRasterizer.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/SoftwareRasterizer.cs) | CPU triangle rasterizer drawing a transformed cube into a Texture2D framebuffer using barycentric coverage/color calculations. | MIT |
| [SunPositionCalculator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/SunPositionCalculator.cs) | Calculates solar elevation/azimuth from date, time zone and geographic location using NOAA-style equations and orients a scene transform accordingly. | MIT |
| [wireframe.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/wireframe.cs) | Camera render callbacks toggling GL.wireframe to render the camera view as mesh wireframes. | MIT |

## Terrain and vegetation

| Files | Short description | License |
| --- | --- | --- |
| [Editor/TerrainTreesManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/TerrainTreesManager.cs) | Editor utility inspecting and reorganizing terrain tree prototypes, using DataTable sorting and asset-path-based prototype reconstruction. | MIT |
| [heightmap_generator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/heightmap_generator.cs) | Generates a procedural fBM heightmap on the CPU, creates a Unity Terrain and provides a button for regenerating it. | MIT |
| [LabyrinthGenerator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LabyrinthGenerator.cs) | Generates a noise-thresholded cube labyrinth and combines its wall geometry into one mesh; distinct from the Prim maze generator. | MIT |
| [LSystem.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LSystem.cs) | Rewrites an L-system grammar, interprets branching transforms with a stack and builds combined capsule-based plant geometry. | MIT |
| [Maze/Prims.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Maze/Prims.cs) | Generates a grid maze using randomized Prim-style wall expansion, then combines wall-cube geometry into one mesh. | MIT |
| [ProceduralBirds.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ProceduralBirds.cs) | Creates billboard bird geometry/textures from embedded data and animates a flock procedurally with randomized flight parameters. | MIT |
| [terrain_chunk_collider.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/terrain_chunk_collider.cs) | Maintains a smaller terrain-collider chunk around a moving position by copying nearby heights from a larger base Terrain. | MIT |
| [terrain_splatmap.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/terrain_splatmap.cs) | Samples terrain alphamap weights at an object's world position and reports the corresponding terrain texture/layer name. | MIT |
| [terrain_utilities.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/terrain_utilities.cs) | Helpers returning a terrain's maximum height in local space, world space or normalized form. | MIT |

## UI and interaction

| Files | Short description | License |
| --- | --- | --- |
| [button.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/button.cs) | UI Button callback example reading the sprite name of the currently selected button image. | MIT |
| [drag_rigidbody.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/drag_rigidbody.cs) | Mouse-drag example converting cursor screen coordinates to world coordinates and directly updating the object Transform while preserving the grab offset. | MIT |
| [RawImageWithMouse.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/RawImageWithMouse.cs) | Converts cursor input into RawImage-local coordinates and supplies iMouse/iResolution shader properties to its material. | MIT |
| [ShowOnClickEventsFromButtons.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ShowOnClickEventsFromButtons.cs) | Exports an HTML report listing persistent UI Button onClick targets and method names with hierarchy paths. | MIT |
| [slider.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/slider.cs) | UI Slider callback mapping its value to a target object's X position. | MIT |
| [speedometer.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/speedometer.cs) | Converts a speed in metres per second to km/h and rotates a gauge pointer to the corresponding angle. | MIT |

## Audio

| Files | Short description | License |
| --- | --- | --- |
| [DemoFMOD.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DemoFMOD.cs) | FMOD Studio integration example creating a 3D event instance, caching a parameter ID, starting playback and releasing the event. Requires the external FMOD Unity integration and matching banks/events. | MIT |
| [footsteps.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/footsteps.cs) | Plays footstep/landing clips selected by collider physics material, with walking/running intervals based on measured speed. Uses CharacterController contact callbacks and an AudioSource. | MIT |
| [PlayMusic.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/PlayMusic.cs) | WAV playlist player loading files from StreamingAssets with UnityWebRequestMultimedia, with next/previous track controls. | MIT |
| [SoundExclusion.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/SoundExclusion.cs) | Enables/disables assigned AudioSources when the camera enters or leaves box-defined sound-exclusion regions. | MIT |

## Assets and scene loading

| Files | Short description | License |
| --- | --- | --- |
| [AddressableAssetSystem/AddressableManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableManager.cs) | Addressables prefab loader returning an asset handle together with the instantiated object, with explicit unload/release and keyboard-controlled usage. Requires Unity Addressables. | MIT |
| [AddressableAssetSystem/AddressableManagerAsync.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableManagerAsync.cs) | Alternative Addressables prefab loader resolving a resource location before loading/instantiating the object; demonstrates async/await and release. Independent of AddressableManager.cs. | MIT |
| [AddressableAssetSystem/AddressableManagerTextures.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableManagerTextures.cs) | Loads and releases an Addressable texture through AssetReferenceTexture2D and assigns it to a generated plane. Requires Unity Addressables. | MIT |
| [AddressableAssetSystem/AddressableManagerTexturesMEC.txt](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableManagerTexturesMEC.txt) | Alternative texture-loading example using MEC coroutines and multiple Addressable textures/planes. C# source stored as text; requires Addressables and MEC Free. | MIT |
| [AddressableAssetSystem/AddressableManagerWithCoroutine.txt](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableManagerWithCoroutine.txt) | Alternative Addressables prefab-loading example using MEC coroutine handles and resource-location lookup. C# source stored as text; requires Addressables and MEC Free. | MIT |
| [AddressableAssetSystem/AddressableToolsEditor.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AddressableAssetSystem/AddressableToolsEditor.cs) | Editor window listing Addressables labels, addresses and groups, building a label/address dictionary, and creating/removing groups and entries. Requires Addressables; belongs in an Editor assembly. | MIT |
| [AssetBundle/AssetBundleAsyncAwait.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AssetBundle/AssetBundleAsyncAwait.cs) | Loads a texture from an AssetBundle using async/await, with custom awaiters for AssetBundleCreateRequest and AssetBundleRequest. Self-contained alternative to the other bundle loaders. | MIT |
| [AssetBundle/AssetBundleManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AssetBundle/AssetBundleManager.cs) | Loads a prefab from a file-based AssetBundle, tracks the bundle and instantiated object, and demonstrates unloading them. | MIT |
| [AssetBundle/Editor/CreateAssetBundles.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AssetBundle/Editor/CreateAssetBundles.cs) | Editor window building asset bundles from selected prefab sources for deployment with the runtime bundle-loading examples. | MIT |
| [AssetBundle/LoadAssetBundle.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AssetBundle/LoadAssetBundle.cs) | Minimal synchronous loader instantiating named prefabs from a bundle in StreamingAssets. | MIT |
| [LoadSceneAsyncAwait.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadSceneAsyncAwait.cs) | Wraps asynchronous scene loading in a Task and starts loading a named scene from keyboard input. | MIT |

## Scene objects and lifecycle

| Files | Short description | License |
| --- | --- | --- |
| [Pool.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Pool.cs) | Unity ObjectPool<GameObject> example reusing falling rigidbody spheres and returning them to the pool through a companion SphereController defined in the same file. | MIT |
| [quit_and_cursoroff.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/quit_and_cursoroff.cs) | Hides/locks the cursor for gameplay and quits the application when Escape is pressed. | MIT |
| [set_all_materials.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/set_all_materials.cs) | Replaces every material slot on renderers in a target hierarchy with a chosen material. | MIT |

## Files and serialization

| Files | Short description | License |
| --- | --- | --- |
| [LoadCSV.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadCSV.cs) | Reads delimited text into lists of lists, jagged arrays or rectangular arrays using simple delimiter splitting. | MIT |
| [LoadDDS.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadDDS.cs) | Loads DDS texture payloads into Texture2D with header-derived dimensions, supported compression format and mipmap handling. | MIT |
| [LoaderPPM.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoaderPPM.cs) | Loads a PPM image through a native helper DLL, marshals RGBA bytes and flips the texture vertically. Includes the C/C++ loader source in a comment. | MIT |
| [LoadPC2.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadPC2.cs) | Reads POINTCACHE2 point-cache animation and visualizes animated points with scene cubes; includes a SpeedTree wind-export workflow and an example path to replace. | MIT |
| [LoadTextureAsync.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadTextureAsync.cs) | Adds a custom awaiter for UnityWebRequestAsyncOperation and asynchronously downloads/loads a texture into a material property. | MIT |
| [LoadTextureAsyncFromDDS.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadTextureAsyncFromDDS.cs) | Asynchronously reads a DDS file, creates its Texture2D from compressed payload bytes and supports loading/unloading it on a material. | MIT |
| [LoadTGA.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadTGA.cs) | Minimal loader for uncompressed 32-bit TGA textures using BGRA32 data, with mipmaps and CPU-readback storage disabled. | MIT |
| [ObjectManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ObjectManager.cs) | Demonstrates writing/reading explicitly laid-out object data through Marshal and raw byte files, using two low-level approaches rather than a high-level serializer. | MIT |
| [save_float_array_to_file.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/save_float_array_to_file.cs) | Minimal float-array binary round trip using Buffer.BlockCopy and file byte buffers. | MIT |
| [Serialization.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Serialization.cs) | Legacy BinaryFormatter example persisting a map struct containing heightmap/alphamap arrays and reconstructing the stored data. | MIT |
| [text_to_quotes.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/text_to_quotes.cs) | Reads a text file line by line and writes each line inside quotation marks to another file for source-string preparation. | MIT |

## Data and algorithms

| Files | Short description | License |
| --- | --- | --- |
| [ArrayHelper.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ArrayHelper.cs) | Binary save/load helpers for one-, two- and three-dimensional int/float arrays using contiguous byte buffers; multidimensional reconstruction requires dimensions. | MIT |
| [Bit.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Bit.cs) | Bit/byte extraction and insertion helpers for 32-bit unsigned integers, including counting set bits. | MIT |
| [collections.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/collections.cs) | Interactive collection of .NET container examples, including lists, dictionaries, sets, stacks, queues, concurrent collections and related utilities. Also contains editor-console reflection code. | MIT |
| [crc.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/crc.cs) | Calculates a file CRC checksum using a lookup table and byte-by-byte processing. | MIT |
| [Cryptography.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Cryptography.cs) | Symmetric AES encrypt/decrypt example using CryptoStream and byte arrays, with fixed demonstration key/IV values and a text round trip. | MIT |
| [cwr.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/cwr.cs) | Enumerates combinations without repetition using an integer-index combination array and a next-combination routine. | MIT |
| [Database.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Database.cs) | In-memory DataTable/DataView example sorting numeric rows by several columns; does not connect to an external database. | MIT |
| [Domain.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Domain.cs) | LINQ set-operation examples for arrays: union, subtraction, intersection and duplicate removal. | MIT |
| [Events.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Events.cs) | Compares Action callbacks, UnityEvent<float> and EventHandler with custom EventArgs, triggered by keyboard input. | MIT |
| [FileWatcher.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/FileWatcher.cs) | Watches text files with FileSystemWatcher and passes change notifications through a ConcurrentQueue for processing on Unity's main thread. | MIT |
| [generic.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/generic.cs) | Small generic-method example using dynamic arithmetic to combine numeric values or concatenate strings. | MIT |
| [Levenshtein.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Levenshtein.cs) | Computes edit distance between two strings using a dynamic-programming matrix. | MIT |
| [ObjectComparer.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ObjectComparer.cs) | Compares serialized representations of objects by XML-serializing them to byte arrays and calling native memcmp; includes matching/nonmatching object examples. | MIT |
| [random_set.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/random_set.cs) | Builds a set of distinct random integers using explicit duplicate checks. | MIT |
| [shuffle.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/shuffle.cs) | Randomly rearranges an array in place using element swaps, with a Vector4-array example. | MIT |

## Asynchronous work and performance

| Files | Short description | License |
| --- | --- | --- |
| [AsyncAwait.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AsyncAwait.cs) | Demonstrates sequential asynchronous work with progress logging, short delays between iterations and a cancellation flag; the CPU calculations themselves run before each await. | MIT |
| [DiskSpeed.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DiskSpeed.cs) | Measures sequential asynchronous disk write/read throughput using a temporary 1 GiB test file and Stopwatch timings. | MIT |
| [ParallelInvoke.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ParallelInvoke.cs) | Generates procedural fBM textures with Parallel.Invoke and compares the multi-threaded work with a sequential version. | MIT |
| [RaycastCommandManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/RaycastCommandManager.cs) | Reusable batched-raycast manager allocating NativeArrays, preparing commands with IJobParallelFor and returning results from RaycastCommand.ScheduleBatch; includes explicit resource release. | MIT |
| [StaticBatching.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/StaticBatching.cs) | Compares static batching, material-grouped mesh combination and instancing-enabled materials on generated geometry, with cleanup. The mesh-combiner path references MaterialNameComparer1, whose definition is absent from this snapshot. | MIT |
| [threading.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/threading.cs) | Splits point-versus-box intersection tests across Task.Run workers and combines their boolean results, with generated test geometry. | MIT |

## Reflection and compilation

| Files | Short description | License |
| --- | --- | --- |
| [Compiler.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Compiler.cs) | Compiles C# source strings with CodeDOM and adds the resulting MonoBehaviour types through reflection. Includes sample component sources; expects a compatible .NET/Mono environment. | MIT |
| [DisassemblerTest.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DisassemblerTest.cs) | Disassembles a method's .NET intermediate language using reflection and opcode metadata; demonstrates the result on a sample method. Credits C# in a Nutshell. | MIT |
| [DynamicCompilation.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/DynamicCompilation.cs) | Reflection.Emit examples generating a dynamic list-sorting method and a dynamic assembly/type with an integer-addition method. | MIT |

## Editor and inspector tools

| Files | Short description | License |
| --- | --- | --- |
| [ColoredField.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ColoredField.cs) | Custom property attribute and PropertyDrawer coloring a serialized field in the Inspector. Contains runtime demo and editor drawer in one file. | MIT |
| [CustomEditorExample.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/CustomEditorExample.cs) | Demonstrates a custom Inspector and an EditorWindow locating its drawer through Unity editor reflection. Demo component and editor types share one file. | MIT |
| [Editor/AlignViewToObject.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/AlignViewToObject.cs) | Editor tools positioning/orienting a scene view or camera relative to a selected object and its bounds. | MIT |
| [Editor/AssembliesInfo.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/AssembliesInfo.cs) | Exports an HTML report mapping project scripts and assembly-definition assets to their compiled assembly names. | MIT |
| [Editor/DynamicProperties.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/DynamicProperties.cs) | Editor window dynamically listing serializable types, drawing their public serialized fields and loading/saving JSON; builds its type-selection enum with Reflection.Emit. | MIT |
| [Editor/EditorBuild.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/EditorBuild.cs) | Sorts the scenes in Editor Build Settings alphabetically by scene path. | MIT |
| [Editor/EditorCoroutines.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/EditorCoroutines.cs) | Demonstrates editor coroutines logging lines from a selected text file at one-second intervals with EditorWaitForSeconds. Requires the Editor Coroutines package. | MIT |
| [Editor/FieldManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/FieldManager.cs) | Saves and restores public field values and Unity asset references from a configuration file, using reflection and asset GUIDs; demonstrates legacy BinaryFormatter storage. | MIT |
| [Editor/GenerateIsolines.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/GenerateIsolines.cs)<br>[Editor/GenerateIsolines.shader](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/GenerateIsolines.shader) | Editor contour-map generator: the script previews and exports an input texture's major/minor contour lines; GenerateIsolines.shader computes the gradient-based contour mask and inversion. | MIT |
| [Editor/GenerateScripts.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/GenerateScripts.cs) | Exports component class skeletons and public-field declarations from a selected hierarchy to C# files in StreamingAssets, using reflection. | MIT |
| [Editor/GetTexturePathsFromMaterials.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/GetTexturePathsFromMaterials.cs) | Scans texture properties of selected materials and writes their referenced asset paths to a text report. | MIT |
| [Editor/LockInspector.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/LockInspector.cs) | Uses editor reflection to find the GameObject held by a locked Inspector and prints its name. | MIT |
| [Editor/MaterialInspector.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/MaterialInspector.cs) | Custom ShaderGUI drawing albedo/normal/metallic controls and a conditional slider. A compatible example surface shader is embedded in a comment and must be saved separately to use it. | MIT |
| [Editor/PrintScreen.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/PrintScreen.cs) | Captures a screenshot and asks the Windows shell to print the resulting image through ShellExecute. | MIT |
| [Editor/PropertyValidator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/PropertyValidator.cs) | Scans serialized component properties in a hierarchy for missing object references and exports an HTML report with hierarchy paths and property names. | MIT |
| [Editor/ResolutionManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/ResolutionManager.cs) | Adds, selects and removes custom Game View resolutions using Unity editor internal types through reflection. | MIT |
| [Editor/Scatterplot.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/Scatterplot.cs) | Generates a top-down XZ scatterplot of scene renderer-bound centers and exports it as PNG; creates its plotting shader from an embedded source string. | MIT |
| [Editor/ScriptableObjectsManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/ScriptableObjectsManager.cs) | Editor window creating, loading, editing and saving a sample CustomSphere ScriptableObject asset. | MIT |
| [Editor/Spreadsheet.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/Spreadsheet.cs) | Simple editable text-cell grid inside an EditorWindow, with alignment, text-color and cell-width controls; not an Excel file importer. | MIT |
| [Editor/TextureArrayGenerator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/TextureArrayGenerator.cs) | Builds a Texture2DArray asset from selected textures, resizing/copying slices and supplying mip levels. | MIT |
| [Editor/TextureChannelEditor.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/TextureChannelEditor.cs) | Copies a selected RGBA channel into another channel, optionally clears the remaining channels, and exports the resulting texture as PNG. | MIT |
| [Editor/TextureImport.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/TextureImport.cs) | Batch-updates texture compression/crunch settings with progress reporting, with an option to retain uncompressed texture formats. | MIT |
| [Editor/VertexAmbientOcclusion.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/VertexAmbientOcclusion.cs)<br>[Editor/VertexAmbientOcclusion.shader](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Editor/VertexAmbientOcclusion.shader) | Bakes ambient occlusion into vertex-color alpha using sampled physics linecasts, with optional smoothing; VertexAmbientOcclusion.shader displays the baked result, and the window can save the mesh as an asset. | MIT |
| [GenerateReports.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/GenerateReports.cs) | Editor window exporting an HTML report of shaders and their global keywords using ShaderUtil reflection. | MIT |
| [MaskFieldExample.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MaskFieldExample.cs) | Generic enum-mask attribute and Inspector drawer, with a demonstration converting selected flags into their names. Runtime and editor types share one file. | MIT |
| [ToolbarManager.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/ToolbarManager.cs) | Adds custom buttons/actions to the Unity editor toolbar using reflection and UIElements hooks; includes an example build-action button. | MIT |

## Diagnostics and profiling

| Files | Short description | License |
| --- | --- | --- |
| [error.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/error.cs) | Logs SystemInfo properties through reflection and deliberately raises a sample error for testing log output. | MIT |
| [MemoryProfiler.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MemoryProfiler.cs) | Records loaded Unity objects of a chosen type with their runtime memory sizes and compares two text snapshots to find newly present entries. | MIT |
| [UnityProfiler.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/UnityProfiler.cs) | Starts/stops Unity binary profiler logging to log.raw in persistentDataPath; source recommends a Development Build. | MIT |

## Networking and services

| Files | Short description | License |
| --- | --- | --- |
| [basic_network.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/basic_network.cs) | Legacy UNet player-movement example with queued input, predicted client state and server reconciliation. Uses NetworkBehaviour, NetworkIdentity and NetworkManager. | MIT |
| [download.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/download.cs) | Downloads a webpage with UnityWebRequest and extracts matching HTML fragments using a regular expression. | MIT |
| [edwi1.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/edwi1.cs) | Legacy webpage downloader saving HTML/text and linked images under a fixed local directory; uses WWW, regex extraction and Windows-1250 decoding. | MIT |
| [edwi2.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/edwi2.cs) | Legacy webpage text-analysis variant downloading and stripping HTML, counting/filtering words and exporting sorted results to a fixed local directory. | MIT |
| [NetworkClient.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NetworkClient.cs)<br>[NetworkServer.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NetworkServer.cs) | Paired TCP client/server image-exchange demo: server generates a red texture, client generates a blue texture, and background reader threads receive encoded image data for display. | MIT |
| [SVN.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/SVN.cs) | Runs Subversion commands as external processes and parses their output to obtain update/revision information; requires an installed/configured SVN client. | MIT |
| [Translator.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Translator.cs) | Requests text translation from a Google Translate HTTP endpoint and parses the returned text. Requires internet access; uses a service endpoint rather than an included translation model. | MIT |
| [UnityEnjin.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/UnityEnjin.cs) | Historical Enjin GraphQL example obtaining an application token and querying wallet data through async UnityWebRequest. Uses a hard-coded Kovan endpoint and requires service credentials. | MIT |
| [UnityJira.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/UnityJira.cs) | Jira REST integration querying project/issue metadata and creating issues, including attaching a Unity log file. Requires a Jira instance and configured credentials/API token. | MIT |
| [UnityOpenAI.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/UnityOpenAI.cs) | Historical text-completion UI calling /v1/completions with an API key and async UnityWebRequest; the source defaults to text-davinci-003. Treat the endpoint/model configuration as part of this historical example. | MIT |
| [UnitySQL.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/UnitySQL.cs) | Connects to Microsoft SQL Server through System.Data.SqlClient and lists database names; requires a reachable server, credentials and a compatible .NET configuration. | MIT |

## Platform and native integration

| Files | Short description | License |
| --- | --- | --- |
| [Android.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Android.cs) | Queries available and total Android memory through Unity's Java bridge and Android ActivityManager. | MIT |
| [AVX.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/AVX.cs) | Calls a native SIMD texture generator and uploads its RGBA output to a Unity texture. Includes native C source in a comment; compile the DLL separately for compatible x86 hardware. | MIT |
| [cdrom.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/cdrom.cs) | Windows MCI example opening and closing the optical-drive tray through winmm.dll. | MIT |
| [CUDA.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/CUDA.cs) | Invokes the NVIDIA CUDA Driver API directly from C#, loads an embedded PTX kernel and displays its animated procedural mountain texture. Requires compatible NVIDIA hardware/driver. | MIT |
| [Display.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Display.cs) | Enumerates monitor layouts and moves the application window to a selected display asynchronously; source specifies Unity 2021.2 or later. | MIT |
| [KeyboardLayout.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/KeyboardLayout.cs) | Reads the active Windows keyboard-layout identifier through user32.dll and maps it to a descriptive language/layout name. | MIT |
| [Library.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Library.cs) | Dynamically loads/unloads a Windows native DLL, resolves an exported byte-sorting function and invokes it through a delegate. Includes the C++ function source in a comment. | MIT |
| [LoadFont.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/LoadFont.cs) | Looks up font family names from files/Windows font metadata and demonstrates creating Unity dynamic fonts; includes several alternative helper approaches. | MIT |
| [MouseOSX.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/MouseOSX.cs) | macOS cursor-position read/write helpers using CoreGraphics, including a circular cursor-motion demonstration. | MIT |
| [NativeCombineString.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/NativeCombineString.cs) | Passes strings to a native concatenation function and marshals the returned pointer into a managed string. Includes the C++ DLL source in a comment. | MIT |
| [OpenCL.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/OpenCL.cs) | Direct OpenCL API bindings and small resource wrappers executing an embedded image-generation kernel and uploading its output to a Unity texture. Requires an OpenCL runtime/device. | MIT |
| [Pointers.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/Pointers.cs) | Unsafe C# examples reading and modifying floats, integers and structs through pointers, including explicit unmanaged allocation/free. | MIT |
| [register.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/register.cs) | Windows registry example creating a per-user Software/Game key and writing a Fullscreen setting. | MIT |
| [VirtualDisk.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/VirtualDisk.cs) | Windows Virtual Disk API bindings and an example creating, opening, attaching and releasing a small virtual-disk image through native handles. | MIT |
| [WindowsForms.cs](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/WindowsForms.cs) | Small wrapper around the Windows user32 MessageBox function; despite the filename, it does not build a WinForms interface. | MIT |

## Notebook and reference material

| Files | Short description | License |
| --- | --- | --- |
| [notepad.txt](https://github.com/przemyslawzaworski/Unity3D-C-programming/blob/master/notepad.txt) | Mixed Unity/C# notebook containing independent snippets for delegates, editor tools, terrain utilities, component/reflection helpers, mesh combination, Steamworks networking and mathematical/vector helpers. Extract individual snippets before use; it is not one compilable script. | MIT |

## Maintainer notes from source inspection

- Add a standalone MIT LICENSE file to make the owner's declaration discoverable from the repository itself; this catalog does not modify the source repository.
- Keep the original file paths when adding this index. Its topic sections provide navigation without requiring file moves that could change Unity assembly boundaries or break external links.
- StaticBatching.cs refers to MaterialNameComparer1 in its mesh-combiner path, but only MaterialNameComparer is defined in this snapshot. That path needs a small source repair before it can compile.
- Native helper comments, fixed filesystem paths, sample credentials/endpoints and inspector-assigned scene objects are part of the examples' setup. Native DLLs, audio banks and most input assets are not included.
- A future cleanup can isolate alternative examples in namespaces/assemblies and split mixed editor/runtime files. This review produces documentation only; it leaves implementation and repository layout unchanged.
