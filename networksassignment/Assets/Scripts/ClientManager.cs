using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.id = _id;
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    //handle the packet which will call the player spawn stats
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    //handle the packet which denotes the players position
    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    //handle the packet which denotes the players rotation using quaternions
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }
    //handle the packet when the player disconnects
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);

    }
    //handle the packet which denotes the players health
    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    //handle the packet which denotes the players respawn status
    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    //handle the packet which spawns the ammo into the scene
    public static void CreateAmmoSpawner(Packet _packet)
    {
        int ammoID = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasAmmo = _packet.ReadBool();

        GameManager.instance.CreateAmmoSpawner(ammoID, _spawnerPosition, _hasAmmo);
    }

    //handle the ammo in the scene by assigning id
    public static void AmmoSpawned(Packet _packet)
    {
        int _ammoID = _packet.ReadInt();

        GameManager.ammo[_ammoID].ammoSpawned();
    }

    //handle the ammo status (if picked up or not)
    public static void AmmoPickedUp(Packet _packet)
    {
        int _ammoID = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.ammo[_ammoID].ammoPickedUp();
        GameManager.players[_byPlayer].ammoCount++;
    }

}