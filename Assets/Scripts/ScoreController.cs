using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour {
    public static ScoreController Instance;

    public TMP_Text rescuedText;
    public PlayerFuel playerFuel;

    private int rescuedCount = 0;
    private bool finalized = false;

    void Awake() {
        // singleton reference
        Instance = this;
    }

    void Start() {
        // reset points
        rescuedCount = 0;
        finalized = false;

        // update points at first
        if (rescuedText != null)
            rescuedText.text = "Rescued: 0";

        // apply extra life buff if player has one
        if (LocalProgress.HasPendingExtraLife) {
            if (playerFuel != null)
                playerFuel.AddLife(1);

            LocalProgress.ClearPendingExtraLife();
        }
    }

    void OnDestroy() {
        // save score when quit
        FinalizeRun();

        // clear singleton reference
        if (Instance == this)
            Instance = null;
    }
    
    public void AddRescue() {
        rescuedCount++;
        
        // update points text
        if (rescuedText != null)
            rescuedText.text = "Rescued: " + rescuedCount;
    }

    // save points
    public void FinalizeRun() {
        if (finalized)
            return;

        finalized = true;

        // add local points into total points
        LocalProgress.TotalPoints += rescuedCount;
    }

    public int GetRescuedCount() {
        return rescuedCount;
    }
}