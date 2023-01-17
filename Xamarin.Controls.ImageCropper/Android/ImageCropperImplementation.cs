using System;
using System.Threading.Tasks;

using Android.App;

using AndroidX.Activity.Result;

using Com.Canhub.Cropper;

namespace Controls.ImageCropper
{
    public class ImageCropperImplementation : Java.Lang.Object, IImageCropper, IActivityResultCallback
    {
        private TaskCompletionSource<string> _tcs;

        public Task<string> Crop(CropSettings settings, string imageFilePath)
        {
            _tcs = new TaskCompletionSource<string>();

            try
            {
                var cropImageOptions = new CropImageOptions();

                cropImageOptions.OutputCompressFormat = Android.Graphics.Bitmap.CompressFormat.Png;
                cropImageOptions.ActivityBackgroundColor = Android.Graphics.Color.DarkGray;

                if (settings.CropShape == CropSettings.CropShapeType.Oval)
                {
                    cropImageOptions.CropShape = CropImageView.CropShape.Oval;
                }
                else
                {
                    cropImageOptions.CropShape = CropImageView.CropShape.Rectangle;
                }

                if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
                {
                    cropImageOptions.FixAspectRatio = true;
                    cropImageOptions.AspectRatioX = settings.AspectRatioX;
                    cropImageOptions.AspectRatioY = settings.AspectRatioY;
                }
                else
                {
                    cropImageOptions.FixAspectRatio = false;
                }

                if (!string.IsNullOrWhiteSpace(settings.PageTitle))
                {
                    cropImageOptions.ActivityTitle = new Java.Lang.String(settings.PageTitle);
                }

                CropperEvents.Instance.Failure = ex =>
                {
                    _tcs?.TrySetException(ex);
                };

                CropperEvents.Instance.Success = file =>
                {
                    _tcs?.TrySetResult(file);
                };

                CropperEvents.Instance.Cancel = () =>
                {
                    _tcs?.TrySetCanceled();
                };

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
                    CropperEvents.Instance.SetSuccess(path);
                }
                else
                {
                    CropperEvents.Instance.SetFailure(result.Error);
                }
            }
            else if (cropImageResult is CropImage.CancelledResult)
            {
                CropperEvents.Instance.SetCancel();
            }
        }
    }
}
