using EcoTrack.Services;

namespace EcoTrack.Views;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _auth;

    public LoginPage(IAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    // Cada vez que aparece el login (al abrir la app o tras cerrar sesión) arranca limpio.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CorreoEntry.Text = string.Empty;
        ClaveEntry.Text = string.Empty;
        ClaveEntry.IsPassword = true;
        MostrarButton.Text = "Mostrar";
        ErrorLabel.IsVisible = false;
    }

    private async void AlEntrar(object? sender, EventArgs e)
    {
        var correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        var clave = ClaveEntry.Text ?? string.Empty;

        // 1) Campos obligatorios
        if (correo.Length == 0 || clave.Length == 0)
        {
            MostrarError("Escribe tu correo y tu contraseña.");
            return;
        }

        // 2) Formato mínimo del correo
        if (!correo.Contains('@'))
        {
            MostrarError("Escribe un correo válido.");
            return;
        }

        // 3) Credenciales fijas (las valida AuthService)
        if (!_auth.IniciarSesion(correo, clave))
        {
            MostrarError("Correo o contraseña incorrectos.");
            ClaveEntry.Text = string.Empty;
            return;
        }

        // Login correcto: se activa el menú lateral y se entra a la pantalla principal.
        ErrorLabel.IsVisible = false;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        await Shell.Current.GoToAsync(Rutas.Inicio);
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }

    private void AlMostrarClave(object? sender, EventArgs e)
    {
        ClaveEntry.IsPassword = !ClaveEntry.IsPassword;
        MostrarButton.Text = ClaveEntry.IsPassword ? "Mostrar" : "Ocultar";
    }

    private async void AlOlvideClave(object? sender, EventArgs e)
        => await DisplayAlertAsync("Recuperar contraseña", "Esta opción se habilitará cuando la app tenga base de datos de usuarios.", "OK");

    private async void AlCrearPerfil(object? sender, EventArgs e)
        => await DisplayAlertAsync("Crear perfil", "Por ahora la app usa un único perfil con credenciales fijas.", "OK");
}