using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;

public class ImgListener : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;


    void Start()
    {
        StartThread();
    }

    void StartThread()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    // Configure and control the server
    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();
        Debug.Log("connected");

        // Start listening
        //running = true;
        //while (running)
        //{
        //    Connection();
        //}
        //remove runnig temporarily
        Connection();
        server.Stop();
    }

    void Connection()
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();

        // read file size
        byte[] header = new byte[1024];
        nwStream.Read(header, 0, header.Length);
        int fileSize = Int32.Parse(Encoding.Default.GetString(header));
        Debug.Log("file size: "+ fileSize);

        //create an array for recieved data based on file size
        byte[] buffer = new byte[fileSize];

        //read the data, and caculate how many bite being read
        int bytesRead = nwStream.Read(buffer, 0, buffer.Length);
        Debug.Log("bytes read:  "+ bytesRead);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log("Data recieved:  "+ dataReceived);

        // transform Base64string into bytes
        byte[] imageBytes = Convert.FromBase64String(dataReceived);

        // Make sure we're not getting an empty string
        //dataReceived.Trim();

        //if (dataReceived != null && dataReceived != "")
        if (buffer != null)
        {
            
            UnityMainThreadDispatcher.Instance().Enqueue(DisplayImage(imageBytes));

            nwStream.Write(buffer, 0, bytesRead);

            Debug.Log("get material data");
        }
    }


    Material generatedMaterial;

    public IEnumerator DisplayImage(byte[] imageData)
    {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Create a new material with the texture
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;

            generatedMaterial = material;

            yield return null;
    }

    

    void Update()
    {
        if (generatedMaterial != null)
        {
            this.GetComponent<Renderer>().material = generatedMaterial;
        }
    }
}
