using UnityEngine;

public static class LocalProgress {
    
    private const string KEY_POINTS = "TOTAL_POINTS";
    private const string KEY_EXTRA_LIFE = "BUFF_EXTRA_LIFE";
    
    public static int TotalPoints {
        get {
            return PlayerPrefs.GetInt(KEY_POINTS, 0);
        }
        set {
            int safeValue = Mathf.Max(0, value);
            PlayerPrefs.SetInt(KEY_POINTS, safeValue);
            PlayerPrefs.Save();
        }
    }

    // determine whether player has an extra life buff
    public static bool HasPendingExtraLife {
        get {
            return PlayerPrefs.GetInt(KEY_EXTRA_LIFE, 0) == 1;
        }
        set {
            int saveValue = value ? 1 : 0;

            PlayerPrefs.SetInt(KEY_EXTRA_LIFE, saveValue);
            PlayerPrefs.Save();
        }
    }

    // clear buff after use
    public static void ClearPendingExtraLife() {
        HasPendingExtraLife = false;
    }
}