//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;

//public class CVWorker
//{
//    private static bool isRunning;
//    private static Thread thread;

//    private static FaceLandmarksDetector detector;

//    public CVWorker(string faceDetectorModel, string landmarksLocatorModel)
//    {
//        isRunning = false;
//        detector = new FaceLandmarksDetector(faceDetectorModel, landmarksLocatorModel);
//    }

//    public void Run()
//    {
//        ThreadStart start = new ThreadStart();
//        thread = new Thread(start);
//        thread.Start();

//    }

//    private static void Process()
//    {
        
//    }
//}
