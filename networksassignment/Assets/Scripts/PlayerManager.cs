using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //player unique identifiers
    public int id;
    public string username;
    public float health;
    public float maxHealth = 100f;
    public MeshRenderer model;
    public int ammoCount = 0;

    //players stats to display
    public Image ammoUI;
    public Text ammoText;
    public Image healthBar;


    public void Update()
    {
        if (ammoText != null)
        {
            ammoText.text = ammoCount.ToString();
        }
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    //health manager
    public void SetHealth(float _health)
    {
        health = _health;

        //set the players healthbar dependant on actual health vs max health
        healthBar.fillAmount = health / maxHealth;

        //when healthh hits 0
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
        healthBar.enabled = false;
       // ammoCount = 0;
    }

    //resets player stats upon player respawn
    public void Respawn()
    {
        healthBar.enabled = true;
        model.enabled = true;
        SetHealth(maxHealth);
    }
}
