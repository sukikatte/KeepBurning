using UnityEngine;

public class Fuel : MonoBehaviour {
    public float fuelAmount = 50f; 

    void OnTriggerEnter(Collider other) {
        
        PlayerFuel playerFuel = other.GetComponentInParent<PlayerFuel>();

        // check if player entered trigger and add fuel
        if (playerFuel != null) {
            playerFuel.AddFuel(fuelAmount);
            
            Destroy(gameObject);
        }
    }
}