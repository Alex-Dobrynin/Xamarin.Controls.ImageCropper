using Bind_TOCropViewController;

using CoreGraphics;

using Foundation;

using UIKit;

namespace Plugin.Maui.ImageCropper;

public partial class ImageCropperImplementation
    : TOCropViewControllerDelegate, IImageCropper
{
    private TaskCompletionSource<string>? _tcs;

    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        if (_tcs?.Task.IsCompleted is false)
        {
            throw new InvalidOperationException("Crop operation already in progress.");
        }

        _tcs = new();

        try
        {
            var image = UIImage.FromFile(imageFilePath)
                ?? throw new FileNotFoundException("Unable to load image.", imageFilePath);

            var cropViewController = settings.CropShape == CropSettings.CropShapeType.Oval
                ? new TOCropViewController(TOCropViewCroppingStyle.Circular, image)
                : new TOCropViewController(image);

            cropViewController.Delegate = this;

            if (!string.IsNullOrWhiteSpace(settings.PageTitle))
                cropViewController.Title = settings.PageTitle;

            if (!string.IsNullOrWhiteSpace(settings.CancelButtonTitle))
                cropViewController.CancelButtonTitle = settings.CancelButtonTitle;

            if (!string.IsNullOrWhiteSpace(settings.DoneButtonTitle))
                cropViewController.DoneButtonTitle = settings.DoneButtonTitle;

            if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
            {
                cropViewController.AspectRatioPreset =
                    TOCropViewControllerAspectRatioPreset.Custom;

                cropViewController.CustomAspectRatio =
                    new CGSize(settings.AspectRatioX, settings.AspectRatioY);

                cropViewController.ResetAspectRatioEnabled = false;
                cropViewController.AspectRatioLockEnabled = true;
            }

            var navController = new UINavigationController(cropViewController);

            var topVC = Platform.GetCurrentUIViewController()
                ?? throw new InvalidOperationException("Unable to get current UIViewController.");

            topVC.PresentViewController(navController, true, null);
        }
        catch (Exception ex)
        {
            _tcs.SetException(ex);
        }

        return _tcs.Task;
    }

    public override async void DidCropToImage(
        TOCropViewController cropViewController, UIImage image,
        CGRect cropRect, nint angle)
    {
        try
        {
            await cropViewController.DismissViewControllerAsync(true);

            FinalizeImage(image, CropSettings.CropShapeType.Rectangle);
        }
        catch (Exception ex)
        {
            _tcs?.SetException(ex);
        }
    }

    public override async void DidCropToCircularImage(
        TOCropViewController cropViewController, UIImage image,
        CGRect cropRect, nint angle)
    {
        try
        {
            await cropViewController.DismissViewControllerAsync(true);

            FinalizeImage(image, CropSettings.CropShapeType.Oval);
        }
        catch (Exception ex)
        {
            _tcs?.SetException(ex);
        }
    }

    public override async void DidFinishCancelled(
        TOCropViewController cropViewController, bool cancelled)
    {
        try
        {
            await cropViewController.DismissViewControllerAsync(true);

            _tcs?.SetCanceled();
        }
        catch (Exception ex)
        {
            _tcs?.SetException(ex);
        }
    }

    private void FinalizeImage(UIImage image, CropSettings.CropShapeType cropShape)
    {
        var documentsDirectory = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments);

        var extension = cropShape == CropSettings.CropShapeType.Oval
            ? "png"
            : "jpg";

        var filePath = Path.Combine(documentsDirectory,
            $"cropped_{DateTime.UtcNow:yyyyMMddHHmmssfff}.{extension}");

        var imageData = cropShape == CropSettings.CropShapeType.Oval
            ? image.AsPNG()!
            : image.AsJPEG()!;

        if (imageData.Save(filePath, false, out NSError? error))
        {
            _tcs?.SetResult(filePath);
        }
        else
        {
            _tcs?.SetException(new Exception(error?.LocalizedDescription));
        }
    }
}
