using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    ClientSend.PlayerShoot(camTransform.forward);
        //}

        //get the player shoot input
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //  ClientSend.PlayerShootBullet(camTransform.forward);
           // ClientSend.PlayerThrowItem(camTransform.forward);

        }

    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    // Sends player input to the server 
    private void SendInputToServer()
    {
        //player movement inputs
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(_inputs);
    }
}
