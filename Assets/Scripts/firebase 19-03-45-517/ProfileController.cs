using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfileController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject guestPanel;
    public GameObject userPanel;

    [Header("Guest UI")]
    public TMP_Text guestHintText; // “登录以存储数据、加入我们的排行榜！”

    [Header("User UI")]
    public TMP_Text usernameText;
    public TMP_Text totalRescuedText;
    public TMP_Text bestRescuedText;

    [Header("Scene names")]
    public string loginSceneName = "Login";
    public string menuSceneName = "menuScene";

    async void OnEnable()
    {
        await RefreshView();
    }

    public async System.Threading.Tasks.Task RefreshView()
    {
        if (AuthService.Instance == null || !AuthService.Instance.IsReady)
        {
            SetGuest("Service loading...");
            return;
        }

        if (!AuthService.Instance.IsLoggedIn)
        {
            SetGuest("Sign in to save your data and join the leaderboard!");
            return;
        }

        Dictionary<string, object> profile = await AuthService.Instance.GetMyProfile();
        if (profile == null)
        {
            SetGuest("Profile not found, please re-login.");
            return;
        }

        guestPanel.SetActive(false);
        userPanel.SetActive(true);

        usernameText.text = "Username: " + (profile.ContainsKey("username") ? profile["username"].ToString() : "-");
        totalRescuedText.text = "Total Rescued: " + (profile.ContainsKey("totalRescued") ? profile["totalRescued"].ToString() : "0");
        bestRescuedText.text = "Best Single Run: " + (profile.ContainsKey("bestSingleRunRescued") ? profile["bestSingleRunRescued"].ToString() : "0");
    }

    void SetGuest(string msg)
    {
        guestPanel.SetActive(true);
        userPanel.SetActive(false);
        if (guestHintText != null) guestHintText.text = msg;
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene(loginSceneName);
    }

    public async void Logout()
    {
        if (AuthService.Instance != null) AuthService.Instance.Logout();
        await RefreshView();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}