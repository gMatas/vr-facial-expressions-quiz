using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using UnityEngine;
using UnityEngine.UI;

public class CVSystemController : MonoBehaviour
{
    public CameraController cameraController;
    public bool DebugMode;

    private RawImage cameraImage;
    private Image<Bgr, byte> frameImage;
    private Texture2D texture2D;
    private bool isControllerReady;

    private FaceLandmarksDetector faceDetector;
    private FaceEmotionsClassifier emotionsClassifier;

    private readonly string faceDetectorModel = "Assets/Resources/Models/haarcascade_frontalface_alt2.xml";
    private readonly string faceLandmarksDetectorModel = "Assets/Resources/Models/lbfmodel.yaml";
    private readonly string faceEmotionsClassifierModel = "Assets/Resources/Models/faceexpress_v5.dat";

    public bool isRunning;
    public bool isBusy;
    private Dictionary<int, int> resultsDict; 

    // Use this for initialization
    void Start()
    {
        isRunning = false;
        isBusy = false;
        resultsDict = new Dictionary<int, int>();

        if (cameraController == null)
        {
            Debug.Log("No camera attached.");
            return;
        }

        cameraImage = cameraController.GetComponent<RawImage>();

        isControllerReady = false;
        StartCoroutine(SetupDetector());

        faceDetector = new FaceLandmarksDetector(faceDetectorModel, faceLandmarksDetectorModel);
        emotionsClassifier = new FaceEmotionsClassifier(faceEmotionsClassifierModel);
    }

    private IEnumerator SetupDetector()
    {
        yield return new WaitUntil(() => cameraController.IsCameraAvailable());

        WebCamTexture webCamTexture = cameraController.GetCameraTexture();
        texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);
        if (DebugMode)
        {
            cameraImage.enabled = true;
            cameraImage.texture = texture2D;
        }
        else
        {
            cameraImage.enabled = false;
        }
        isControllerReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControllerReady) return;

        if (!isBusy && isRunning)
        {
            isBusy = true;
            StartCoroutine(CvPipline());
        }
    }

    public bool IsCvPipelineRunning()
    {
        return isRunning;
    }

    public void StartCvPipline()
    {
        isRunning = true;
    }

    public void StopCvPipeline()
    {
        isRunning = false;
        resultsDict.Clear();
    }

    public Dictionary<int, int> GetEmotions()
    {
        if (!isRunning)
            return null;

        Dictionary<int, int> emotionsDict = resultsDict;
        resultsDict = new Dictionary<int, int>();
        return emotionsDict;
    }

    public bool TakePhoto = false;
    private IEnumerator CvPipline()
    {
        CameraTextureFetching();
        UpdateFrameImageWithTexture(texture2D);
        ProcessImage(frameImage);

        if (TakePhoto)
        {
            frameImage.Save("D:\\My Work\\VR\\lol_watdafak.png");
            TakePhoto = false;
        }

        //UpdateCameraImageTextureWithImage(frameImage);
        isBusy = false;
        yield return null;
    }

    private void CameraTextureFetching()
    {
        WebCamTexture webCamTexture = cameraController.GetCameraTexture();
        texture2D.SetPixels(webCamTexture.GetPixels());
        texture2D.Apply();
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

    private void ProcessImage(Image<Bgr, byte> image)
    {
        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
        PointF[] facepoints = faceDetector.GetNormalisedFacepoints(grayImage);
        if (facepoints == null)
            return;

        int emotionCode = emotionsClassifier.Predict(facepoints, debug: true);
        if (!resultsDict.ContainsKey(emotionCode))
            resultsDict[emotionCode] = 0;
        resultsDict[emotionCode]++;

        //faceDetector.DebugDetectFace(grayImage, image);
        //faceDetector.DebugMarkFacialPoints(grayImage, image);
    }

    private void UpdateCameraImageTextureWithImage(Image<Bgr, byte> image)
    {
        if (image == null)
            Debug.Log("Image is null.");

        MemoryStream stream = new MemoryStream();
        image.Bitmap.Save(stream, image.Bitmap.RawFormat);
        texture2D.LoadImage(stream.ToArray());
    }

    //private void UpdateFrameImageWithTexture(Texture2D texture)
    //{
    //    if (texture == null)
    //        Debug.Log("Provided texture is null.");

    //    MemoryStream stream = new MemoryStream(texture.EncodeToPNG());
    //    Image<Rgba, byte> temp = new Image<Rgba, byte>(texture.width, texture.height)
    //    {
    //        Bitmap = new System.Drawing.Bitmap(stream)
    //    };
    //    frameImage = temp.Convert<Bgr, byte>();
    //}

    //private void UpdateCameraImageTextureWithImage(Image<Bgr, byte> image)
    //{
    //    if (image == null)
    //        Debug.Log("Image is null.");

    //    MemoryStream stream = new MemoryStream();
    //    image.Bitmap.Save(stream, image.Bitmap.RawFormat);
    //    texture2D.LoadImage(stream.ToArray());
    //}
}
