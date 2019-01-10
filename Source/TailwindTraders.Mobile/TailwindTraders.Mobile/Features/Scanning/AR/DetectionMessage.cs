namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class DetectionMessage
    {
        public float Xmin { get; set; }

        public float Ymin { get; set; }

        public float Xmax { get; set; }

        public float Ymax { get; set; }

        public float Score { get; set; }

        public string Label { get; set; }
    }
}
