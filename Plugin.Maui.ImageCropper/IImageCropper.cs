namespace Plugin.Maui.ImageCropper;

public interface IImageCropper
{
    Task<string> Crop(CropSettings settings, string imageFilePath);
}