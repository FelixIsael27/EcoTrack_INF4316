namespace EcoTrack.Views;

public partial class ActivityFormPage : ContentPage
{
	public ActivityFormPage()
	{
		InitializeComponent();
	}

    private async void AlCerrar(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}