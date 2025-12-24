namespace Controls.ImageCropper;

public class ImageCropper
{
    private static IImageCropper? _implementation;

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

    private static IImageCropper CreateCropper()
    {
        return new ImageCropperImplementation();
    }
}