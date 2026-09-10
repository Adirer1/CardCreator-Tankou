

namespace Tankou
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            cameraView.OnError += OnCameraError;
            cameraView.PermissionsResult += OnPermissionsResult;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            cameraView.IsOn = true;
        }

        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            cameraView.IsOn = false;
        }

        void OnCameraError(object? sender, string message)
        {
          ShowStatusLabel(message);
        }

        void OnPermissionsResult(object? sender, bool granted)
        {
            if (!granted)
            {
                ShowStatusLabel("Camera permission denied. Please enable it in settings.");
            }
        }

        void ShowStatusLabel(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel.Text = message;
                statusLabel.IsVisible = true;
            });
        }
    }
}
