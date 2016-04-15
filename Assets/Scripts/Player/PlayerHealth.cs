using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    //public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    SwordManager sword;
    FirstPersonController controller;
    Animator anim;
    AudioSource playerAudio;

    bool isDead;
    bool damaged;


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        sword = GetComponent<SwordManager>();
        controller = GetComponent<FirstPersonController>();
        currentHealth = startingHealth;
    }


    void Update ()
    {
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;
        

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }
    

    void Death ()
    {
        isDead = true;


        //playerAudio.clip = deathClip;
        //playerAudio.Play ();
        controller.enabled = false;
        sword.enabled = false;
    }
    
}
