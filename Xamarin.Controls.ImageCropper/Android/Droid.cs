using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;

using Com.Canhub.Cropper;

namespace Controls.ImageCropper.Platform;

public class Droid
{
    public static void Init(AppCompatActivity activity)
    {
        ImageCropperActivityResultLauncher = activity.RegisterForActivityResult(new CropImageContract(), (ImageCropper.Current as IActivityResultCallback)!);
    }

    public static ActivityResultLauncher? ImageCropperActivityResultLauncher { get; set; }
}