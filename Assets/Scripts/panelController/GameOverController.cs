using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject gameOverPanel;
    public PlayerMove playerMove;

    void Start() {
        // hide gameOverPanel at first
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOver() {
        // finalize score when game ends
        if (ScoreController.Instance != null)
            ScoreController.Instance.FinalizeRun();

        // show gameOverPanel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // disable player movement
        if (playerMove != null)
            playerMove.enabled = false;
    }

    public void RestartGame() {
        // save score 
        if (ScoreController.Instance != null)
            ScoreController.Instance.FinalizeRun();

        // load gameScene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu() {
        // save score
        if (ScoreController.Instance != null)
            ScoreController.Instance.FinalizeRun();
        
        SceneManager.LoadScene("menuScene");
    }
}