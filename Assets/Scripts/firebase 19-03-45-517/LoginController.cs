using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField pinInput;
    public TMP_Text statusText;

    public string profileSceneName = "Profile";
    public string menuSceneName = "menuScene";

    void Start()
    {
        if (AuthService.Instance == null || !AuthService.Instance.IsReady)
            SetStatus("Initializing...");
        else
            SetStatus("Enter username + numeric PIN.");
    }

    public async void OnClickRegister()
    {
        string u = usernameInput.text;
        string p = pinInput.text;

        if (AuthService.Instance == null) { SetStatus("AuthService missing."); return; }

        var result = await AuthService.Instance.Register(u, p);
        SetStatus(result.msg);

        if (result.ok)
            SceneManager.LoadScene(profileSceneName);
    }

    public async void OnClickLogin()
    {
        string u = usernameInput.text;
        string p = pinInput.text;

        if (AuthService.Instance == null) { SetStatus("AuthService missing."); return; }

        var result = await AuthService.Instance.Login(u, p);
        SetStatus(result.msg);

        if (result.ok)
            SceneManager.LoadScene(profileSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log("[Login] " + msg);
    }
}