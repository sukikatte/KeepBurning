using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BufferPanelController : MonoBehaviour {
    public MenuController menuController;
    public GameObject panelRoot;
    public TMP_Text pointsText;
    public Button buyAndStartButton;

    void Awake() {
        // hide bufferPanel at first
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    void OnEnable() {
        RefreshUI();
    }

    // open bufferPanel
    public void OpenPanel() {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        RefreshUI();
    }

    // close bufferPanel
    public void ClosePanel() {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    // buy extra life then start game
    public void OnClickBuyAndStart() {
        if (menuController == null)
            return;

        // buy buff
        bool success = menuController.BuyExtraLifeBuff();

        if (!success)
            return;
        
        menuController.LoadGameplayScene();
    }

    // start game without buying
    public void OnClickStartDirectly() {
        if (menuController == null)
            return;

        menuController.LoadGameplayScene();
    }

    // update panel (points + button state)
    public void RefreshUI() {
        int points = LocalProgress.TotalPoints;

        // update points
        if (pointsText != null)
            pointsText.text = "Points: " + points;

        // enable button only if enough points
        if (buyAndStartButton != null)
            buyAndStartButton.interactable = points >= 5;
    }
}