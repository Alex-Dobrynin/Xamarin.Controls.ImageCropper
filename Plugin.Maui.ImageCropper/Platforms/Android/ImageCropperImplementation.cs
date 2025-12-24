using AndroidX.Activity.Result;
using Com.Canhub.Cropper;
using Microsoft.Maui.LifecycleEvents;

namespace Plugin.Maui.ImageCropper.Platforms.Android;

public partial class ImageCropperImplementation
    : Java.Lang.Object, IImageCropper, IActivityResultCallback
{
    private ActivityResultLauncher? _launcher;
    private TaskCompletionSource<string>? _tcs;

    public ImageCropperImplementation(MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(l =>
            l.AddAndroid(ab =>
                ab.OnCreate((a, b) =>
                {
                    if (a is not MauiAppCompatActivity mauiActivity)
                        return;

                    _launcher = mauiActivity.RegisterForActivityResult(
                        new CropImageContract(),
                        this);
                })));
    }

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        if (_tcs != null && !_tcs.Task.IsCompleted)
            throw new InvalidOperationException(
                "Crop operation already in progress.");

        if (_launcher == null)
            throw new InvalidOperationException(
                "ImageCropper is not initialized yet.");

        _tcs = new TaskCompletionSource<string>();

        try
        {
            var fixAspect =
                settings.AspectRatioX > 0 &&
                settings.AspectRatioY > 0;

            var cropImageOptions = new CropImageOptions
            {
                CropperLabelText = settings.CropLabelText,
                ActivityBackgroundColor =
                    global::Android.Graphics.Color.DarkGray,

                CropShape =
                    settings.CropShape == CropSettings.CropShapeType.Oval
                        ? CropImageView.CropShape.Oval!
                        : CropImageView.CropShape.Rectangle!,

                OutputCompressFormat =
                    settings.CropShape == CropSettings.CropShapeType.Oval
                        ? global::Android.Graphics.Bitmap.CompressFormat.Png!
                        : global::Android.Graphics.Bitmap.CompressFormat.Jpeg!,

                FixAspectRatio = fixAspect
            };

            if (fixAspect)
            {
                cropImageOptions.AspectRatioX = settings.AspectRatioX;
                cropImageOptions.AspectRatioY = settings.AspectRatioY;
            }

            if (!string.IsNullOrWhiteSpace(settings.PageTitle))
            {
                cropImageOptions.ActivityTitle =
                    new Java.Lang.String(settings.PageTitle);
            }

            var uri =
                global::Android.Net.Uri.FromFile(
                    new Java.IO.File(imageFilePath));

            _launcher.Launch(
                new CropImageContractOptions(uri, cropImageOptions));
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public void OnActivityResult(Java.Lang.Object? cropImageResult)
    {
        if (_tcs == null || _tcs.Task.IsCompleted)
            return;

        if (cropImageResult is CropImage.ActivityResult result)
        {
            if (result.IsSuccessful)
            {
                var path =
                    result.GetUriFilePath(
                        MauiApplication.Context,
                        true);

                if (!string.IsNullOrEmpty(path))
                {
                    _tcs.TrySetResult(path);
                }
                else
                {
                    _tcs.TrySetException(
                        new Exception(
                            "Failed to resolve cropped image path."));
                }
            }
            else
            {
                _tcs.TrySetException(
                    result.Error ??
                    new Java.Lang.Exception(
                        "Crop operation failed."));
            }
        }
        else if (cropImageResult is CropImage.CancelledResult)
        {
            _tcs.TrySetCanceled();
        }
    }
}
