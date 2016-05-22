
using System.Collections;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject[] foods;
    public float spawnTime = 3f;
    public float destroyTime = 4;
    public float randomOffset = 0.3f;
    public GameObject[] spawnPoints;
    public GameObject HUDCanvas;
    public WiimoteScript wiimoteScript;
    private Vector3 target = new Vector3(0f, 1f, 0f);
    private Random random;


    private AudioSource[] cannon;
    private AudioSource[] fuze;
    private ParticleSystem[] explosions;
    //Sound constante
    private float initialSpawnTime;
    private static float fuzeSoundOffset = 0.1f;
    //constante for the decrease asymptotic spawntime function
    private static float minSpawnTime = fuzeSoundOffset*2.0f;
    private static float rateSpawnFunction = 0.008f;
    private static float inflectionSpawnFunction = 150f;
    private int currentFiringTowerIndex = -1;
    
    private bool soundPlaying;
    public float speed;

    private GameObject healthBar;
    private GameObject score;
    private GameObject introText;
    void Start() {
        initialSpawnTime = spawnTime;
        random = new Random();
        soundPlaying = false;
        cannon = new AudioSource[spawnPoints.Length];
        
        fuze = new AudioSource[spawnPoints.Length];
        explosions = new ParticleSystem[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            cannon[i] = spawnPoints[i].GetComponents<AudioSource>()[0];
            
            fuze[i] = spawnPoints[i].GetComponents<AudioSource>()[1];
            explosions[i] = spawnPoints[i].GetComponent<ParticleSystem>();
        }

        healthBar = HUDCanvas.transform.Find("HealthUI").gameObject;
        score = HUDCanvas.transform.Find("ScoreText").gameObject;
        healthBar.SetActive(false);
        score.SetActive(false);
        introText= HUDCanvas.transform.Find("IntroText").gameObject;
        //Hide score and healthBar
        StartCoroutine(WaitInput());


    }
    private IEnumerator WaitInput()
    {
        bool wait = true;
        while (wait)
        {
            if (wiimoteScript.getWiimote().Button.one)
            {

                //hide introText, display score and healthbar
                introText.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
                healthBar.SetActive(true);
                score.SetActive(true);
                InvokeRepeating("Spawn", spawnTime + fuzeSoundOffset, spawnTime+ fuzeSoundOffset);
                wait = false;
            }
            yield return null;
        }
    }


    public int getCurrentFiringTowerIndex()
    {
        return currentFiringTowerIndex;
    }

    void Spawn()
    {
        if (playerHealth.currentHealth <= 0f)
        {
            return;
        }
        spawnTime = initialSpawnTime -(initialSpawnTime - minSpawnTime) * Mathf.Exp(-inflectionSpawnFunction* Mathf.Exp(-rateSpawnFunction* ScoreManager.score));
        
        StartCoroutine(PlaySound());
        
        
    }
    IEnumerator PlaySound()
    {

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        currentFiringTowerIndex = spawnPointIndex;
        int foodIndex = Random.Range(0, foods.Length);
        
        soundPlaying = true;
        fuze[spawnPointIndex].time = fuzeSoundOffset;
        fuze[spawnPointIndex].Play();
        fuze[spawnPointIndex].SetScheduledEndTime( AudioSettings.dspTime +fuzeSoundOffset + spawnTime);
        yield return new WaitForSeconds(spawnTime+ fuzeSoundOffset);

        Vector3 init = spawnPoints[spawnPointIndex].transform.position;
        GameObject clone = (GameObject)Instantiate(foods[foodIndex], init, spawnPoints[spawnPointIndex].transform.rotation);
		Destroy (clone, destroyTime);


        Vector3 noisyTarget = target + new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset), 
            Random.Range(-randomOffset, randomOffset));
        Vector3 finalNoisyTarget = noisyTarget;
        Vector3 Vi = (noisyTarget - init).normalized;

        Vector3 tran = (finalNoisyTarget - spawnPoints[spawnPointIndex].transform.position).normalized * speed;
        soundPlaying = false;
        clone.GetComponent<Rigidbody>().velocity = new Vector3(tran.x, 0.0f, tran.z);
        cannon[spawnPointIndex].Play();
        explosions[spawnPointIndex].Play();
    }

}