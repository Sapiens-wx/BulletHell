using UnityEngine;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Communicator : Singleton<Communicator>
{
    [SerializeField] string ip;
    [SerializeField] int port;
    TcpClient client;
    NetworkStream stream;
    bool connected;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public void Init(){
        Init(ip,port);
    }
    public void Init(string ip, int port)
    {
        connected = false;
        Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    client = new TcpClient(ip, port);
                    stream = client.GetStream();
                    Debug.Log("Communicator: connected");
                    break;
                }
                catch (SocketException e)
                {
                    Task.Delay(1000).Wait();
                }
            }
            connected = true;
        });
    }
    public void Close()
    {
        client.Close();
    }
    public void SendMessage(byte[] data)
    {
        if (!connected)
        {
            //Debug.Log("Warning: no connection");
            return;
        }
        try
        {
            stream.Write(data, 0, data.Length);
        } catch(System.IO.IOException e)
        {
            Debug.Log("Communicator: warning: connection lost");
            connected = false;
        }
    }
}