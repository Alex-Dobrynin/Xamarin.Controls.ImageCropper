using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Bind_TOCropViewController;

using CoreGraphics;

using Foundation;

using UIKit;

namespace Controls.ImageCropper;

public class ImageCropperImplementation : TOCropViewControllerDelegate, IImageCropper
{
    private TaskCompletionSource<string>? _tcs;

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        _tcs = new();

        try
        {
            var image = UIImage.FromFile(imageFilePath)!;

            var cropViewController = settings.CropShape is CropSettings.CropShapeType.Oval
                ? new TOCropViewController(TOCropViewCroppingStyle.Circular, image)
                : new TOCropViewController(image);

            cropViewController.Title = settings.PageTitle;
            cropViewController.Delegate = this;
            cropViewController.CancelButtonTitle = settings.CancelTitle;
            cropViewController.DoneButtonTitle = settings.DoneTitle;

            if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
            {
                cropViewController.AspectRatioPreset = TOCropViewControllerAspectRatioPreset.Custom;
                cropViewController.ResetAspectRatioEnabled = false;
                cropViewController.AspectRatioLockEnabled = true;
                cropViewController.CustomAspectRatio = new CGSize(settings.AspectRatioX, settings.AspectRatioY);
            }

            var navController = new UINavigationController(cropViewController);

            var topVC = GetKeyWindow()?.RootViewController;
            while (topVC?.PresentedViewController != null)
            {
                topVC = topVC.PresentedViewController;
            }

            topVC?.PresentViewController(navController, true, null);
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public override async void DidCropToImage(TOCropViewController cropViewController, UIImage image, CGRect cropRect, nint angle)
    {
        await cropViewController.DismissViewControllerAsync(true);

        Finalize(image);
    }

    public override async void DidCropToCircularImage(TOCropViewController cropViewController, UIImage image, CGRect cropRect, nint angle)
    {
        await cropViewController.DismissViewControllerAsync(true);

        Finalize(image);
    }

    public override async void DidFinishCancelled(TOCropViewController cropViewController, bool cancelled)
    {
        await cropViewController.DismissViewControllerAsync(true);

        _tcs!.SetCanceled();
    }

    private void Finalize(UIImage image)
    {
        string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string jpgFilename = Path.Combine(documentsDirectory, $"cropped_{DateTime.Now:yyyyMMddHHmmssfff}.jpg");
        var imgData = image.AsJPEG()!;

        if (imgData.Save(jpgFilename, false, out NSError? err))
        {
            _tcs!.SetResult(jpgFilename);
        }
        else
        {
            _tcs!.SetException(new Exception(err?.Description));
        }
    }

    public static UIWindow? GetKeyWindow()
    {
        var window = GetActiveScene()?.Windows.FirstOrDefault(w => w.IsKeyWindow);

        return window;
    }

    public static UIWindowScene? GetActiveScene()
    {
        var connectedScene = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault(x => x.ActivationState == UISceneActivationState.ForegroundActive);

        return connectedScene;
    }
}