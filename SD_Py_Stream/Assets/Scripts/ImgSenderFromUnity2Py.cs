using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
public class ImgSenderFromUnity2Py : MonoBehaviour
{
    // Start is called before the first frame update
    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;
    private byte[] imageData;
    NetworkStream nwStream;

    void Start()
    {
        StartThread();
    }

    void StartThread()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(SendData);
        thread = new Thread(ts);
        thread.Start();
    }

    void SendData()
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

    void Connection()
    {

        // Read data from the network stream
        nwStream = client.GetStream();

        try
        {
            UnityMainThreadDispatcher.Instance().Enqueue(LoadAndSendImage());
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending image: " + e.Message);
        }

    }

    public IEnumerator LoadAndSendImage()
    {

        // Load image from Resources folder (you can change the path accordingly)
        Texture2D texture = Resources.Load<Texture2D>("Image/1");
        
        // Convert the image to bytes
        imageData = texture.EncodeToPNG();
        Debug.Log(imageData);
       
        // Send the image data
        nwStream.Write(imageData, 0, imageData.Length);

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
