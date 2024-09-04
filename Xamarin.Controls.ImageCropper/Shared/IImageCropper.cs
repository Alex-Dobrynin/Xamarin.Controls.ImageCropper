using System.Threading.Tasks;

namespace Controls.ImageCropper;

public interface IImageCropper
{
    Task<string> Crop(CropSettings settings, string imageFilePath);
}