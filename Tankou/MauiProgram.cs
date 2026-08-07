using CommunityToolkit.Maui;
using DrawnUi.Draw;
using Microsoft.Extensions.Logging;

namespace Tankou
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            }).UseMauiCommunityToolkitCamera();
            builder.Services.AddMauiBlazorWebView();
            builder.UseDrawnUi(); //initialize DrawnUI
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}