using EcoTrack.Helpers;
using EcoTrack.Models;
using EcoTrack.Services;

namespace EcoTrack.Views;

public partial class ActivitiesPage : ContentPage
{
    private readonly IRegistroService _registros;
    private readonly Dictionary<string, Border> _chips;

    private string _filtro = "Todas";
    private string _texto = string.Empty;

    public ActivitiesPage(IRegistroService registros)
    {
        InitializeComponent();
        _registros = registros;

        _chips = new Dictionary<string, Border>
        {
            ["Todas"] = ChipTodas,
            ["Transporte"] = ChipTransporte,
            ["Energia"] = ChipEnergia,
            ["Residuos"] = ChipResiduos,
            ["Agua"] = ChipAgua
        };

        // Si se agrega un registro, la lista se actualiza sola.
        _registros.RegistrosCambiaron += Refrescar;
        Refrescar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refrescar();
    }

    private void Refrescar()
    {
        // Chip activo resaltado, los demás en estilo normal
        foreach (var (clave, chip) in _chips)
        {
            var activo = clave == _filtro;
            chip.Style = Tema.Estilo(activo ? "ChipFiltroActivo" : "ChipFiltro");
            if (chip.Content is Label etiqueta)
                etiqueta.Style = Tema.Estilo(activo ? "ChipTextoActivo" : "ChipTexto");
        }

        // Filtro por categoría + búsqueda por texto
        var todos = _registros.ObtenerTodos();
        IEnumerable<RegistroEco> datos = todos;

        if (Enum.TryParse<CategoriaEco>(_filtro, out var categoria))
            datos = datos.Where(r => r.Categoria == categoria);

        var texto = _texto.Trim();
        if (texto.Length > 0)
            datos = datos.Where(r => r.Descripcion.Contains(texto, StringComparison.OrdinalIgnoreCase));

        ListaCollection.ItemsSource = datos.Select(RegistroVista.Desde).ToList();

        // Resumen y mensaje de lista vacía
        var hayDatos = todos.Count > 0;
        ResumenLabel.Text = hayDatos
            ? $"{todos.Count} {(todos.Count == 1 ? "registro" : "registros")} · {Formato.Co2(todos.Sum(r => r.CO2EvitadoKg))} kg de CO₂ evitados"
            : "Aún no has registrado actividades";

        VacioTitulo.Text = hayDatos ? "Sin resultados" : "Aún no tienes actividades";
        VacioDetalle.Text = hayDatos
            ? "Prueba con otra categoría o con otra palabra."
            : "Cuando registres una, aparecerá aquí.";
    }

    private void AlElegirFiltro(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string clave && _chips.ContainsKey(clave))
        {
            _filtro = clave;
            Refrescar();
        }
    }

    private void AlBuscar(object? sender, TextChangedEventArgs e)
    {
        _texto = e.NewTextValue ?? string.Empty;
        Refrescar();
    }

    private void AlAbrirMenu(object? sender, EventArgs e)
        => Shell.Current.FlyoutIsPresented = true;

    private async void AlRegistrar(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Formulario);
}