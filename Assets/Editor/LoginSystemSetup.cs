using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class LoginSystemSetup : EditorWindow
{
    [MenuItem("Tools/Setup Login System")]
    public static void Setup()
    {
        // 1. Setup Scenes
        string loginScenePath = "Assets/Scenes/LoginScene.unity";
        string battleScenePath = "Assets/Scenes/Battle scence.unity"; // User specific name

        EnsureDirectoryExists("Assets/Scenes");

        // --- Create Login Scene ---
        Scene loginScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Setup UI in Login Scene
        SetupLoginUI();
        
        // Save Login Scene
        EditorSceneManager.SaveScene(loginScene, loginScenePath);
        Debug.Log("Created LoginScene at " + loginScenePath);

        // --- Create Battle Scene ---
        Scene battleScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Add a text to identify it
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject textObj = new GameObject("BattleText");
        textObj.transform.SetParent(canvasObj.transform, false);
        Text text = textObj.AddComponent<Text>();
        text.text = "Battle Scene\n(Welcome!)";
        text.font = GetDefaultFont();
        text.fontSize = 50;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        textObj.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 300);

        // Save Battle Scene
        EditorSceneManager.SaveScene(battleScene, battleScenePath);
        Debug.Log("Created Battle scence at " + battleScenePath);

        // --- Add to Build Settings ---
        AddScenesToBuildSettings(new string[] { loginScenePath, battleScenePath });

        // --- Return to Login Scene ---
        EditorSceneManager.OpenScene(loginScenePath);
        
        EditorUtility.DisplayDialog("Setup Complete", 
            "Login System has been set up successfully!\n\n" +
            "1. LoginScene created.\n" +
            "2. 'Battle scence' created.\n" +
            "3. Build Settings updated.\n\n" +
            "You can now press Play to test the login.", "OK");
    }

    static void SetupLoginUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create EventSystem
        GameObject esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Background Panel
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.2f, 1f); // Dark blue-gray
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        Font font = GetDefaultFont();

        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panelGO.transform, false);
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "GAME LOGIN";
        titleText.font = font;
        titleText.fontSize = 40;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchoredPosition = new Vector2(0, 100);
        titleRT.sizeDelta = new Vector2(400, 60);

        // Username Input
        GameObject userGO = CreateInputField("UsernameInput", "Username...", 0, 20, font);
        userGO.transform.SetParent(panelGO.transform, false);

        // Password Input
        GameObject passGO = CreateInputField("PasswordInput", "Password...", 0, -40, font);
        passGO.transform.SetParent(panelGO.transform, false);
        InputField passInput = passGO.GetComponent<InputField>();
        passInput.contentType = InputField.ContentType.Password;

        // Login Button
        GameObject btnGO = new GameObject("LoginButton");
        btnGO.transform.SetParent(panelGO.transform, false);
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 1f); // Light blue
        Button btn = btnGO.AddComponent<Button>();
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(160, 40);
        btnRT.anchoredPosition = new Vector2(0, -110);
        
        GameObject btnTextGO = new GameObject("Text");
        btnTextGO.transform.SetParent(btnGO.transform, false);
        Text btnText = btnTextGO.AddComponent<Text>();
        btnText.text = "LOGIN";
        btnText.font = font;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        btnText.fontStyle = FontStyle.Bold;
        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        // Status Text
        GameObject statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(panelGO.transform, false);
        Text statusText = statusGO.AddComponent<Text>();
        statusText.text = "";
        statusText.font = font;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.fontSize = 14;
        statusText.fontStyle = FontStyle.Italic;
        RectTransform statusRT = statusGO.GetComponent<RectTransform>();
        statusRT.sizeDelta = new Vector2(400, 50);
        statusRT.anchoredPosition = new Vector2(0, -170);

        // Setup Manager
        GameObject managerGO = new GameObject("LoginManager");
        LoginManager manager = managerGO.AddComponent<LoginManager>();
        manager.usernameInput = userGO.GetComponent<InputField>();
        manager.passwordInput = passInput;
        manager.loginButton = btn;
        manager.statusText = statusText;
    }

    static GameObject CreateInputField(string name, string placeholder, float x, float y, Font font)
    {
        GameObject root = new GameObject(name);
        Image img = root.AddComponent<Image>();
        img.color = Color.white;
        InputField input = root.AddComponent<InputField>();
        RectTransform rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(250, 40);
        rt.anchoredPosition = new Vector2(x, y);

        // Text Component
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(root.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.font = font;
        text.color = Color.black;
        text.fontSize = 14;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 0);
        textRT.offsetMax = new Vector2(-10, 0);
        
        // Placeholder
        GameObject phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(root.transform, false);
        Text ph = phGO.AddComponent<Text>();
        ph.text = placeholder;
        ph.font = font;
        ph.fontStyle = FontStyle.Italic;
        ph.fontSize = 14;
        ph.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        RectTransform phRT = phGO.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = new Vector2(10, 0);
        phRT.offsetMax = new Vector2(-10, 0);

        input.textComponent = text;
        input.placeholder = ph;
        input.targetGraphic = img;

        return root;
    }

    static Font GetDefaultFont()
    {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    static void AddScenesToBuildSettings(string[] paths)
    {
        EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
        List<EditorBuildSettingsScene> newSettings = new List<EditorBuildSettingsScene>(original);
        
        foreach (string path in paths)
        {
            if (!newSettings.Exists(s => s.path == path))
            {
                newSettings.Add(new EditorBuildSettingsScene(path, true));
            }
        }
        EditorBuildSettings.scenes = newSettings.ToArray();
    }
}
