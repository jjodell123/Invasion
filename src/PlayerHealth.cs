using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private float startHealth = 100f;
    [SerializeField]
    private Slider slider;

    private float currentHealth;
    private bool playerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
        slider.maxValue = startHealth;
        slider.value = currentHealth;
        playerDead = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        slider.value = currentHealth;

        if (currentHealth <= 0 && !playerDead)
        {
            PlayerDies();
        }
    }

    public void Heal(float healAmount)
    {
        if (!playerDead)
        {
            currentHealth += healAmount;
            slider.value = currentHealth;

            if (currentHealth > startHealth)
            {
                currentHealth = startHealth;
            }
        }
    }

    public bool GetPlayerDead()
    {
        return playerDead;
    }

    private void PlayerDies()
    {
        FindObjectOfType<LevelManager>().LevelLost();
        playerDead = true;
    }
}
