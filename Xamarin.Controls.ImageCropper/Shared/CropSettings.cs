using System.Runtime.Versioning;

namespace Controls.ImageCropper;

public class CropSettings
{
    public enum CropShapeType
    {
        Rectangle,
        Oval
    };

    public CropShapeType CropShape { get; set; } = CropShapeType.Rectangle;
    public int AspectRatioX { get; set; } = 0;
    public int AspectRatioY { get; set; } = 0;
    public string? PageTitle { get; set; }

    [SupportedOSPlatform("ios")]
    public string? CancelButtonTitle { get; set; }
    [SupportedOSPlatform("ios")]
    public string? DoneButtonTitle { get; set; }
    [SupportedOSPlatform("android")]
    public string? CropLabelText { get; set; }
}