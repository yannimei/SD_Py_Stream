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
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();
        Debug.Log("connected");

        // Start listening
        running = true;
        while (running)
        {
            Connection();
        }
        
        server.Stop();
    }

    void Connection()
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();

        //create an array for recieved data
        byte[] buffer = new byte[client.ReceiveBufferSize];
        //byte[] buffer = new byte[99999];

        //read the data, and caculate how many bite being read
        int bytesRead = nwStream.Read(buffer, 0, buffer.Length);
        Debug.Log(bytesRead);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log(dataReceived);

        // dataReceived.Trim();
        // Decode Base64 and display the image (you may need to adjust this part)
        byte[] imageBytes = Convert.FromBase64String(dataReceived);

        // Make sure we're not getting an empty string
        //dataReceived.Trim();
        //if (dataReceived != null && dataReceived != "")
        if (buffer != null)
        {
            // Convert the received string of data to the format we are using
            //position = ParseData(dataReceived);
            Debug.Log("get material data");

            UnityMainThreadDispatcher.Instance().Enqueue(DisplayImage(imageBytes));

            nwStream.Write(buffer, 0, bytesRead);
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
        // Set this object's position in the scene according to the position received
        //transform.position = position;

        this.GetComponent<Renderer>().material = generatedMaterial;
    }
}
