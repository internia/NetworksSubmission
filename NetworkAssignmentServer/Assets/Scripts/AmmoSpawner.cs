using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public static Dictionary<int, AmmoSpawner> ammo = new Dictionary<int, AmmoSpawner>();
    private static int nextAmmoID = 1;

    public int ammoID;
    public bool hasAmmo = false;

    private void Start()
    {
        hasAmmo = false;
        ammoID = nextAmmoID;
        nextAmmoID++; //ensures each ammo item has unique id
        ammo.Add(ammoID, this);//add instance to dictorionary

        StartCoroutine(SpawnItem());
    }

    private void OnTriggerEnter(Collider other)
    {
        //checks if spawner has item and if the collider is a player
        if (hasAmmo && other.CompareTag("Player"))
        {
            Player _player = other.GetComponent<Player>();
            if (_player.getAmmo())
            {
                ammoPickedUp(_player.id);
            }
        }
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(10f);
        hasAmmo = true;
        PacketSender.AmmoSpawned(ammoID);
    }

    private void ammoPickedUp(int byPlayer)
    {
        hasAmmo = false;
        PacketSender.AmmoPickUp(ammoID, byPlayer);
        StartCoroutine(SpawnItem());
    }

 }


