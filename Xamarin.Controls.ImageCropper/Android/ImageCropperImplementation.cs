using System;
using System.Threading.Tasks;

using AndroidX.Activity.Result;

using Com.Canhub.Cropper;

using Controls.ImageCropper.Platform;

namespace Controls.ImageCropper;

public class ImageCropperImplementation
    : Java.Lang.Object, IImageCropper, IActivityResultCallback
{
    private TaskCompletionSource<string>? _tcs;

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        if (_tcs?.Task.IsCompleted is false)
        {
            throw new InvalidOperationException("Crop operation already in progress.");
        }

        if (Droid.ImageCropperActivityResultLauncher is null)
        {
            throw new Exception("You must call Controls.ImageCropper.Platform.Droid.Init(activity) in the OnCreate method of your activity");
        }

        _tcs = new();

        try
        {
            var fixAspect = settings.AspectRatioX > 0 && settings.AspectRatioY > 0;

            var cropImageOptions = new CropImageOptions
            {
                CropperLabelText = settings.CropLabelText,
                ActivityBackgroundColor = Android.Graphics.Color.DarkGray,

                CropShape = settings.CropShape == CropSettings.CropShapeType.Oval
                    ? CropImageView.CropShape.Oval!
                    : CropImageView.CropShape.Rectangle!,

                OutputCompressFormat = settings.CropShape == CropSettings.CropShapeType.Oval
                    ? Android.Graphics.Bitmap.CompressFormat.Png!
                    : Android.Graphics.Bitmap.CompressFormat.Jpeg!,

                FixAspectRatio = fixAspect
            };

            if (fixAspect)
            {
                cropImageOptions.AspectRatioX = settings.AspectRatioX;
                cropImageOptions.AspectRatioY = settings.AspectRatioY;
            }

            if (!string.IsNullOrWhiteSpace(settings.PageTitle))
            {
                cropImageOptions.ActivityTitle = new Java.Lang.String(settings.PageTitle);
            }

            Droid.ImageCropperActivityResultLauncher.Launch(
                new CropImageContractOptions(Android.Net.Uri.FromFile(new Java.IO.File(imageFilePath)),
                cropImageOptions));
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public void OnActivityResult(Java.Lang.Object? cropImageResult)
    {
        if (_tcs?.Task.IsCompleted is not false) return;

        if (cropImageResult is CropImage.CancelledResult)
        {
            _tcs.TrySetCanceled();
            return;
        }

        if (cropImageResult is not CropImage.ActivityResult result) return;

        if (!result.IsSuccessful)
        {
            _tcs.TrySetException(result.Error
                ?? new Java.Lang.Exception("Crop operation failed."));
            return;
        }

        var path = result.GetUriFilePath(Android.App.Application.Context, true);

        if (!string.IsNullOrEmpty(path))
        {
            _tcs.TrySetResult(path);
        }
        else
        {
            _tcs.TrySetException(new Exception("Failed to resolve cropped image path."));
        }
    }
}