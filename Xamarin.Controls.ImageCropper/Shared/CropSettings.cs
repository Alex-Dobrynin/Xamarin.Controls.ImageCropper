namespace Controls.ImageCropper
{
    public class CropSettings
    {
        public enum CropShapeType
        {
            Rectangle,
            Oval
        };

        public CropShapeType CropShape { get; set; } = CropShapeType.Rectangle;
        public int AspectRatioX { get; set; } = 0;
        public int AspectRatioY { get; set; } = 0;
        public string PageTitle { get; set; } = null;
    }
}
