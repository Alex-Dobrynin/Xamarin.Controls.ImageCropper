namespace Plugin.Maui.ImageCropper;

public static class Cropper
{
    private static IImageCropper? _implementation;

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

    public static MauiAppBuilder UseImageCropper(this MauiAppBuilder builder, Action? configure = null)
    {
        return UseImageCropper(builder, false, configure);
    }

    public static MauiAppBuilder UseImageCropper(this MauiAppBuilder builder, bool registerInterface, Action? configure = null)
    {
#if IOS
        Current = new ImageCropperImplementation();
#elif ANDROID
        Current = new ImageCropperImplementation(builder);
#else
        Current = new ImageCropperImplementation();
#endif
        configure?.Invoke();

        if (registerInterface)
        {
            builder.Services.AddSingleton((s) => Current);
        }

        return builder;
    }
}