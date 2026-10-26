using System.Collections;
using UnityEngine;

public class Snowman : MonoBehaviour {
    public GameObject boy;         
    // animation time for rescue
    public float revealDuration = 1.2f;    
    
    // player auido when a rescue success
    public AudioClip rescueClip;         
    [Range(0f, 1f)]
    public float rescueVolume = 1f;       

    private bool isTriggered = false;      
    private PlayerFuel playerFuel;        

    void Start() {
        // hide boy at first
        boy.transform.localScale = Vector3.zero;
    }

    void OnTriggerEnter(Collider other) {
        // only triggering once
        if (isTriggered) return;

        // find PlayerFuel from player object
        playerFuel = other.GetComponentInParent<PlayerFuel>();

        // stop if no player found
        if (playerFuel == null) return;

        isTriggered = true;

        // start rescue animation
        StartCoroutine(RevealBoy());
    }

    IEnumerator RevealBoy() {
        float time = 0f;

        // the original scales
        Vector3 snowmanStartScale = transform.localScale;
        Vector3 boyTargetScale = boy.transform.localScale;

        // hide boy first
        boy.transform.localScale = Vector3.zero;

        // Animation: snowman disappears, boy appears
        while (time < revealDuration) {
            time += Time.deltaTime;
            float t = time / revealDuration;

            // Snowman becomes smaller
            transform.localScale =
                Vector3.Lerp(snowmanStartScale, Vector3.zero, t);

            // Boy becomes bigger
            boy.transform.localScale =
                Vector3.Lerp(Vector3.zero, boyTargetScale, t);

            yield return null;
        }

        // set final scale
        boy.transform.localScale = boyTargetScale;
        
        PlayRescueSound();
        
        playerFuel.AddLife(1);

        // add rescue score
        ScoreController.Instance?.AddRescue();

        // hide snowman
        gameObject.SetActive(false);
    }

    void PlayRescueSound()
    {
        GameObject temp = new GameObject("RescueSound");

        // play sound at snowman position
        temp.transform.position = transform.position;

        AudioSource audioSource = temp.AddComponent<AudioSource>();
        audioSource.clip = rescueClip;
        audioSource.volume = rescueVolume;
        audioSource.spatialBlend = 0f; 
        audioSource.Play();

        // Ddestroy object after 4 seconds
        Destroy(temp, 4f);
    }
}