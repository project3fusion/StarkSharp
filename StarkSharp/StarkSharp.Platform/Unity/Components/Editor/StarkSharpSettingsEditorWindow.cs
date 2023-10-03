using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;


namespace StarkSharp.Unity.Resources
{

    public class StarkSharpSettingsEditorWindow : EditorWindow
    {
        private int connectorType = 0;
        private string[] connectorOptions = { "API", "WebGL", "Web Socket" };
        private bool continousCheck = false;
        private float continousCheckInterval = 0.5f;
        private string apiURL, webSocketURL, webSocketWebsiteDomain;
        private bool showApiKey, debugging;
        private Texture2D imageTexture, backgroundTexture;
        private Vector2 imageSize = new Vector2(0, 100), backgroundImageSize = new Vector2(0, 100);

        [MenuItem("StarkSharp/Settings")]
        public static void ShowWindow()
        {
            // Show an existing window instance. If one doesn't exist, make one.
            var window = GetWindow(typeof(StarkSharpSettingsEditorWindow), false, "StarkSharp Settings");
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

        void OnGUI()
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

            EditorGUILayout.Space(imageSize.y + 5);

            // SDK settings label
            EditorGUILayout.LabelField("SDK Settings", EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space(1);

            //Connector Dropdown
            EditorGUILayout.BeginHorizontal();
            var connectorContent = new GUIContent("Connector", "Connection option that application will use to connect Starknet.");
            EditorGUILayout.LabelField(connectorContent);
            connectorType = EditorGUILayout.Popup(connectorType, connectorOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            switch (connectorType)
            {
                case 0: // Option 1
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("API Key");
                    GUIContent toggleContentAPI = new GUIContent("Show API Key");
                    if (!showApiKey) apiURL = EditorGUILayout.PasswordField(apiURL);
                    else apiURL = EditorGUILayout.TextField(apiURL);
                    EditorGUILayout.EndHorizontal();
                    showApiKey = EditorGUILayout.Toggle(toggleContentAPI, showApiKey);
                    break;
                case 1: // Option 2

                    break;
                case 2: // Option 3
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("URL");
                    webSocketURL = EditorGUILayout.TextField(webSocketURL);

                    EditorGUILayout.LabelField("Domain");
                    webSocketWebsiteDomain = EditorGUILayout.TextField(webSocketWebsiteDomain);
                    EditorGUILayout.EndHorizontal();
                    break;
            }
            EditorGUI.indentLevel--;

            GUIContent toggleContent = new GUIContent("Continous Check", "Listen smart contract changes in given intervals.");
            continousCheck = EditorGUILayout.Toggle(toggleContent, continousCheck);

            EditorGUI.BeginDisabledGroup(!continousCheck);  // Start the disabled group
            GUIContent sliderContent = new GUIContent("Interval (Minutes)", "Slide to adjust the value.");
            continousCheckInterval = EditorGUILayout.Slider(sliderContent, continousCheckInterval, 0.1f, 30f);
            EditorGUILayout.HelpBox("You can continously listen smart contract changes in given intervals.", MessageType.Info);
            EditorGUI.EndDisabledGroup();  // End the disabled group

            GUIContent debuggingContent = new GUIContent("Debugging", "Debug all transactions and queries.");
            debugging = EditorGUILayout.Toggle(debuggingContent, debugging);

            // Start a vertical group to push the button to the bottom
            GUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();  // Pushes the next control(s) to the bottom

                // Start a horizontal group to push the button to the right
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();  // Pushes the next control(s) to the right

                    if (GUILayout.Button("Reset", GUILayout.Width(100)))
                    {
                        apiURL = "";
                        webSocketURL = "";
                        continousCheck = false;
                        continousCheckInterval = 0;
                        debugging = false;
                        CreateScript("","", "", false, 0, false);
                    }

                    if (GUILayout.Button("Save", GUILayout.Width(100)))
                    {
                        if (apiURL == "" && webSocketURL == "" && continousCheck == false && continousCheck == false && debugging == false) DisplayMessage();
                        else CreateScript(apiURL, webSocketURL, webSocketWebsiteDomain, continousCheck, continousCheck ? continousCheckInterval : 0.5f, debugging);
                    }
                }
                GUILayout.EndHorizontal();  // End horizontal group
            }
            GUILayout.EndVertical();  // End vertical group

            EditorGUILayout.Space(3);
        }

        private void CreateScript(string apiURL, string webSocketURL, string webSocketWebsiteDomain, bool continousCheck, float continousCheckInterval, bool transactionDebugging)
        {
            // Ensure directory exists
            string dirPath = Application.dataPath + "/StarkSharp/StarkSharp.Resources/StarkSharp.Unity.Settings/";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string scriptPath = dirPath + "Settings.cs";
            using (StreamWriter sw = File.CreateText(scriptPath))
            {
                sw.WriteLine("namespace StarkSharp.Settings {");
                sw.WriteLine("    public class Settings {");
                sw.WriteLine($"        public static string apiurl = \"{apiURL}\";"); 
                sw.WriteLine($"        public static string webSocketWebsiteDomain = \"{webSocketWebsiteDomain}\";");
                sw.WriteLine($"        public static string webSocketipandport = \"{webSocketURL}\";");
                sw.WriteLine($"        public static bool continousCheck = {continousCheck.ToString().ToLower()};");
                sw.WriteLine($"        public static float continousCheckInterval = {continousCheckInterval.ToString("F2", CultureInfo.InvariantCulture)}f;");
                sw.WriteLine($"        public static bool transactionDebugging = {transactionDebugging.ToString().ToLower()};");
                sw.WriteLine("    }");
                sw.WriteLine("}");
    }

            AssetDatabase.Refresh(); // Ensure the script is recognized by Unity immediately.
            EditorUtility.DisplayDialog("Success", "Settings saved!", "Ok");
        }

        private void DisplayMessage()
        {
            EditorUtility.DisplayDialog("Alert", "You are trying to give empty settings?", "Ok");
        }

    }
}
