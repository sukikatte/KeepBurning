using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFuel : MonoBehaviour {
    
    public float maxFuel = 100f;             
    public float currentFuel = 100f;         
    public float decayPerSecond = 5f;        
    
    // the main flame particles
    public ParticleSystem[] flameParticles;  
    // half scale for extra life fire
    public float extraFireBaseFactor = 0.5f;  
    
    public GameObject extraFirePrefab;      
    public Transform extraFireParent;      
    // first fire position
    public Transform lifeFireAnchor;     
    // distance between fires
    public Vector3 extraFireStep = new Vector3(0f, 0f, -1.2f);
    
    public int extraLives = 0;              
    
    public GameOverController gameOverController;

    private bool isDead = false;          
    private bool isExtinguishing = false;    
    
    private List<GameObject> extraFires = new List<GameObject>();
    private List<ParticleData> allParticles = new List<ParticleData>();

    // the original particle values
    struct ParticleData {
        public ParticleSystem ps;
        public float size;
        public float speed;
        public float lifetime;
        public float emission;
    }
    

    void Start() {
        // save original particle values
        SaveParticleData(flameParticles);

        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        UpdateVisual();
    }

    void Update() {
        if (isDead || isExtinguishing) return;

        // uppdate fuel decreases over time
        currentFuel -= decayPerSecond * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        UpdateVisual();
        
        if (currentFuel <= 0) {
            Die(0.1f);
        }
    }
    

    public void AddFuel(float amount) {
        if (isDead) return;

        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        UpdateVisual();
    }

    public void AddLife(int amount) {
        for (int i = 0; i < amount; i++) {
            extraLives++;

            if (extraFirePrefab == null) continue;

            // create extra fire visual
            GameObject fire = Instantiate(extraFirePrefab, extraFireParent);

            // set size
            if (lifeFireAnchor != null)
                fire.transform.localScale = lifeFireAnchor.localScale * extraFireBaseFactor;

            extraFires.Add(fire);

            // add particles
            ParticleSystem[] ps = fire.GetComponentsInChildren<ParticleSystem>(true);

            SaveParticleData(ps);
        }

        UpdateFirePositions();
        UpdateVisual();
    }

    public void ExtinguishOverTime(float duration, float delay) {
        if (isDead || isExtinguishing) return;

        StartCoroutine(ExtinguishRoutine(duration, delay));
    }

    
    void Die(float delay) {
        // if player has extra life, consume it
        if (extraLives > 0) {
            extraLives--;

            // remove last extra fire
            if (extraFires.Count > 0) {
                Destroy(extraFires[extraFires.Count - 1]);
                extraFires.RemoveAt(extraFires.Count - 1);
            }

            // restart with full fuel
            currentFuel = maxFuel;
            UpdateVisual();

            // resume movement if stopped by water
            PlayerMove move = GetComponent<PlayerMove>();
            if (move != null)
                move.ResumeMove();

            return;
        }
        
        isDead = true;
        StartCoroutine(GameOver(delay));
    }

    IEnumerator GameOver(float delay) {
        yield return new WaitForSeconds(delay);

        if (gameOverController != null) {
            gameOverController.ShowGameOver();
        }
    }

    // Extinguish Effect
    IEnumerator ExtinguishRoutine(float duration, float delay) {
        isExtinguishing = true;

        float startFuel = currentFuel;
        float time = 0f;

        while (time < duration) {
            time += Time.deltaTime;

            float t = time / duration;

            // reduce fuel to 0
            currentFuel = Mathf.Lerp(startFuel, 0f, t);

            UpdateVisual();
            yield return null;
        }

        currentFuel = 0f;
        isExtinguishing = false;

        Die(delay);
    }

    

    void SaveParticleData(ParticleSystem[] particles) {
       
        if (particles == null) return;

        for (int i = 0; i < particles.Length; i++) {
            if (particles[i] == null) continue;

            // get particle settings
            ParticleSystem.MainModule main = particles[i].main;
            ParticleSystem.EmissionModule emission = particles[i].emission;

            // save original values for later scaling
            ParticleData data = new ParticleData();
            data.ps = particles[i];
            data.size = main.startSizeMultiplier;
            data.speed = main.startSpeedMultiplier;
            data.lifetime = main.startLifetimeMultiplier;
            data.emission = emission.rateOverTimeMultiplier;

            allParticles.Add(data);
        }
    }
    
    void UpdateVisual() {
        // current fuel percentage
        float fuelPercent = Mathf.Clamp01(currentFuel / maxFuel);

        for (int i = 0; i < allParticles.Count; i++) {
            if (allParticles[i].ps == null) continue;
            
            ParticleSystem.MainModule main = allParticles[i].ps.main;
            ParticleSystem.EmissionModule emission = allParticles[i].ps.emission;
            
            // Scale flame by current fuel percentage
            main.startSizeMultiplier = allParticles[i].size * fuelPercent;
            main.startSpeedMultiplier = allParticles[i].speed * fuelPercent;
            main.startLifetimeMultiplier = allParticles[i].lifetime * fuelPercent;
            emission.rateOverTimeMultiplier = allParticles[i].emission * fuelPercent;

            // clear particles when fuel is empty
            if (fuelPercent <= 0.001f) {
                allParticles[i].ps.Clear();
            }
        }
    }

    void UpdateFirePositions() {
        if (lifeFireAnchor == null) return;

        for (int i = 0; i < extraFires.Count; i++) {
            if (extraFires[i] == null) continue;

            // put extra fires behind player
            extraFires[i].transform.localPosition = lifeFireAnchor.localPosition + extraFireStep * (i + 1);
        }
    }
}