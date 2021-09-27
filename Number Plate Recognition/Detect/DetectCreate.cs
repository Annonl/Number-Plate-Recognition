namespace Number_Plate_Recognition.Detect
{
    class DetectCreate
    {
        static public DetectWay detectWay = DetectWay.Yolo;
        static private IDetect detect;

        public static IDetect Detect(string fileName)
        {
            if (detectWay == DetectWay.Yolo)
                detect = new YoloDetect(fileName);
            else if (detectWay == DetectWay.Haar)
                detect = new HaarDetect(fileName);
            else
                detect = new YoloDetect(fileName);
            return detect;
        }
    }
}
