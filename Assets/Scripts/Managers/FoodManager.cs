using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject[] foods;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;
    private Vector3 target = new Vector3(0f, 1f, 0f);
    private Random random;
    public float speed;
    void Start ()
    {
        random = new Random();
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
        
       
    }


    void Spawn ()
    {
        if(playerHealth.currentHealth <= 0f)
        {
            return;
        }

        int spawnPointIndex = Random.Range (0, spawnPoints.Length);
        int foodIndex = Random.Range(0, foods.Length);
        Vector3 init = spawnPoints[spawnPointIndex].position;
        GameObject clone = (GameObject)Instantiate(foods[foodIndex], init, spawnPoints[spawnPointIndex].rotation);
        Vector3 noisyTarget = target + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        Vector3 finalNoisyTarget = noisyTarget;
        Vector3 Vi = (noisyTarget - init).normalized;

        /*float den = 0.0f;
        float num = 0.0f;
        if (init.x < 1.0f)
        {
            Vi = new Vector3(Vi.z, Vi.y, Vi.x);
            init = new Vector3(init.z, init.y, init.x);
            noisyTarget = new Vector3(noisyTarget.z, noisyTarget.y, noisyTarget.x);
        }
        
        den = 2 * Mathf.Pow(Vi.x, 2) * ((Vi.y / Vi.x) * (noisyTarget.x - init.x) + init.y - noisyTarget.y);
        num = 9.81f * Mathf.Pow((noisyTarget.x-init.x),2.0f);
        float speed = Mathf.Sqrt(num / den);
        Debug.Log(speed);
        Debug.Log(den);
        Debug.Log(num);*/
        Vector3 tran = (finalNoisyTarget - spawnPoints[spawnPointIndex].position).normalized * speed;

        clone.GetComponent<Rigidbody>().velocity = new Vector3(tran.x, 0.0f, tran.z);
    }
}
