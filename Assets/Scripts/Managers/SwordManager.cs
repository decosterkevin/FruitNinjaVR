using UnityEngine;
using System.Collections;

public class SwordManager : MonoBehaviour {
    GameObject player;
    PlayerHealth playerHealth;
    public int attackDamage = 10;
    public int scoreValue = 10;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bomb")
        {
            playerHealth.TakeDamage(attackDamage);
            //Destroy(other.gameObject);
        }
        else if(other.gameObject.tag == "Food") 
        {
            ScoreManager.score += scoreValue;
            //Destroy(other.gameObject);
        }
        if (playerHealth.currentHealth <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
