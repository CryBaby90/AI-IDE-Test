using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Text statusText;

    // Target scene name as requested
    private const string BATTLE_SCENE_NAME = "Battle scence";

    void Start()
    {
        // Ensure UI elements are assigned
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }
        
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    public void OnLoginClicked()
    {
        if (usernameInput == null || passwordInput == null)
        {
            Debug.LogError("Input fields are not assigned!");
            return;
        }

        string username = usernameInput.text;
        string password = passwordInput.text;

        // Basic validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            UpdateStatus("请输入用户名和密码 (Please enter username and password)", Color.red);
            return;
        }

        // Mock authentication logic
        // For demo purposes, we accept any username/password longer than 1 character
        if (username.Length > 0 && password.Length > 0)
        {
            UpdateStatus("登录成功 (Login Success)! Loading Battle Scene...", Color.green);
            
            // Delay slightly to show the message, then load scene
            Invoke(nameof(LoadBattleScene), 1.0f);
        }
    }

    private void LoadBattleScene()
    {
        // Check if scene is in build settings is handled by Unity, but we can try-catch just in case
        try
        {
            SceneManager.LoadScene(BATTLE_SCENE_NAME);
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Error loading scene: {e.Message}. Did you add '{BATTLE_SCENE_NAME}' to Build Settings?", Color.red);
            Debug.LogError(e);
        }
    }

    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
        Debug.Log(message);
    }
}
