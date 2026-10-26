using UnityEngine;

public class Lake : MonoBehaviour {
    public float extinguishDuration = 0.8f; 
    public float gameOverDelay = 0.15f;     

    void OnTriggerEnter(Collider other) {
        
        PlayerMove move = other.GetComponentInParent<PlayerMove>();

        // if in lake, stop immediately
        if (move != null) {
            move.StopImmediately();
        }
        
        PlayerFuel fuel = other.GetComponentInParent<PlayerFuel>();

        // go to ash
        if (fuel != null) {
            fuel.ExtinguishOverTime(extinguishDuration, gameOverDelay);
        }
    }
}