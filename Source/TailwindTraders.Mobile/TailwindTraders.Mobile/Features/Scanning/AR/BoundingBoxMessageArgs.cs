namespace TailwindTraders.Mobile.Features.Scanning.AR
{
    public class BoundingBoxMessageArgs
    {
        public static BoundingBoxMessageArgs FullScreen { get; } = new BoundingBoxMessageArgs 
        { 
            Xmin = 0, 
            Ymin = 0, 
            Xmax = 1, 
            Ymax = 1,
        };

        public float Xmin { get; set; }

        public float Ymin { get; set; }

        public float Xmax { get; set; }

        public float Ymax { get; set; }
    }
}
