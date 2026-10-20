using EcoTrack.Services;
using EcoTrack.Views;

namespace EcoTrack
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _auth;

        public AppShell(IAuthService auth)
        {
            InitializeComponent();
            _auth = auth;

            // El formulario se abre sobre la pantalla actual, no vive en el menú.
            Routing.RegisterRoute(Rutas.Formulario, typeof(ActivityFormPage));
        }

        private async void AlRegistrar(object? sender, EventArgs e)
        {
            FlyoutIsPresented = false;

            // Evita abrir dos formularios apilados si ya estás en uno.
            if (CurrentState.Location.OriginalString.Contains(Rutas.Formulario))
            {
                return;
            }

            await GoToAsync(Rutas.Formulario);
        }

        private async void AlCerrarSesion(object? sender, EventArgs e)
        {
            FlyoutIsPresented = false;

            var confirmar = await DisplayAlert("Cerrar sesión", "¿Quieres salir de tu perfil?", "Cerrar sesión", "Cancelar");
            if (!confirmar)
            {
                return;
            }

            _auth.CerrarSesion();
            FlyoutBehavior = FlyoutBehavior.Disabled;   // el menú vuelve a desaparecer
            await GoToAsync(Rutas.Login);
        }
    }
}

/// <summary>Rutas de navegación usadas por el Shell.</summary>
public static class Rutas
{
    public const string Login = "//login";
    public const string Inicio = "//inicio";
    public const string Actividades = "//actividades";
    public const string Formulario = "formulario";
}