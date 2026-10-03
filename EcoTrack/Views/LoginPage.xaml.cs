namespace EcoTrack.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    // Solamente la Navegación.
    private async void AlEntrar(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Inicio);
}