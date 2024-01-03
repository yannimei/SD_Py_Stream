using System.Collections;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using PimDeWitte.UnityMainThreadDispatcher;


public class UnityImageListener : MonoBehaviour
{
    private TcpClient client;
    private Thread receiveThread;
    private bool isReceiving = false;
    

    void Start()
    {
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            // Replace with your server's IP address and port
            string serverIP = "127.0.0.1";
            int serverPort = 25002;

            client = new TcpClient(serverIP, serverPort);
            receiveThread = new Thread(new ThreadStart(ReceiveImage));
            receiveThread.Start();
            isReceiving = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to the server: " + e.Message);
        }
    }

    void ReceiveImage()
    {
        while (isReceiving)
        {
            try
            {
                NetworkStream dataStream = client.GetStream();

                // Read the header to get the image size
                byte[] header = new byte[4096];
                dataStream.Read(header, 0, header.Length);
                int imageSize = Int32.Parse(Encoding.UTF8.GetString(header).Trim());
                Debug.Log("img size is: " + imageSize);

                // Read the image data
                byte[] imageData = new byte[imageSize];
                int bytesRead = 0; 
                int increment = 0;

                while (!dataStream.DataAvailable)
                {
                    // Waste time
                }

                while (bytesRead < imageSize)
                {
                    bytesRead += dataStream.Read(imageData, bytesRead, imageSize - bytesRead);
                    
                    Debug.Log("read");
                }

                // Display the received image
                DisplayImage(imageData);
                //UnityMainThreadDispatcher.Instance().Enqueue(DisplayImage(imageData));
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving image: " + e.Message.ToString());
                isReceiving = false;
            }
        }
    }

    void DisplayImage(byte[] imageData)
    {
        try
        {
            Texture2D texture = new Texture2D(512, 512);
            texture.LoadImage(imageData);

            // Assuming you have a RawImage component on your UI
            //RawImage rawImage = GetComponent<RawImage>();
            //rawImage.texture = texture;

            // Create a new material with the texture
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;

            this.GetComponent<Renderer>().material = material;

        }
        catch (Exception e)
        {
            Debug.LogError("Error displaying image: " + e.Message);
        }
    }



    void OnDestroy()
    {
        // Close the client and stop the receiving thread when the script is destroyed
        if (client != null && client.Connected)
        {
            isReceiving = false;
            client.Close();
            receiveThread.Join(); // Wait for the receiving thread to finish
        }
    }
}