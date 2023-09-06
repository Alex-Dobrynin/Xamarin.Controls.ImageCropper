using System;
using System.Linq;
using System.Threading.Tasks;

using Bind_TOCropViewController;

using CoreGraphics;

using Foundation;

using UIKit;

namespace Controls.ImageCropper;

public class ImageCropperImplementation : TOCropViewControllerDelegate, IImageCropper
{
    private TaskCompletionSource<string> _tcs;

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        _tcs = new TaskCompletionSource<string>();

        try
        {
            var image = UIImage.FromFile(imageFilePath);

            var cropViewController = settings.CropShape is CropSettings.CropShapeType.Oval
                ? new TOCropViewController(TOCropViewCroppingStyle.Circular, image)
                : new TOCropViewController(image);

            cropViewController.Title = settings.PageTitle;
            cropViewController.Delegate = this;

            if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
            {
                cropViewController.AspectRatioPreset = TOCropViewControllerAspectRatioPreset.Custom;
                cropViewController.ResetAspectRatioEnabled = false;
                cropViewController.AspectRatioLockEnabled = true;
                cropViewController.CustomAspectRatio = new CGSize(settings.AspectRatioX, settings.AspectRatioY);
            }

            var navController = new UINavigationController(cropViewController);

            var topVC = GetDefaultWindow().RootViewController;
            while (topVC.PresentedViewController != null)
            {
                topVC = topVC.PresentedViewController;
            }

            topVC.PresentViewController(navController, true, null);
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public override void DidCropToImage(TOCropViewController cropViewController, UIImage image, CGRect cropRect, nint angle)
    {
        cropViewController.DismissModalViewController(true);

        Finalize(image);
    }

    public override void DidCropToCircularImage(TOCropViewController cropViewController, UIImage image, CGRect cropRect, nint angle)
    {
        cropViewController.DismissModalViewController(true);

        Finalize(image);
    }

    public override void DidFinishCancelled(TOCropViewController cropViewController, bool cancelled)
    {
        cropViewController.DismissModalViewController(true);

        _tcs.SetCanceled();
    }

    private void Finalize(UIImage image)
    {
        string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string jpgFilename = System.IO.Path.Combine(documentsDirectory, $"cropped{DateTime.Now:yyyyMMddHHmmssfff}.jpg");
        var imgData = image.AsJPEG();

        if (imgData.Save(jpgFilename, false, out NSError err))
        {
            _tcs.SetResult(jpgFilename);
        }
        else
        {
            _tcs.SetException(new Exception(err.Description));
        }
    }

    public static UIWindow GetDefaultWindow()
    {
        UIWindow window = null;

        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
            {
                if (scene is UIWindowScene windowScene)
                {
                    window = windowScene.KeyWindow;

                    window ??= windowScene?.Windows?.LastOrDefault();
                }
            }
        }
        else
        {
            window = UIApplication.SharedApplication.Windows?.LastOrDefault();
        }

        return window;
    }
}