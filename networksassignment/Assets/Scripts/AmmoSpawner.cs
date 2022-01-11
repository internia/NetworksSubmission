using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    //unique ammo id
    public int ammoID;

    //defines whether the ammo spawner has ammo in the location
    public bool hasAmmo;

    //ammo prefab to be spawned in
    public MeshRenderer ammoModel;

    //defines the bounce for the ammo 
    public float ammoAnim = 3f;

    //defines the default position for the ammo spawned - used for the bounce animation
    private Vector3 basePosition;

    private void Update()
    {
        //if ammo is present in the spawner, play the bounce animation
        if (hasAmmo)
        {
            transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * ammoAnim), 0f);
        }
    }
    public void Initialize(int _ammoID, bool _hasAmmo) 
    {
        ammoID = _ammoID;
        hasAmmo = _hasAmmo;

        ammoModel.enabled = hasAmmo;

        basePosition = transform.position;
    }

    //when ammo is spawned
    public void ammoSpawned()
    {
        hasAmmo = true;
        ammoModel.enabled = true;
    }

    //when player picks up the ammo
    public void ammoPickedUp()
    {
        hasAmmo = false;
        ammoModel.enabled = false;
    }

    //public void ammoUsed()
    //{
    //    hasAmmo = false;
    //    ammoModel.enabled = false;
    //}

}
