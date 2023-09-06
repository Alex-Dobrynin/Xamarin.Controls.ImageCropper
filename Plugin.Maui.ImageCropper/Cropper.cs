namespace Plugin.Maui.ImageCropper;

public static class Cropper
{
    static IImageCropper _implementation;

    public static IImageCropper Current
    {
        get
        {
            if (_implementation is null)
                throw new ArgumentException("[Plugin.Maui.ImageCropper] You must call UseImageCropper() in your MauiProgram for initialization");

            return _implementation;
        }
        set => _implementation = value;
    }

    public static MauiAppBuilder UseImageCropper(this MauiAppBuilder builder, Action configure = null)
    {
        return UseImageCropper(builder, false, configure);
    }

    public static MauiAppBuilder UseImageCropper(this MauiAppBuilder builder, bool registerInterface, Action configure = null)
    {
#if IOS
        Current = new ImageCropperImplementation();
#else
        Current = new ImageCropperImplementation(builder);
#endif

        configure?.Invoke();

        if (registerInterface)
        {
            builder.Services.AddTransient((s) => Current);
        }

        return builder;
    }
}