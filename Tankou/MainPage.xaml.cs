namespace Tankou
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        bool cameraStarted = false;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await StartCameraAsync();
        }

        async Task StartCameraAsync()
        {
            if (!cameraStarted)
            {

                PermissionStatus current = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (current != PermissionStatus.Granted)
                {
                    current = await Permissions.RequestAsync<Permissions.Camera>();
                    if (current != PermissionStatus.Granted)
                    {
                        statusLabel.IsVisible = true;
                        statusLabel.Text = $"Camera permission status: {current}";
                        return;
                    }

                }

                try
                {
                    cameraView.Start();
                }
                catch (Exception ex)
                {
                    statusLabel.IsVisible = true;
                    statusLabel.Text = $"{ex.Message}";
                    return;
                }

                cameraStarted = true;

                //await CameraView.StartCameraAsync();
            }
            else return;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (cameraStarted)
            {
                cameraView.Stop();
                cameraStarted = false;
            }
        }
    }
}
