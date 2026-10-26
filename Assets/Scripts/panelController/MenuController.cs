using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public GameObject bufferPanel;

    void Start() {
        // hide bufferPanel at first
        if (bufferPanel != null) {
            bufferPanel.SetActive(false);
        }
    }

    // show bufferPanel before gamePanel
    public void StartGame() {
        if (bufferPanel != null) {
            bufferPanel.SetActive(true);
        }
        else {
            // if panel is not assigned, start game directly
            LoadGameplayScene();
        }
    }

    // hide bufferPanel
    public void ClosebufferPanel() {
        if (bufferPanel != null) {
            bufferPanel.SetActive(false);
        }
    }

    // load gameScene
    public void LoadGameplayScene() {
        SceneManager.LoadScene("mainScene");
    }

    // load leaderboardScene
    public void OpenLeaderboard() {
        SceneManager.LoadScene("Leaderboard");
    }

    // load profileScene
    public void OpenProfile() {
        SceneManager.LoadScene("Profile");
    }

    // back to menuScene
    public void BackToMenu() {
        // reset time in case game was paused
        Time.timeScale = 1f;
        SceneManager.LoadScene("menuScene");
    }

    // for button in other scenes
    public void BackToMain() {
        BackToMenu();
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // buy extra life using points
    public bool BuyExtraLifeBuff()
    {
        // not enough points
        if (LocalProgress.TotalPoints < 5)
        {
            return false;
        }

        LocalProgress.TotalPoints -= 5;

        // give extra life buff
        LocalProgress.HasPendingExtraLife = true;

        return true;
    }
}