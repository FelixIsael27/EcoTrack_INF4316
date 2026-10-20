using EcoTrack.Helpers;
using EcoTrack.Models;
using EcoTrack.Services;

namespace EcoTrack.Views;

public partial class DashboardPage : ContentPage
{
    private static readonly string[] Consejos =
    {
        "Lavar la ropa con agua fría ahorra energía: la mayor parte del consumo de una lavadora se va en calentar el agua.",
        "Desconecta los cargadores y los aparatos en stand-by: siguen consumiendo energía aunque estén apagados.",
        "Para trayectos cortos, caminar o usar bicicleta evita emisiones y además mejora tu salud.",
        "Separa tus residuos en casa: un material limpio y seco es mucho más fácil de reciclar.",
        "Una ducha más corta ahorra agua y también la energía que se usa para calentarla.",
        "Compartir el carro reparte las emisiones del trayecto entre todas las personas que van dentro.",
        "Aprovecha la luz natural durante el día y apaga las luces de los cuartos vacíos."
    };

    private readonly IAuthService _auth;
    private readonly IRegistroService _registros;
    private readonly BoxView[] _barras;
    private readonly Label[] _dias;

    public DashboardPage(IAuthService auth, IRegistroService registros)
    {
        InitializeComponent();
        _auth = auth;
        _registros = registros;

        _barras = new[] { Barra0, Barra1, Barra2, Barra3, Barra4, Barra5, Barra6 };
        _dias = new[] { Dia0, Dia1, Dia2, Dia3, Dia4, Dia5, Dia6 };

        // Si se agrega un registro, el resumen se recalcula solo.
        _registros.RegistrosCambiaron += Refrescar;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refrescar();
    }

    private void Refrescar()
    {
        var hoy = DateTime.Today;

        // Saludo
        var nombre = string.IsNullOrEmpty(_auth.NombreUsuario) ? "Eco" : _auth.NombreUsuario;
        SaludoLabel.Text = $"Hola, {nombre}";
        AvatarLabel.Text = nombre[..1].ToUpperInvariant();
        FechaLabel.Text = Formato.FechaLarga(hoy);
        ConsejoLabel.Text = Consejos[hoy.DayOfYear % Consejos.Length];

        // La semana va de lunes a domingo
        var lunes = hoy.AddDays(-(((int)hoy.DayOfWeek + 6) % 7));
        var domingo = lunes.AddDays(6);

        var todos = _registros.ObtenerTodos();
        var semana = todos.Where(r => r.Fecha.Date >= lunes && r.Fecha.Date <= domingo).ToList();
        var semanaAnterior = todos.Where(r => r.Fecha.Date >= lunes.AddDays(-7) && r.Fecha.Date < lunes).ToList();

        // Total de la semana y comparación con la anterior
        var totalSemana = semana.Sum(r => r.CO2EvitadoKg);
        var totalAnterior = semanaAnterior.Sum(r => r.CO2EvitadoKg);
        TotalSemanaLabel.Text = Formato.Co2(totalSemana);

        if (totalSemana == 0 && totalAnterior == 0)
            ComparacionLabel.Text = "Registra tu primera actividad para empezar a sumar.";
        else if (totalSemana > totalAnterior)
            ComparacionLabel.Text = $"{Formato.Co2(totalSemana - totalAnterior)} kg más que la semana pasada.";
        else if (totalSemana < totalAnterior)
            ComparacionLabel.Text = $"{Formato.Co2(totalAnterior - totalSemana)} kg menos que la semana pasada.";
        else
            ComparacionLabel.Text = "Igual que la semana pasada.";

        // Barras: la altura de cada día es proporcional al mejor día de la semana
        var porDia = Enumerable.Range(0, 7)
            .Select(i => semana.Where(r => r.Fecha.Date == lunes.AddDays(i)).Sum(r => r.CO2EvitadoKg))
            .ToArray();
        var maximo = porDia.Max();
        var indiceHoy = (int)(hoy - lunes).TotalDays;

        for (var i = 0; i < 7; i++)
        {
            var esHoy = i == indiceHoy;
            _barras[i].HeightRequest = maximo > 0 ? Math.Max(6, 64 * porDia[i] / maximo) : 6;
            _barras[i].Color = esHoy ? Tema.ColorDe("Lima") : Color.FromArgb("#3C6A55");
            _dias[i].TextColor = esHoy ? Tema.ColorDe("Lima") : Color.FromArgb("#A9C3B4");
        }

        // Cantidades de la semana por categoría
        double Cantidad(CategoriaEco categoria) => semana.Where(r => r.Categoria == categoria).Sum(r => r.Cantidad);

        TransporteLabel.Text = Formato.Numero(Cantidad(CategoriaEco.Transporte));
        EnergiaLabel.Text = Formato.Numero(Cantidad(CategoriaEco.Energia));
        ResiduosLabel.Text = Formato.Numero(Cantidad(CategoriaEco.Residuos));
        AguaLabel.Text = Formato.Numero(Cantidad(CategoriaEco.Agua));

        // Los 3 registros más recientes
        BindableLayout.SetItemsSource(UltimosLayout, todos.Take(3).Select(RegistroVista.Desde).ToList());
    }

    private void AlAbrirMenu(object? sender, EventArgs e)
        => Shell.Current.FlyoutIsPresented = true;

    private async void AlRegistrar(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Formulario);

    private async void AlVerHistorial(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync(Rutas.Actividades);
}