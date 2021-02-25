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

Don't forget to add this row into your AndroidManifest.xml file application tag:

    <activity android:name="com.theartofdev.edmodo.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />

### iOS:
No extra actions required

### Usage:

```csharp
await ImageCropper.Current.Crop(new CropSettings()
                {
                    AspectRatioX = 1,
                    AspectRatioY = 1,
                    CropShape = CropSettings.CropShapeType.Rectangle
                }, imageFilePath).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        var ex = t.Exception;
                        //alert user
                    }
                    else if (t.IsCanceled)
                    {
                        //do nothing
                    }
                    else if (t.IsCompletedSuccessfully)
                    {
                        var result = t.Result;
                        //do smth with result
                    }
                });
```

### Example of usage:
