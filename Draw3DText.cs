using UnityEngine;

public class Draw3DText : MonoBehaviour
{
    public float Speed = 1.0f;
    TextClass[] TextClasses;
    Vector3[] Positions;

    void Start()
    {
        TextClasses = new TextClass[1000];
        Positions = new Vector3[TextClasses.Length];
        for (int i = 0; i < TextClasses.Length; i++)
        {
            Positions[i] = Random.insideUnitSphere * 5f;
            TextClasses[i] = Create3DText("Start", Positions[i], Quaternion.identity, 1.0f);
        }
    }

    void Update()
    {
        if (TextClasses == null) return;
        float radian = Time.time * Mathf.Deg2Rad * Speed;
        Vector3 offset = new Vector3(0f, Mathf.Cos(radian), Mathf.Sin(radian));
        for (int i = 0; i < TextClasses.Length; i++)
        {
            Vector3 position = Positions[i] + offset;
            Update3DText(TextClasses[i], position.ToString("F2"), position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            for (int i = 0; i < TextClasses.Length; i++)
            {
                Destroy3DText(TextClasses[i]);
            }
            TextClasses = null;
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////

    public class TextClass
    {
        public GameObject instance;
        public TMPro.TextMeshPro textMeshPro;
        public RectTransform rectTransform;
    }

    TextClass Create3DText(string text, Vector3 worldPos, Quaternion rotation, float fontSize = 1.0f)
    {
        GameObject widget = new GameObject("Widget");
        widget.hideFlags = HideFlags.HideInHierarchy;
        RectTransform rectTransform = widget.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.position = worldPos;
        rectTransform.sizeDelta = new Vector2(0f, 0f);
        MeshRenderer meshRenderer = widget.AddComponent<MeshRenderer>();
        TMPro.TextMeshPro textMeshPro = widget.AddComponent<TMPro.TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.enableWordWrapping = false;
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = Color.black;
        textMeshPro.fontStyle = TMPro.FontStyles.Bold;
        textMeshPro.alignment = TMPro.TextAlignmentOptions.Center;
        TextClass textClass = new TextClass();
        textClass.instance = widget;
        textClass.textMeshPro = textMeshPro;
        textClass.rectTransform = rectTransform;
        return textClass;
    }

    void Update3DText(TextClass textClass, string text, Vector3 worldPos, Quaternion rotation)
    {
        textClass.textMeshPro.text = text;
        textClass.rectTransform.position = worldPos;
        textClass.rectTransform.rotation = rotation;
    }
    
    void Destroy3DText(TextClass textClass)
    {
        Destroy(textClass.instance);
        textClass = null;
    }

//////////////////////////////////////////////////////////////////////////////////////////////////
   
    #if UNITY_EDITOR
    // Enable Dynamic Batching manually or use:
    // SetBatchingForPlatform(UnityEditor.BuildTarget.StandaloneWindows64, 1, 1);
    void SetBatchingForPlatform(UnityEditor.BuildTarget platform, int staticBatching, int dynamicBatching)
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
        System.Reflection.MethodInfo method = typeof(UnityEditor.PlayerSettings).GetMethod("SetBatchingForPlatform", flags);       
        object[] args = new object[]{platform, staticBatching,dynamicBatching};
        method.Invoke(null, args);
    }    
    #endif    
}