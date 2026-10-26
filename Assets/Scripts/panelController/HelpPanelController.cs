using UnityEngine;

public class HelpPanelController : MonoBehaviour {
    public GameObject helpPanel;
    public GameObject mobileControls;

    void Start() {
        // hide helpPanel at first
        if (helpPanel != null)
            helpPanel.SetActive(false);
    }


    public void OpenHelp() {
        if (helpPanel == null) return;

        helpPanel.SetActive(true);
        // pause game when help panel is opened
        Time.timeScale = 0f;

        // hide mobile controls for android devices
        if (mobileControls != null)
            mobileControls.SetActive(false);
    }

    // close helpPanel, and show mobile controls for android devices
    public void CloseHelp() {
        if (helpPanel == null) return;

        helpPanel.SetActive(false);
        // reset time after back from paused
        Time.timeScale = 1f;

        bool isMobile = Application.platform == RuntimePlatform.Android;

        if (mobileControls != null)
            mobileControls.SetActive(isMobile);
    }
}
