# Xamarin.Controls.TabView
Simple crossplatform xamarin image cropper

It uses 

https://github.com/ArthurHub/Android-Image-Cropper for Android, supports Android 11
https://github.com/TimOliver/TOCropViewController for iOS

The nuget package: https://www.nuget.org/packages/Xamarin.Controls.ImageCropper

## Initialize:

### Android:
It uses Plugin.CurrentActivity under the hood, so don't forget to initialize it in your MainActivity.cs's OnCreate method:

    Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

Also, you need to add OnActivityResult handler:

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
    {
        base.OnActivityResult(requestCode, resultCode, intent);

        Controls.ImageCropper.Platform.Droid.OnActivityResult(requestCode, resultCode, intent);
    }

### iOS:
No extra actions required

### Usage:

### Example of usage:
