using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.ImageCropper;

public partial class ImageCropperImplementation : IImageCropper
{
    public Task<string> Crop(CropSettings settings, string imageFilePath)
    {
        return Task.FromResult(string.Empty);
    }
}