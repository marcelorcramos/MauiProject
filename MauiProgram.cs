using Microsoft.Extensions.Logging;
using Maui_1.ViewModel;
using CommunityToolkit.Maui;

namespace Maui_1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() 
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IMC_VM>();
            builder.Services.AddSingleton<DataContext>();
            builder.Services.AddSingleton<PessoaVM>();
            return builder.Build();
        }
    }
}