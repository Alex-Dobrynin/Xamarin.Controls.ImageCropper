namespace Controls.ImageCropper
{
    public class ImageCropper
    {
        static IImageCropper _implementation;

        public static IImageCropper Current
        {
            get
            {
                return _implementation ??= CreateCropper();
            }
            set
            {
                _implementation = value;
            }
        }

        static IImageCropper CreateCropper()
        {
#if NETSTANDARD2_0 || NETSTANDARD2_1
            return null;
#else
            return new ImageCropperImplementation();
#endif
        }
    }
}
