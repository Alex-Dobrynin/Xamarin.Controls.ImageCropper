using System;
using System.Threading.Tasks;

using Android.App;

using AndroidX.Activity.Result;

using Com.Canhub.Cropper;

namespace Controls.ImageCropper;

public class ImageCropperImplementation : Java.Lang.Object, IImageCropper, IActivityResultCallback
{
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

            if (Platform.Droid.ImageCropperActivityResultLauncher is null)
            {
                throw new Exception("You must call Controls.ImageCropper.Platform.Droid.Init(activity) in the OnCreate method of your activity");
            }

            Platform.Droid.ImageCropperActivityResultLauncher.Launch(new CropImageContractOptions(Android.Net.Uri.FromFile(new Java.IO.File(imageFilePath)), cropImageOptions));
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
                var path = result.GetUriFilePath(Application.Context, true);
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