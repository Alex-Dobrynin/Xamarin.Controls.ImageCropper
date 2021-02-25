using System;
using System.Threading.Tasks;

using Plugin.CurrentActivity;

using TheArtOfDev.Edmodo.Cropper;

namespace Controls.ImageCropper
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ImageCropperImplementation : IImageCropper
    {
        private TaskCompletionSource<string> _tcs;

        public Task<string> Crop(CropSettings settings, string imageFilePath)
        {
            _tcs = new TaskCompletionSource<string>();

            try
            {
                CropImage.ActivityBuilder activityBuilder = CropImage.Activity(Android.Net.Uri.FromFile(new Java.IO.File(imageFilePath)));

                if (settings.CropShape == CropSettings.CropShapeType.Oval)
                {
                    activityBuilder.SetCropShape(CropImageView.CropShape.Oval);
                }
                else
                {
                    activityBuilder.SetCropShape(CropImageView.CropShape.Rectangle);
                }

                if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
                {
                    activityBuilder.SetFixAspectRatio(true);
                    activityBuilder.SetAspectRatio(settings.AspectRatioX, settings.AspectRatioY);
                }
                else
                {
                    activityBuilder.SetFixAspectRatio(false);
                }

                if (!string.IsNullOrWhiteSpace(settings.PageTitle))
                {
                    activityBuilder.SetActivityTitle(settings.PageTitle);
                }

                CropperEvents.Instance.Failure = ex =>
                {
                    _tcs.SetException(ex);
                };

                CropperEvents.Instance.Success = file =>
                {
                    _tcs.SetResult(file);
                };

                CropperEvents.Instance.Cancel = () =>
                {
                    _tcs.SetCanceled();
                };

                activityBuilder.Start(CrossCurrentActivity.Current.Activity);
            }
            catch (Exception ex)
            {
                _tcs.SetException(ex);
            }

            return _tcs.Task;
        }
    }
}
