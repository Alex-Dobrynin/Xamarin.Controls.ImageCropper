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
#if ANDROID || IOS
            return new ImageCropperImplementation();
#else
            return null;
#endif
        }
    }
}
