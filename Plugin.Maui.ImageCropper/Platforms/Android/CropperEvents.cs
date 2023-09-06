namespace Plugin.Maui.ImageCropper;

public class CropperEvents
{
    private static readonly Lazy<CropperEvents> _instance = new(() => new CropperEvents(), LazyThreadSafetyMode.PublicationOnly);

    public static CropperEvents Instance => _instance.Value;

    public Action<string> Success { get; set; }
    public Action<Exception> Failure { get; set; }
    public Action Cancel { get; set; }

    public void SetSuccess(string croppedFilePath)
    {
        Success?.Invoke(croppedFilePath);
    }

    public void SetFailure(Exception ex)
    {
        Failure?.Invoke(ex);
    }

    public void SetCancel()
    {
        Cancel?.Invoke();
    }
}