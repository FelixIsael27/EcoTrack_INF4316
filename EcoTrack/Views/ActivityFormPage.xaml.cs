using EcoTrack.Helpers;
using EcoTrack.Models;
using EcoTrack.Services;
using EcoTrack.ViewsModels;
using Microsoft.Maui.Media;
using System.Globalization;

namespace EcoTrack.Views;

public partial class ActivityFormPage : ContentPage
{
    // Toda la lógica vive en el ViewModel: aquí solo se conecta con la vista.
    public ActivityFormPage(ActivityFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}