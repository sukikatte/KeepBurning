using UnityEngine;

public class FireAudio : MonoBehaviour {
    public AudioClip fireClip;         
    public PlayerFuel playerFuel;       

    public float minVolume = 0f;        
    public float maxVolume = 0.7f;    

    private AudioSource audioSource;  

    void Start() {
        // get AudioSource component
        audioSource = GetComponent<AudioSource>();
        
        // play audio
        audioSource.clip = fireClip;
        audioSource.Play();
    }

    void Update() {
        // stop if no playerFuel
        if (playerFuel == null) return;

        // calculate fuel percentage
        float fuelPercent = Mathf.Clamp01(playerFuel.currentFuel / playerFuel.maxFuel);

        // change volume based on fuel amount
        audioSource.volume = Mathf.Lerp(minVolume, maxVolume, fuelPercent);
    }
}