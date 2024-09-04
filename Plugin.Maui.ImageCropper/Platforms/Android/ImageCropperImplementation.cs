using AndroidX.Activity.Result;

using Com.Canhub.Cropper;

using Microsoft.Maui.LifecycleEvents;

namespace Plugin.Maui.ImageCropper;

public class ImageCropperImplementation : Java.Lang.Object, IImageCropper, IActivityResultCallback
{
    private ActivityResultLauncher _launcher;

    public ImageCropperImplementation(MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(l =>
            l.AddAndroid(ab =>
                ab.OnCreate((a, b) =>
                {
                    if (a is not MauiAppCompatActivity mauiActivity) return;

                    _launcher = mauiActivity.RegisterForActivityResult(new CropImageContract(), Cropper.Current as IActivityResultCallback);
                })));
    }

    private TaskCompletionSource<string> _tcs;

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        _tcs = new();

        try
        {
            var cropImageOptions = new CropImageOptions
            {
                OutputCompressFormat = Android.Graphics.Bitmap.CompressFormat.Png,
                ActivityBackgroundColor = Android.Graphics.Color.DarkGray,
                CropShape = settings.CropShape is CropSettings.CropShapeType.Oval
                    ? CropImageView.CropShape.Oval
                    : CropImageView.CropShape.Rectangle
            };

            if (cropImageOptions.FixAspectRatio = settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
            {
                cropImageOptions.AspectRatioX = settings.AspectRatioX;
                cropImageOptions.AspectRatioY = settings.AspectRatioY;
            }

            if (!string.IsNullOrWhiteSpace(settings.PageTitle))
            {
                cropImageOptions.ActivityTitle = new Java.Lang.String(settings.PageTitle);
            }

            _launcher.Launch(new CropImageContractOptions(Android.Net.Uri.FromFile(new Java.IO.File(imageFilePath)), cropImageOptions));
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public void OnActivityResult(Java.Lang.Object cropImageResult)
    {
        if (cropImageResult is CropImage.ActivityResult result)
        {
            if (result.IsSuccessful)
            {
                var path = result.GetUriFilePath(MauiApplication.Context, true);
                _tcs?.TrySetResult(path);
            }
            else
            {
                _tcs?.TrySetException(result.Error);
            }
        }
        else if (cropImageResult is CropImage.CancelledResult)
        {
            _tcs?.TrySetCanceled();
        }
    }
}