namespace EcoTrack.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
	}

    private async void AlRegistrar(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Formulario);

    private async void AlVerHistorial(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Actividades);
}