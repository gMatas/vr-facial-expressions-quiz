using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine.UI;

public class CVSystemController : MonoBehaviour
{
    public CameraController cameraController;

    private RawImage cameraImage;
    private Image<Bgr, byte> frameImage;
    private Texture2D texture2D;
    private bool isControllerReady;

    private FaceLandmarksDetector faceDetector;

    private readonly string faceDetectorModel = "Assets/Resources/Models/haarcascade_frontalface_alt2.xml";
    private readonly string faceLandmarksDetectorModel = "Assets/Resources/Models/lbfmodel.yaml";

    // Use this for initialization
    void Start()
    {
        if (cameraController == null)
        {
            Debug.Log("No camera attached.");
            return;
        }

        cameraImage = cameraController.GetComponent<RawImage>();

        isControllerReady = false;
        StartCoroutine(SetupDetector());

        faceDetector = new FaceLandmarksDetector(faceDetectorModel, faceLandmarksDetectorModel);
    }

    private IEnumerator SetupDetector()
    {
        yield return new WaitUntil(() => cameraController.IsCameraAvailable());

        WebCamTexture webCamTexture = cameraController.GetCameraTexture();
        texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);
        cameraImage.texture = texture2D;
        isControllerReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControllerReady) return;

        WebCamTexture webCamTexture = cameraController.GetCameraTexture();
        texture2D.SetPixels(webCamTexture.GetPixels());
        texture2D.Apply();

        UpdateFrameImageWithTexture(texture2D);

        ProcessImage(frameImage);

        UpdateCameraImageTextureWithImage(frameImage);
    }

    private void ProcessImage(Image<Bgr, byte> image)
    {
        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
        faceDetector.DebugDetectFace(grayImage, image);
    }

    private void UpdateFrameImageWithTexture(Texture2D texture)
    {
        if (texture == null)
            Debug.Log("Provided texture is null.");

        MemoryStream stream = new MemoryStream(texture.EncodeToPNG());
        Image<Rgba, byte> temp = new Image<Rgba, byte>(texture.width, texture.height)
        {
            Bitmap = new System.Drawing.Bitmap(stream)
        };
        frameImage = temp.Convert<Bgr, byte>();
    }

    private void UpdateCameraImageTextureWithImage(Image<Bgr, byte> image)
    {
        if (image == null)
            Debug.Log("Image is null.");

        MemoryStream stream = new MemoryStream();
        image.Bitmap.Save(stream, image.Bitmap.RawFormat);
        texture2D.LoadImage(stream.ToArray());
    }
}
