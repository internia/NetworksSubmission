//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BulletManager : MonoBehaviour
//{
//    //bullet id
//    public int bulletID;
//    //particle effect prefab to play upon impact
//    public GameObject impact;


//    public void Initialize(int _id)
//    {
//        bulletID = _id;
//    }

//    //function to deonate what to do when a bullet either colliders with player / goes as far as it can without collision
//    public void Impact(Vector3 _position)
//    {
//        Debug.Log("BulletManager Impact");
//        //grab bullets current position
//        transform.position = _position;
//        //returnsd the instantiated impact in place of the bullet
//        Instantiate(impact, transform.position, Quaternion.identity);
//        //remove the id from the bullet dictionary
//        GameManager.bullets.Remove(bulletID);
//        //remove the bullet from the game scene
//        Destroy(gameObject);
//    }
//}
