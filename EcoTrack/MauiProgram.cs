using EcoTrack.Services;
using EcoTrack.Views;
using Microsoft.Extensions.Logging;

namespace EcoTrack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>();

            // Servicios: una sola instancia para toda la app, así los datos se comparten entre pantallas.
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IRegistroService, RegistroService>();

            // Shell y páginas: el contenedor les inyecta los servicios por constructor.
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ActivitiesPage>();
            builder.Services.AddTransient<ActivityFormPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
