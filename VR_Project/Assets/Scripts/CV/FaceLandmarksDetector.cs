using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine;

public class FaceLandmarksDetector
{
    private CascadeClassifier faceDetector;

    public FaceLandmarksDetector(string faceDetectorModel, string faceLandmarkerModel)
    {
        faceDetector = new CascadeClassifier(faceDetectorModel);
    }

    public Rectangle? DetectFace(Image<Gray, byte> image)
    {
        Rectangle[] facesRects = faceDetector.DetectMultiScale(image);

        if (facesRects.Length == 0)
            return null;

        // Get the biggest face rectangle
        Rectangle faceRect = facesRects[0];
        int rectEdgeLength = faceRect.Width.CompareTo(faceRect.Height) < 0 ? faceRect.Width : faceRect.Height;
        for (int i = 1; i < facesRects.Length; i++)
        {
            Rectangle rect = facesRects[i];
            int length = rect.Width.CompareTo(rect.Height) < 0 ? rect.Width : rect.Height;
            if (length > rectEdgeLength)
            {
                faceRect = rect;
                rectEdgeLength = length;
            }
        }
        return faceRect;
    }

    public Image<Bgr, byte> DebugDetectFace(Image<Gray, byte> grayImage, Image<Bgr, byte> image)
    {
        var detectionResult = DetectFace(grayImage);
        if (!detectionResult.HasValue)
            return image;

        Rectangle faceRect = detectionResult.Value;
        image.Draw(faceRect, new Bgr(255, 255, 255), 1);
        image.Draw(new CircleF(faceRect.Location, 10), new Bgr(255, 255, 255), 1);
        return image;
    }
}
