namespace EcoTrack.Views;

public partial class ActivitiesPage : ContentPage
{
	public ActivitiesPage()
	{
		InitializeComponent();
	}

    private async void AlRegistrar(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Formulario);
}