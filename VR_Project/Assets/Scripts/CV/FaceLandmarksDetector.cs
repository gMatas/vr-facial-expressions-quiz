using System;
using System.Drawing;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;
using Emgu.CV.Structure;

public class FaceLandmarksDetector
{
    private CascadeClassifier faceDetector;
    private FacemarkLBF facemark;

    public FaceLandmarksDetector(string faceDetectorModel, string faceLandmarkerModel)
    {
        // Load face detector model
        faceDetector = new CascadeClassifier(faceDetectorModel);

        // Load facemark model (face landmarker)
        FacemarkLBFParams facemarkParams = new FacemarkLBFParams();
        facemark = new FacemarkLBF(facemarkParams);
        facemark.LoadModel(faceLandmarkerModel);
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

    public VectorOfPointF MarkFacialPoints(Image<Gray, byte> image, Rectangle faceRect, out bool isSuccess)
    {
        VectorOfVectorOfPointF landmarks = new VectorOfVectorOfPointF();
        VectorOfRect faces = new VectorOfRect(new Rectangle[] { faceRect });
        isSuccess = facemark.Fit(image, faces, landmarks);
        if (isSuccess)
            return landmarks[0];  // return the landmarks for the first (and only) face rectangle 
        return new VectorOfPointF();  // return an empty vector
    }

    public PointF[] GetNormalisedFacepoints(Image<Gray, byte> grayImage)
    {
        var detectionResult = DetectFace(grayImage);
        if (!detectionResult.HasValue)
            return null;

        bool isSuccess;
        Rectangle faceRect = detectionResult.Value;
        VectorOfPointF landmarks = MarkFacialPoints(grayImage, faceRect, out isSuccess);
        if (!isSuccess)
            return null;

        PointF[] facepoints = landmarks.ToArray();
        NormalizeFacepoints(facepoints);
        return facepoints;
    }

    public Image<Bgr, byte> DebugDetectFace(Image<Gray, byte> grayImage, Image<Bgr, byte> image)
    {
        var detectionResult = DetectFace(grayImage);
        if (!detectionResult.HasValue)
            return image;

        Rectangle faceRect = detectionResult.Value;
        image = DrawFaceRect(image, faceRect);
        return image;
    }

    public Image<Bgr, byte> DebugMarkFacialPoints(Image<Gray, byte> grayImage, Image<Bgr, byte> image)
    {
        var detectionResult = DetectFace(grayImage);
        if (!detectionResult.HasValue)
            return image;

        bool isSuccess;
        Rectangle faceRect = detectionResult.Value;
        VectorOfPointF landmarks = MarkFacialPoints(grayImage, faceRect, out isSuccess);
        if (!isSuccess)
            return image;

        PointF[] landmarksPoints = landmarks.ToArray();
        Bgr color = new Bgr(0, 255, 255);
        int landmarkIndex = 0;
        foreach (PointF point in landmarksPoints)
        {
            image.Draw(new CircleF(point, 1), color, 1);
            image.Draw(landmarkIndex++.ToString(), 
                new Point((int)point.X, (int)point.Y), 
                Emgu.CV.CvEnum.FontFace.HersheyPlain, 1.0, 
                new Bgr(255, 255, 255), 1);
        }

        PointF anchor = GetCenterOfMassPoint(landmarksPoints);
        image.Draw(new CircleF(anchor, 2), new Bgr(0, 0, 255), 1);

        //NormalizeFacepoints(landmarksPoints);

        return image;
    }

    private void NormalizeFacepoints(PointF[] facepoints)
    {
        // Rebase facepoints to the nose point as the center of the plane.
        const int noseTipPointIndex = 30;
        PointF noseTipPoint = facepoints[noseTipPointIndex];
        PointF basePoint = new PointF(noseTipPoint.X, noseTipPoint.Y);
        for (int i = 0; i < facepoints.Length; i++)
        {
            facepoints[i].X -= basePoint.X;
            facepoints[i].Y -= basePoint.Y;
        }

        // Compute a rotation matrix to remove nose ridge tilt.
        double magnitude = Math.Sqrt(facepoints[27].X * facepoints[27].X + facepoints[27].Y * facepoints[27].Y);
        double cosAlpha = facepoints[27].X / magnitude;
        double alpha = Math.Acos(cosAlpha);
        double beta = alpha - Math.PI * .5;  // face tilt radius
        double[,] rotationMatrix = new double[,]
        {
                { Math.Cos(beta), -Math.Sin(beta) },
                { Math.Sin(beta), Math.Cos(beta) }
        };

        // Normalise the facepoints tilt using previously computed rotatation matrix.
        for (int i = 0; i < facepoints.Length; i++)
        {
            if (i == noseTipPointIndex)
                continue;

            double x = facepoints[i].X;
            double y = facepoints[i].Y;
            double x1 = rotationMatrix[0, 0] * x + rotationMatrix[0, 1] * y;
            double y1 = rotationMatrix[1, 0] * x + rotationMatrix[1, 1] * y;
            facepoints[i].X = (float)Math.Round(x1, 9);
            facepoints[i].Y = (float)Math.Round(y1, 9);
        }

        // Locate the center of facepoints cloud.
        PointF cloudCenterPoint = new PointF(0f, 0f);
        for (int i = 0; i < facepoints.Length; i++)
        {
            cloudCenterPoint.X += facepoints[i].X;
            cloudCenterPoint.Y += facepoints[i].Y;
        }
        cloudCenterPoint.X /= facepoints.Length;
        cloudCenterPoint.Y /= facepoints.Length;

        // Rebase facepoints to the center of point cloud as the center of the plane.
        for (int i = 0; i < facepoints.Length; i++)
        {
            facepoints[i].X -= cloudCenterPoint.X;
            facepoints[i].Y -= cloudCenterPoint.Y;
        }

        // Find most deviated coordinate.
        float maxCoordDev = -1;
        foreach (PointF point in facepoints)
        {
            float xDev = Math.Abs(point.X);
            float yDev = Math.Abs(point.Y);
            float coordDev = xDev > yDev ? xDev : yDev;
            if (coordDev >= maxCoordDev)
                maxCoordDev = coordDev;
        }

        // Normalise facepoints coordinates to fit range from -1 to 1 for both, x and y axies.
        for (int i = 0; i < facepoints.Length; i++)
        {
            facepoints[i].X /= maxCoordDev;
            facepoints[i].Y /= maxCoordDev;
        }
    }

    private Image<Bgr, byte> DrawFaceRect(Image<Bgr, byte> image, Rectangle faceRect)
    {
        image.Draw(faceRect, new Bgr(255, 255, 255), 1);
        return image;
    }

    private PointF GetCenterOfMassPoint(PointF[] points)
    {
        int sumOfX = 0;
        int sumOfY = 0;
        foreach (PointF point in points)
        {
            sumOfX += (int) point.X;
            sumOfY += (int) point.Y;
        }
        float meanX = sumOfX / (float) points.Length;
        float meanY = sumOfY / (float) points.Length;
        return new PointF(meanX, meanY);
    }
}
