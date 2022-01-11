using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;

    //buffer to store the response bytes
    public static int dataBufferSize = 4096;


    //ip address and port for the client to connect to
    public string ip = "192.168.0.10";
    public int port = 26950;

    //setting client id
    public int id = 0;


    public TCP tcp;
    public UDP udp;

    //whether or not the client is connected to the server
    private bool connected = false;

    //packet handling information
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

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

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect(); 
    }

    // this function is used to establish a connection to the server upon the clientside buttonclick 
    public void ConnectToServer()
    {
        InitializeClientData();

        connected = true;
        tcp.Connect(); 
    }

    //this class sets up all functionality needed to establish a tcp connection
    public class TCP
    {
        //provides client connections for TCP network services
        public TcpClient socket;

        // sets a client stream for reading and writing
        private NetworkStream stream;

        //establish a packet to recieve dara
        private Packet receivedData;
        private byte[] receiveBuffer;

        
        //connects to the server
        public void Connect()
        {
            //create a new tcpclient connection
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            //begins an asynchronous request for a remote host connection. 
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        //callback functiion to create a stream.
        private void ConnectCallback(IAsyncResult _result)
        {
            //ends a pending asynchronous connection request.
             socket.EndConnect(_result);

            //if the connection fails, return out of this function
            if (!socket.Connected)
            {
                return;
            }

            //obtain the NetworkStream
            stream = socket.GetStream();

            receivedData = new Packet();

            //begins an asynchronous read operation.
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        // sends data to the client via TCP
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    /// begins an asynchronous write to a stream to send data to server
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            //error catching for server send
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        // Reads incoming data from the stream 
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                //sets bytelength by using the fucntion which handles the end of a read to get the result
                int _byteLength = stream.EndRead(_result);

                //if said result returns empty
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                //create new data byte
                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                //reset receivedData if all data was handled correctly
                receivedData.Reset(HandleData(_data)); 

                //begins an asynchronous read from the NetworkStream.
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        // prepares received data to be used by the appropriate packet handler methods 
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                // if client's received data contains a packet
                _packetLength = receivedData.ReadInt();

                //if the packet is empty
                if (_packetLength <= 0)
                {
                    // reset receivedData instance to allow it to be reused
                    return true; 
                }
            }

            // whilstt packet contains data AND packet data length doesnt exceed the length of the packet
            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });

                _packetLength = 0; // reset packet length
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    
                    if (_packetLength <= 0)
                    { 
                        return true; 
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        // disconnects from the server and cleans up the TCP connection 
        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    //this class sets up all functionality needed to establish a udp connection

    public class UDP
    {
        //provides client connections for UDP network services
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            //represents a network endpoint as an IP address and a port number
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        // attempts to connect to the server via UDP 
        public void Connect(int _localPort)
        {
            //assigns the local port number.
            socket = new UdpClient(_localPort);

            //connects to remote host using the specified network endpoint
            socket.Connect(endPoint);

            //receives a datagram from a remote host asynchronously
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        // sends data to the client via UDP 
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.id); // Insert the client's ID at the start of the packet
                if (socket != null)
                {
                    //sends data to a remote host asynchronously with the destination being previously specified by a call to #Connect'
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        // receives incoming UDP data 
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                //an array of bytes that contains datagram data set by the end of a pending asynchronous received
                byte[] _data = socket.EndReceive(_result, ref endPoint);

                //
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        // prepares received data to be used by the appropriate packet handler methods 
        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                }
            });
        }

        // disconnects from the server and cleans up the udp connection 
        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    // initializes all necessary client data 
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientManager.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientManager.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientManager.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientManager.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientManager.PlayerDisconnected},
            { (int)ServerPackets.playerHealth, ClientManager.PlayerHealth },
            { (int)ServerPackets.playerRespawned, ClientManager.PlayerRespawned },
            { (int)ServerPackets.createAmmoSpawner, ClientManager.CreateAmmoSpawner },
            { (int)ServerPackets.ammoSpawned, ClientManager.AmmoSpawned },
            { (int)ServerPackets.ammoPickUp, ClientManager.AmmoPickedUp }
        };
        Debug.Log("Initialized packets.");
    }

    // disconnects from the server and stops all network traffic 
    private void Disconnect()
    {
        if (connected)
        {
            connected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("disconneted from the server");
        }
    }
}
