using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSender
{
    // Sends a packet to a client via TCP 
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    // Sends a packet to a client via UDP 
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    // Sends a packet to all clients via TCP 
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    // Sends a packet to all clients except one via TCP >
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    // Sends a packet to all clients via UDP 
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    // Sends a packet to all clients except one via UDP 
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    // Sends a welcome message to the given client 
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    // Tells a client to spawn a player 
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    // Sends a player's updated position to all clients 
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    // Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation) 
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int playerID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(playerID);

            SendTCPDataToAll(_packet);
        }

    }

    public static void PlayerHealth(Player _player)
    {
        using(Packet _packet  = new Packet((int)ServerPackets.playerHealth))
        {

            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }


    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {

            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void CreateAmmoSpawner(int _toClient, int ammoID, Vector3 _spawnerPosition, bool _hasAmmo)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createAmmoSpawner))
        {
            _packet.Write(ammoID);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasAmmo);

            SendTCPData(_toClient, _packet);
        }

    }

    public static void AmmoSpawned(int _ammoID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.ammoSpawned))
        {
            _packet.Write(_ammoID);

            SendTCPDataToAll(_packet);
        }

    }

    public static void AmmoPickUp(int _ammoID, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.ammoPickUp))
        {
            _packet.Write(_ammoID);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    //public static void CompletionStatus(bool _completed)
    //{
    //    using (Packet _packet = new Packet((int)ServerPackets.completionStatus))
    //    {
    //        _packet.Write(_completed);

    //        SendTCPDataToAll(_packet);
    //    }

    //}

    #endregion
}
