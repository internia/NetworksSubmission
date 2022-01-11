using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public GameObject startMenu;
    public InputField username;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("instance already exists");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        username.interactable = false;
        Client.instance.ConnectToServer();


    }
}
