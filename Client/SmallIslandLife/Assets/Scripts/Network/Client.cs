using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour
{
    public static Client instance;

    public const int MAX_BUFFER_SIZE = 4096;

    private Socket socket;

    public uint UID
    {
        get;
        set;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            return;
        }

        Destroy(gameObject);
    }

    public void ConnectToServer(int serverId)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            socket.Connect(IPAddress.Parse("192.168.0.5"), 4804 + serverId);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void SendData(ushort command, string message)
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }

        byte[] data = Encoding.ASCII.GetBytes($"{command}|{message}");

        socket.Send(data, data.Length, SocketFlags.None);
    }

    public void RecvData(Action<string> callback)
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }

        ThreadPool.QueueUserWorkItem((_) =>
        {
            byte[] buffer = new byte[MAX_BUFFER_SIZE];

            socket.Receive(buffer, buffer.Length, SocketFlags.None);

            callback(Encoding.ASCII.GetString(buffer));
        });
    }

    public ushort GetCommandByData(string data)
    {
        if (data == null || data.Length == 0)
        {
            return Command.INVALID;
        }

        string[] split = data.Split('|');

        if (split == null || split.Length == 0)
        {
            return Command.INVALID;
        }

        return ushort.TryParse(split[0], out ushort result) ? result : Command.INVALID;
    }

    public void DisconnectToServer()
    {
        if (!socket.Connected)
        {
            return;
        }

        socket.Close();

        socket = null;
    }
}
