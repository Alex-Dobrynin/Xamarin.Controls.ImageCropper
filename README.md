# MAUI / Xamarin ImageCropper
Simple crossplatform MAUI / Xamarin image cropper, you can use it in Xamarin.Forms, Xamarin native and MAUI. Also compatible with .NET 7

| Supported platforms | Nuget | Android | iOS | Windows | MacCatalyst |
|---------------------|---------------|---------|-----|---------|-------------|
| MAUI                | [![Nuget](https://img.shields.io/nuget/v/Plugin.Maui.ImageCropper)](https://www.nuget.org/packages/Plugin.Maui.ImageCropper)                 | :white_check_mark: | :white_check_mark: | :heavy_multiplication_x: | :heavy_multiplication_x: |
| Xamarin             | [![Nuget](https://img.shields.io/nuget/v/Xamarin.Controls.ImageCropper)](https://www.nuget.org/packages/Xamarin.Controls.ImageCropper/1.0.1) | :white_check_mark: | :white_check_mark: | :heavy_multiplication_x: | :heavy_multiplication_x: |
| .NET                | [![Nuget](https://img.shields.io/nuget/v/Xamarin.Controls.ImageCropper)](https://www.nuget.org/packages/Xamarin.Controls.ImageCropper)       | :white_check_mark: | :white_check_mark: | :heavy_multiplication_x: | :heavy_multiplication_x: |

## Powered By:

* Mono Android - uses ArturHub's [Android-Image-Cropper](https://github.com/ArthurHub/Android-Image-Cropper)
* .NET / MAUI Android - uses CanHub's [Android-Image-Cropper](https://github.com/CanHub/Android-Image-Cropper)
* iOS - uses uses Tim Oliver's [TOCropViewController](https://github.com/TimOliver/TOCropViewController)

### Another platforms:
Currently Windows and MacCatalyst are not supported, but your PRs are welcome

## Mono setup:
### Android:
It uses Plugin.CurrentActivity under the hood, so don't forget to initialize it in your MainActivity.cs's OnCreate method:

    Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

Also, you need to add OnActivityResult handler:

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
    {
        base.OnActivityResult(requestCode, resultCode, intent);

        Plugin.Maui.ImageCropper.Platform.Droid.OnActivityResult(requestCode, resultCode, intent);
    }

Don't forget to add this row into your AndroidManifest.xml file application tag:

    <activity android:name="com.theartofdev.edmodo.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />

### iOS:
No extra actions required

## .NET setup:
### Android:
All you need is to initialize it in your MainActivity.cs's OnCreate method:

    Plugin.Maui.ImageCropper.Platform.Droid.Init(this);

Don't forget to add this row into your AndroidManifest.xml file application tag:

    <activity android:name="com.canhub.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />

### iOS:
No extra actions required

## MAUI setup:
To use it in MAUI you should call ```UseImageCropper()``` method from ```Plugin.Maui.ImageCropper``` namespace.

Also there is another overload which allows you to register ImageCropper instance in services and then use it with DI

```csharp
UseImageCropper(registerInterface: true)
```

### Android:
Don't forget to add this row into your AndroidManifest.xml file application tag:

    <activity android:name="com.canhub.cropper.CropImageActivity" android:theme="@style/Base.Theme.AppCompat" />

### iOS:
No extra actions required

## Example of usage:

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
