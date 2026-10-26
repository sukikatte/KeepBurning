using System.Collections;
using UnityEngine;

public class Crate_Box : MonoBehaviour {
    // crate shrinking time
    public float shrinkDuration = 0.5f;
    
    // when player hits crate, play audio
    public AudioClip hitClip;           
    [Range(0f, 1f)]
    // Sound volume
    public float hitVolume = 1f;         

    private bool isTriggered = false;    
    private Vector3 startScale;          
    private PlayerMove playerMove;        

    void Start() {
        // the original scales
        startScale = transform.localScale;
    }

    void OnTriggerEnter(Collider other) {
        // only triggering once
        if (isTriggered) return;

        // find PlayerMove from player object
        playerMove = other.GetComponentInParent<PlayerMove>();

        // stop if no player found
        if (playerMove == null) return;

        isTriggered = true;

        // Start crate clear process
        StartCoroutine(ClearCrate());
    }

    IEnumerator ClearCrate() {
        // stop player movement
        playerMove.StopImmediately();

        float waitTime = 0f;

        // Play hit sound
        if (hitClip != null) {
            GameObject temp = new GameObject("CrateSound");
            temp.transform.position = transform.position;

            AudioSource audioSource = temp.AddComponent<AudioSource>();
            audioSource.clip = hitClip;
            audioSource.volume = hitVolume;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
            
            // wait until the sound finished
            waitTime = hitClip.length;

            Destroy(temp, waitTime);
        }

        // crate shrinking animation
        float time = 0f;

        while (time < shrinkDuration) {
            time += Time.deltaTime;

            float t = time / shrinkDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

       
        Destroy(gameObject);

        // let player move again
        if (playerMove != null) {
            playerMove.ResumeMove();
        }
    }
    
}