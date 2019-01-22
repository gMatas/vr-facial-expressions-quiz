using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public RawImage CameraImage;
    public AspectRatioFitter Fitter;

    private WebCamTexture cameraTexture;
    private WebCamDevice[] devices;
    private bool isCameraAvailable = false;

	// Use this for initialization
	void Start ()
    {
        devices = WebCamTexture.devices;

        CameraImage.enabled = true;

        if (devices.Length == 0)
        {
            Debug.Log("No camera devices found :(");
            return;
        }

        foreach (WebCamDevice device in devices)
            if (device.isFrontFacing)
            {
                cameraTexture = new WebCamTexture(device.name, Screen.width, Screen.height);
                break;
            }

        if (cameraTexture == null)
        {
            Debug.Log("No front facing camera device was found.");
            return;
        }

        cameraTexture.Play();
        isCameraAvailable = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isCameraAvailable)
            return;

        float aspectRatio = (float) cameraTexture.width / cameraTexture.height;
        Fitter.aspectRatio = aspectRatio;

        float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f;
        CameraImage.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orientation = -cameraTexture.videoRotationAngle;
        CameraImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, orientation);
	}

    public bool IsCameraAvailable()
    {
        return isCameraAvailable;
    }

    public WebCamTexture GetCameraTexture()
    {
        return cameraTexture;
    }
}
