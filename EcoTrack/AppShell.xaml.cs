using EcoTrack.Views;

namespace EcoTrack
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // El formulario se abre sobre la pestaña activa, no vive en el TabBar.
            Routing.RegisterRoute(Rutas.Formulario, typeof(ActivityFormPage));
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