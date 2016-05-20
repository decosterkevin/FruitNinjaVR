using UnityEngine;
using System.Collections;

public class SwordManager : MonoBehaviour {
    GameObject player;
    PlayerHealth playerHealth;
    public int attackDamage = 10;
    public int scoreValue = 10;

    public AudioClip hitSound;
    public AudioClip bombSound;

	public GameObject explosion;
	public GameObject pointsGain;

	private GameObject sword;
	public AudioSource audioSource;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();

		sword = GameObject.FindGameObjectWithTag ("Sword");
		audioSource = sword.GetComponent<AudioSource> ();

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bomb")
        {
            playerHealth.TakeDamage(attackDamage);

			GameObject expl = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;

            audioSource.PlayOneShot(bombSound);

			Destroy (expl, 3);
            Destroy(other.gameObject);
        }
        else if(other.gameObject.tag == "Food") 
        {
            ScoreManager.score += scoreValue;

			GameObject effect = Instantiate (pointsGain, transform.position, Quaternion.identity) as GameObject;

			audioSource.PlayOneShot(hitSound);

			Destroy (effect, 3);
            Destroy(other.gameObject);
        }
    }
}
