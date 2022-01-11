using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public float gravity = -30f;
    public float moveSpeed = 15f;
    public float jumpSpeed = 50f;
    public float health;
    public float maxHealth = 100f;
    public float throwForce = 1000f;

    public int ammoAmount = 0;
    public int maxAmmo = 4;

    public Image healthBar;

   // public bool completed;
    private bool[] inputs;
    private float yVelocity = 0;

    public Canvas completed;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;


        inputs = new bool[5];
    }

    //Processes player input and moves the player.
    public void FixedUpdate()
    {
        if(health <= 0f)
        {
            return;
        }
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    // Calculates the player's desired movement direction and moves him
    private void Move(Vector2 _inputDirection)
    {
        //transform the players movement using the inputs direction and speed
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        //check jump status for jump functiomality
        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        PacketSender.PlayerPosition(this);
        PacketSender.PlayerRotation(this);
    }

    //Updates the player input with newly received input
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    //detect contact with spikes and take damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes"))
        {
            TakeDamage(20f);
        }
    }

    //manages the players health statuc
    public void TakeDamage(float _damage)
    {
        //if health is 0 , return out of the function
        if(health <= 0f)
        {
            return;
        }

        //take damage from health
        health -= _damage;

        //set the health bar
        healthBar.fillAmount = health / maxHealth;

        //if health is less than the damage taken, then respawn
        if(health <= _damage)
        {
            health = 0f;
            controller.enabled = false;
            healthBar.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            PacketSender.PlayerPosition(this);
            StartCoroutine(Respawn());

        }

        PacketSender.PlayerHealth(this);

    }

    //respawn and reset stats
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        healthBar.enabled = true;
        PacketSender.PlayerRespawned(this);

    }

    //sets if player can pick up more ammo or not
    public bool getAmmo()
    {
        if(ammoAmount >= maxAmmo)
        {
            return false;
        }

        ammoAmount++;
        return true;
    }
}
