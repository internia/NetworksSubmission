using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //create dictionaries to handle all instances within the game scene clientside
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, AmmoSpawner> ammo = new Dictionary<int, AmmoSpawner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();




    //holds the prefabs to be loaded in clientside
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject ammoPrefab;
    public GameObject projectilePrefab;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //spawns players into their application, either their istance using the localplayer prefab or any other player that joins using the enemy prefab
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.id)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        //initialize the player with id and name
        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        //add the initialized player to the dictionary
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    //create the ammo spawner making use of the predefined prefab for ammo
    public void CreateAmmoSpawner(int ammoID, Vector3 _position, bool _hasAmmo)
    {
        GameObject spawner = Instantiate(ammoPrefab, _position, ammoPrefab.transform.rotation);
        spawner.GetComponent<AmmoSpawner>().Initialize(ammoID, _hasAmmo);
        ammo.Add(ammoID, spawner.GetComponent<AmmoSpawner>());
    }


    public void SpawnProjectile(int _id, Vector3 _position)
    {
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

}
