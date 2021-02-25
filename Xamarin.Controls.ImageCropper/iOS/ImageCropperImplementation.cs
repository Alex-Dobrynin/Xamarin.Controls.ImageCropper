using System;
using System.Threading.Tasks;

using CoreGraphics;

using Foundation;

using TimOliver.TOCropViewController.Xamarin;

using UIKit;

namespace Controls.ImageCropper
{
    public class ImageCropperImplementation : TOCropViewControllerDelegate, IImageCropper
    {
        private TaskCompletionSource<string> _tcs;

        public Task<string> ShowFromFile(CropSettings settings, string imageFilePath)
        {
            _tcs = new TaskCompletionSource<string>();

            try
            {
                UIImage image = UIImage.FromFile(imageFilePath);

                TOCropViewController cropViewController;

                if (settings.CropShape == CropSettings.CropShapeType.Oval)
                {
                    cropViewController = new TOCropViewController(TOCropViewCroppingStyle.Circular, image);
                }
                else
                {
                    cropViewController = new TOCropViewController(image);
                }

                if (settings.AspectRatioX > 0 && settings.AspectRatioY > 0)
                {
                    cropViewController.AspectRatioPreset = TOCropViewControllerAspectRatioPreset.Custom;
                    cropViewController.ResetAspectRatioEnabled = false;
                    cropViewController.AspectRatioLockEnabled = true;
                    cropViewController.CustomAspectRatio = new CGSize(settings.AspectRatioX, settings.AspectRatioY);
                }

                cropViewController.Title = settings.PageTitle;
                cropViewController.Delegate = this;

                var navController = new UINavigationController(cropViewController);

                var topVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
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
            NSData imgData = image.AsJPEG();
            NSError err;

            if (imgData.Save(jpgFilename, false, out err))
            {
                _tcs.SetResult(jpgFilename);
            }
            else
            {
                _tcs.SetException(new Exception(err.Description));
            }
        }
    }
}
