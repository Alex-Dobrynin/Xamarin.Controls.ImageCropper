using Android.App;
using Android.Content;

using TheArtOfDev.Edmodo.Cropper;

namespace Controls.ImageCropper.Platform
{
    public class Droid
    {
        public static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(intent);

                if (resultCode == Result.Ok)
                {
                    CropperEvents.Instance.SetSuccess(result.Uri.Path);
                }
                else if (resultCode == Result.Canceled)
                {
                    CropperEvents.Instance.SetCancel();
                }
                else if ((int)resultCode == CropImage.CropImageActivityResultErrorCode)
                {
                    CropperEvents.Instance.SetFailure(new System.Exception("Something went wrong while cropping an image"));
                }
            }
        }
    }
}
