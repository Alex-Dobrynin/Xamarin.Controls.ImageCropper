using Plugin.Maui.ImageCropper;

namespace ImageCropper.Demo.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void CropBtn_Clicked(object sender, EventArgs e)
        {
            var rs = await MediaPicker.PickPhotoAsync();

            //Delay to close Mediapicker's view
            await Task.Delay(1000);

            var path = await Cropper.Current.Crop(new CropSettings()
            {
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = CropSettings.CropShapeType.Rectangle,
                PageTitle = "Crop image"
            }, rs.FullPath);

            if (string.IsNullOrWhiteSpace(path)) return;

            Img.Source = path;
        }
    }
}