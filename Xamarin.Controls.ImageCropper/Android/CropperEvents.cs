using System;

namespace Controls.ImageCropper
{
    public class CropperEvents
    {
        private static Lazy<CropperEvents> _instance = new Lazy<CropperEvents>(() => new CropperEvents(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static CropperEvents Instance
        {
            get
            {
                return _instance.Value;
            }
        }

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
}
