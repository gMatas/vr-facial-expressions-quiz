using System.Drawing;
using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using UnityEngine;

public class FaceEmotionsClassifier
{
    public static readonly int EMOTION_ANGRY = 0;
    public static readonly int EMOTION_HAPPY = 3;
    public static readonly int EMOTION_SURPRISED = 5;
    public static readonly int EMOTION_NEUTRAL = 6;

    private string modelFilepath;
    private SVM classifier;

    public FaceEmotionsClassifier(string modelFilepath)
    {
        classifier = new SVM();
        classifier.Load(modelFilepath);

        classifier.C = 2.67f;
        classifier.Gamma = 5.383f;
        classifier.Type = SVM.SvmType.CSvc;
        classifier.SetKernel(SVM.SvmKernelType.Linear);
    }

    public int Predict(PointF[] facepoints, bool debug = false)
    {
        int nCoords = facepoints.Length * 2;
        int nSamples = 1;

        Mat inputData = new Mat(nSamples, nCoords, Emgu.CV.CvEnum.DepthType.Cv32F, 1);
        ConvertFacepointsToMat(facepoints, inputData);

        int prediction = (int)classifier.Predict(inputData);
        Debug.Log(prediction.ToString());
        return prediction;
    }

    public static void ConvertFacepointsToMat(PointF[] facepoints, Mat outputMat)
    {
        var row = outputMat.Row(0);
        for (int i = 0; i < facepoints.Length; i++)
        {
            int j = i * 2;
            float x = facepoints[i].X;
            float y = facepoints[i].Y;
            row.Col(j).SetTo(new MCvScalar(x));
            row.Col(j + 1).SetTo(new MCvScalar(y));
        }
    }
}
