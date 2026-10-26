using UnityEngine;

public class MobileOnly : MonoBehaviour {
    void Start() {
        bool isMobile = Application.platform == RuntimePlatform.Android;
        
        gameObject.SetActive(isMobile);
    }
}