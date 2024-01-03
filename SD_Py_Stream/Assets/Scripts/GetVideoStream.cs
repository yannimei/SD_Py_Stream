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

public class GetVideoStream : MonoBehaviour
{

    byte[] header;
    int recieved;
    int fileSize;
    NetworkStream dataStream;
    MemoryStream ms;
    TcpClient camClient;
    bool connectCam = false;
    int camPort = 25002;
    string camIP = "127.0.0.1";

    void Start()
    {
        getVideoStream();
    }


    void getVideoStream()
    {
        

        while (connectCam)
        {
            fileSize = 0;
            recieved = 0;
            camClient = new TcpClient(camIP, camPort);

            //get header
            dataStream = camClient.GetStream();
            while (!dataStream.DataAvailable)
            {
                //waste time
            }
            header = new byte[1024];
            dataStream.Read(header, 0, header.Length);
            fileSize = Int32.Parse(Encoding.Default.GetString(header)); //not sure
            byte[] result = Encoding.ASCII.GetBytes(fileSize.ToString());

            //send response
            dataStream.Write(result, 0, result.Length);

            ms = new MemoryStream();
            while (!dataStream.DataAvailable)
            {
                //waste time
            }
            int increment = 0;
            while (recieved < fileSize)
            {
                byte[] data = new byte[camClient.ReceiveBufferSize];
                increment = dataStream.Read(data, 0, data.Length);
                recieved += increment;
                //ms.Write(data.Take(increment).ToArray(), 0, increment);
                ms.Write(data, 0, increment);
            }
            //the below class simply sends function calls from secondary thread back to the main thread
            //UnityMainThreadDispatcher.Instance().Enqueue(convertBytesToTexture(ms.ToArray()));
            convertBytesToTexture(ms.ToArray());
            dataStream.Close();
            camClient.Close();

        }
    }

    void convertBytesToTexture(byte[] byteArray)
    {
        try
        {
            Texture2D camTexture = new Texture2D(2, 2);
            camTexture.LoadImage(byteArray); //Texture2D object

            RawImage camImage = GetComponent<RawImage>();
            camImage.texture = camTexture; //RawImage object
        }
        catch (Exception e)
        {
            print(e);
        }
    }

    void Update()
    {
        
    }
}
