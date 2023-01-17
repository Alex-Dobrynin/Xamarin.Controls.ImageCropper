# Xamarin.Controls.ImageCropper
Simple crossplatform xamarin image cropper, you can use it in both Xamarin.Forms and Xamarin native. Also compatible with .NET 7

For mono use v1.0.1, for .NET use v1.1.0

For Mono Android it uses 

https://github.com/ArthurHub/Android-Image-Cropper for Android, supports Android 12

For .NET Android it uses 

https://github.com/CanHub/Android-Image-Cropper for Android, supports Android 13

For iOS it uses

https://github.com/TimOliver/TOCropViewController for iOS


## Initialize:

### Android Mono:
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

### iOS Mono:
No extra actions required

### Android .NET:
All you need is to initialize it in your MainActivity.cs's OnCreate method:

    Controls.ImageCropper.Platform.Droid.Init(this);

Don't forget to add this row into your AndroidManifest.xml file application tag:

    <activity android:name="com.theartofdev.edmodo.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />

### iOS .NET:
No extra actions required

### Example of usage:

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
