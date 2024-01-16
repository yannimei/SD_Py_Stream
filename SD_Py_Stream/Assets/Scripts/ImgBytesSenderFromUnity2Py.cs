using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
// using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;

public class ImgBytesSenderFromUnity2Py : MonoBehaviour
{
    // Start is called before the first frame update
    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;
    public byte[] imageData = null;
    NetworkStream nwStream;

    public void StartThread()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(SendData);
        thread = new Thread(ts);
        thread.Start();
    }

    public void SendData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();
        Debug.Log("connected");

        // Start listening
        Connection();

        server.Stop();
    }

    public void Connection()
    {

        // Read data from the network stream
        nwStream = client.GetStream();

        try
        {
            nwStream.Write(imageData, 0, imageData.Length);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending image: " + e.Message);
        }

    }
}
