namespace Plugin.Maui.ImageCropper;

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
    public string? PageTitle { get; set; } = null;

    /// <summary>
    /// iOS only
    /// </summary>
    public string? CancelTitle { get; set; } = null;

    /// <summary>
    /// iOS only
    /// </summary>
    public string? DoneTitle { get; set; } = null;

    /// <summary>
    /// Android only
    /// </summary>
    public string? CropLabelText { get; set; } = null;
}