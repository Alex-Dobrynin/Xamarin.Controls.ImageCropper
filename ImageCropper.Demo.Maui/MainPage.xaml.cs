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
            string deletefoto = User_photo.Source != null ? "Delete a photo" : null;
            string tempresult = await Shell.Current.DisplayActionSheetAsync("Select a Photo to Crop", "Cancel", null, "Capture Photo", "Choose Photo", deletefoto);

            if (!string.IsNullOrEmpty(tempresult) && tempresult != "Cancel")
            {
                try
                {
                    IsProgress.IsRunning = true;
                    IsProgress.IsVisible = true;

                    FileResult photo = null;

                    if (tempresult == "Choose Photo")
                    {
                        List<FileResult> photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
                        {
                            SelectionLimit = 1,
                            RotateImage = true,
                            PreserveMetaData = true,
                            Title = "Choose Photo"
                        });

                        photo = photos.FirstOrDefault();
                    }

                    else if (tempresult == "Capture Photo" && MediaPicker.Default.IsCaptureSupported)
                    {
                        photo = await MediaPicker.Default.CapturePhotoAsync();
                    }

                    else if (tempresult == "Delete a photo")
                    {
                        User_photo.Source = null;
                        return;
                    }

                    if (photo != null)
                    {
                        string extension = Path.GetExtension(photo.FileName);
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}{extension}");
                        using (Stream sourceStream = await photo.OpenReadAsync())
                        using (FileStream localFileStream = File.Create(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                            await localFileStream.FlushAsync();
                        }

                        //Delay to close Mediapicker's view
                        await Task.Delay(1000);

                        string croppedPath = await Cropper.Current.Crop(new CropSettings()
                        {
                            AspectRatioX = 1,
                            AspectRatioY = 1,
                            CropShape = CropSettings.CropShapeType.Oval,
                            PageTitle = "Profile Photo",
#if IOS
                            CancelButtonTitle = "Cancel",
                            DoneButtonTitle = "Done",
#elif ANDROID
                            CropLabelText = "Crop",
#endif
                        }, localFilePath);

                        if (!string.IsNullOrEmpty(croppedPath))
                        {
                            if (File.Exists(localFilePath))
                            {
                                File.Delete(localFilePath);
                            }

                            User_photo.Source = ImageSource.FromFile(croppedPath);
                        }
                    }
                }

                catch (TaskCanceledException)
                {
                    // Task was canceled
                }

                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error:", ex.Message, "OK");
                }

                finally
                {
                    IsProgress.IsRunning = false;
                    IsProgress.IsVisible = false;
                }
            }
        }
    }
}