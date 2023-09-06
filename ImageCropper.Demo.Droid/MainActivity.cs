using System;
using System.Threading.Tasks;

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using Controls.ImageCropper;

using Java.IO;

namespace ImageCropper.Demo.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Controls.ImageCropper.Platform.Droid.Init(this);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var btn = FindViewById<Button>(Resource.Id.btn);
            btn.Click += Btn_Click;
        }

        private async void Btn_Click(object? sender, EventArgs e)
        {
            var rs = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();

            //Delay to close Mediapicker's view
            await Task.Delay(1000);

            var path = await Controls.ImageCropper.ImageCropper.Current.Crop(new CropSettings()
            {
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = CropSettings.CropShapeType.Rectangle
            }, rs.FullPath);

            File imgFile = new File(path);

            if (imgFile.Exists())
            {
                Bitmap myBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);

                var myImage = new ImageView(this);

                myImage.SetImageBitmap(myBitmap);

                var layout = this.Window.DecorView as ViewGroup;

                layout.AddView(myImage);
            }
        }
    }
}