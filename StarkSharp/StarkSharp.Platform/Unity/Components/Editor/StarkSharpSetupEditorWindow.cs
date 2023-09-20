using StarkSharp.Fusion.Sharpion.Unity;
using UnityEditor;
using UnityEngine;


public class StarkSharpSetupEditorWindow : EditorWindow
{
    private int selectedOption = 0;
    private Texture2D imageTexture, backgroundTexture;
    private Vector2 imageSize = new Vector2(0, 100), backgroundImageSize = new Vector2(0, 100);

    [MenuItem("StarkSharp/Setup")]
    public static void ShowWindow()
    {
        // Show an existing window instance. If one doesn't exist, make one.
        var window = GetWindow(typeof(StarkSharpSetupEditorWindow), false, "StarkSharp Setup");
        // Set initial min size and max size of the window.
        window.minSize = new Vector2(500, 370);
        window.maxSize = new Vector2(500, 370);
    }

    private void OnEnable()
    {
        //Get textures
        imageTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/StarkSharp/StarkSharp.Resources/Image/StarkSharpSdkLogo.png");
        backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/StarkSharp/StarkSharp.Resources/Image/StarkSharpSdkBackground.png");
    }

    private void OnGUI()
    {
        if (backgroundTexture != null)
        {
            // Calculate width / height based on window's width / height.
            backgroundImageSize.x = position.width;
            backgroundImageSize.y = position.height;

            // Draw the image with the specified dimensions.
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, backgroundImageSize.x, backgroundImageSize.y), backgroundTexture);
        }

        if (imageTexture != null)
        {
            // Calculate width / height based on window's width / height.
            imageSize.x = position.width;
            imageSize.y = position.height * 40 / 100;

            // Draw the image with the specified dimensions.
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, imageSize.x, imageSize.y), imageTexture);
        }

        EditorGUILayout.Space(imageSize.y + 20);

        var sceneSetupContent = new GUIContent("Select Connector Type", "Connection option that application will use to connect Starknet.");
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        centeredStyle.fontStyle = FontStyle.Bold;
        centeredStyle.fontSize = 14;
        EditorGUILayout.LabelField(sceneSetupContent, centeredStyle);

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        bool apiSelected = EditorGUILayout.Toggle("API", selectedOption == 0);
        if (apiSelected)
            selectedOption = 0;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        bool webglSelected = EditorGUILayout.Toggle("WebGL", selectedOption == 1);
        if (webglSelected)
            selectedOption = 1;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        bool webSocketSelected = EditorGUILayout.Toggle("Web Socket", selectedOption == 2);
        if (webSocketSelected)
            selectedOption = 2;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label(GetSelectedOption());

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Scene Setup", GUILayout.Width(125)))
        {
            if (webSocketSelected) AddGameObjectWithWebSocketDependency();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

    }

    string GetSelectedOption()
    {
        switch (selectedOption)
        {
            case 0: return " - ";
            case 1: return " - ";
            case 2: return "Add Socket to scene.";
            default: return "Unknown";
        }
    }

    public void AddGameObjectWithWebSocketDependency()
    {
        GameObject existingObject = GameObject.Find("Socket");

        if (existingObject == null)
        {
            GameObject newGameObject = new GameObject("Socket");

            newGameObject.AddComponent<Socket>();

            Selection.activeGameObject = newGameObject;
        }
        else EditorUtility.DisplayDialog("Alert", "There is already a Socket in your scene.", "Ok");
    }
}
