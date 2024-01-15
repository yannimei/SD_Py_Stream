using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ControlCamera : MonoBehaviour
{
    public int resWidth = 512;
    public int resHeight = 512;
    public Camera mainCamera;
    public RenderTexture originalRT;
    // Check for user input, for example, you can use a key press or mouse scroll wheel
    public float newFieldOfView;
    // Start is called before the first frame update

    private bool takeShot = false;
    void Start()
    {
        newFieldOfView = mainCamera.fieldOfView;
        originalRT = mainCamera.targetTexture;
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshot/screen_{1}x{2}_{3}.png",
                              Application.dataPath,
                              width,height,
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"));
    }

    public void TakeShot()
    {
        takeShot = true;
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        takeShot |= OVRInput.GetDown(OVRInput.Button.One);
        if (takeShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            mainCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            mainCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

            mainCamera.targetTexture = originalRT;
            RenderTexture.active = originalRT;
            Destroy(rt);

            byte[] imgBytes = screenShot.EncodeToPNG();
            string fileName = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(fileName, imgBytes);
            Debug.Log(string.Format("take screenshot to: {0}", fileName));
            takeShot = false;
        }

        // Check for user input, for example, you can use a key press or mouse scroll wheel
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            // Increase the field of view by 1
            newFieldOfView -= 1f;
        } else if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            newFieldOfView += 1f;
        }

        // Call a method to change the camera's field of view
        ChangeFieldOfView(newFieldOfView);
    }

    void ChangeFieldOfView(float newFOV)
    {
        
        // Check if the camera component exists
        if (mainCamera != null)
        {
            // Set the new field of view
            mainCamera.fieldOfView = newFOV;

            // Print a message to the console (optional)
            Debug.Log("Camera Field of View changed to: " + newFOV);
        }
        else
        {
            // Print an error message if the camera component is not found
            Debug.LogError("Camera component not found on this GameObject.");
        }
    }



}
