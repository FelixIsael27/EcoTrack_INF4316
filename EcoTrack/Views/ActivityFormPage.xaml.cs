using System.Globalization;
using EcoTrack.Helpers;
using EcoTrack.Models;
using EcoTrack.Services;
using Microsoft.Maui.Media;

namespace EcoTrack.Views;

public partial class ActivityFormPage : ContentPage
{
    private readonly IRegistroService _registros;
    private readonly Dictionary<CategoriaEco, Border> _chips;

    private CategoriaEco _categoria = CategoriaEco.Transporte;
    private string? _fotoPath;

    public ActivityFormPage(IRegistroService registros)
    {
        InitializeComponent();
        _registros = registros;

        _chips = new Dictionary<CategoriaEco, Border>
        {
            [CategoriaEco.Transporte] = ChipTransporte,
            [CategoriaEco.Energia] = ChipEnergia,
            [CategoriaEco.Residuos] = ChipResiduos,
            [CategoriaEco.Agua] = ChipAgua
        };

        // No se permiten fechas futuras: el calendario se limita a hoy.
        FechaPicker.MaximumDate = DateTime.Today;
        FechaPicker.Date = DateTime.Today;

        AplicarCategoria(CategoriaEco.Transporte);
    }

    // ───────────── Categoría ─────────────

    private void AlElegirCategoria(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string clave
            && Enum.TryParse<CategoriaEco>(clave, out var categoria)
            && categoria != _categoria)
        {
            AplicarCategoria(categoria);
        }
    }

    /// <summary>Cambia las etiquetas y las opciones del formulario según la categoría elegida.</summary>
    private void AplicarCategoria(CategoriaEco categoria)
    {
        _categoria = categoria;

        foreach (var (c, chip) in _chips)
            chip.Style = Tema.Estilo(c == categoria ? "ChipCategoriaActivo" : "ChipCategoria");

        switch (categoria)
        {
            case CategoriaEco.Transporte:
                TipoLabel.Text = "Medio de transporte *";
                CantidadLabel.Text = "Distancia recorrida (km) *";
                DescripcionEntry.Placeholder = "Ej.: Ir al trabajo en bicicleta";
                TipoPicker.ItemsSource = FactoresEmision.Transporte.Keys.ToList();
                break;

            case CategoriaEco.Energia:
                TipoLabel.Text = "Tipo de ahorro *";
                CantidadLabel.Text = "Energía ahorrada (kWh) *";
                DescripcionEntry.Placeholder = "Ej.: Apagué el aire 3 horas";
                TipoPicker.ItemsSource = FactoresEmision.AccionesEnergia.ToList();
                break;

            case CategoriaEco.Residuos:
                TipoLabel.Text = "Material *";
                CantidadLabel.Text = "Peso reciclado (kg) *";
                DescripcionEntry.Placeholder = "Ej.: Separé plásticos y cartón";
                TipoPicker.ItemsSource = FactoresEmision.Materiales.Keys.ToList();
                break;

            default:
                TipoLabel.Text = "Acción *";
                CantidadLabel.Text = "Agua ahorrada (litros) *";
                DescripcionEntry.Placeholder = "Ej.: Ducha de 4 minutos";
                TipoPicker.ItemsSource = FactoresEmision.AccionesAgua.ToList();
                break;
        }

        TipoPicker.SelectedIndex = -1;
        CantidadEntry.Text = string.Empty;
        LimpiarErrores();
        ActualizarAhorro();
    }

    // ───────────── Vista previa del ahorro ─────────────

    private void AlCambiarDatos(object? sender, EventArgs e) => ActualizarAhorro();

    private void AlCambiarCantidad(object? sender, TextChangedEventArgs e) => ActualizarAhorro();

    private void ActualizarAhorro()
    {
        var tipo = TipoPicker.SelectedItem as string ?? string.Empty;
        var co2 = TryNumero(CantidadEntry.Text, out var cantidad) && cantidad > 0
            ? FactoresEmision.CO2(_categoria, tipo, cantidad)
            : 0;

        AhorroLabel.Text = co2 > 0 ? $"≈ {Formato.Co2(co2)} kg CO₂" : "— kg CO₂";
    }

    // ───────────── Validación ─────────────

    private bool Validar(out double cantidad)
    {
        LimpiarErrores();
        var valido = true;
        cantidad = 0;

        if (string.IsNullOrWhiteSpace(DescripcionEntry.Text))
        {
            MostrarError(ErrDescripcion, "Escribe qué hiciste.");
            valido = false;
        }

        if (TipoPicker.SelectedIndex < 0)
        {
            MostrarError(ErrTipo, "Elige una opción de la lista.");
            valido = false;
        }

        if (!TryNumero(CantidadEntry.Text, out cantidad))
        {
            MostrarError(ErrCantidad, "Escribe un número, por ejemplo 5 o 2,5.");
            valido = false;
        }
        else if (cantidad <= 0)
        {
            MostrarError(ErrCantidad, "El valor debe ser mayor que 0.");
            valido = false;
        }

        return valido;
    }

    /// <summary>Acepta tanto "2,5" como "2.5".</summary>
    private static bool TryNumero(string? texto, out double valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        var normal = texto.Trim().Replace(',', '.');
        return double.TryParse(normal, NumberStyles.Float, CultureInfo.InvariantCulture, out valor)
               && !double.IsNaN(valor)
               && !double.IsInfinity(valor);
    }

    private static void MostrarError(Label etiqueta, string mensaje)
    {
        etiqueta.Text = mensaje;
        etiqueta.IsVisible = true;
    }

    private void LimpiarErrores()
    {
        ErrDescripcion.IsVisible = false;
        ErrTipo.IsVisible = false;
        ErrCantidad.IsVisible = false;
    }

    // ───────────── Foto ─────────────

    private async void AlElegirFoto(object? sender, TappedEventArgs e)
    {
        try
        {
            var fotos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Elige una foto del registro"
            });

            // PickPhotosAsync devuelve una lista: usamos solo la primera foto.
            var foto = fotos?.FirstOrDefault();
            if (foto is null)
                return;

            var ruta = foto.FullPath;
            _fotoPath = ruta;
            FotoPreview.Source = ImageSource.FromStream(() => File.OpenRead(ruta));
            FotoPreview.IsVisible = true;
            FotoVacia.IsVisible = false;
            QuitarFotoButton.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("No se pudo cargar la foto", ex.Message, "OK");
        }
    }

    private void AlQuitarFoto(object? sender, EventArgs e)
    {
        _fotoPath = null;
        FotoPreview.Source = null;
        FotoPreview.IsVisible = false;
        FotoVacia.IsVisible = true;
        QuitarFotoButton.IsVisible = false;
    }

    // ───────────── Guardar y cerrar ─────────────

    private async void AlGuardar(object? sender, EventArgs e)
    {
        if (!Validar(out var cantidad))
            return;

        var tipo = (string)TipoPicker.SelectedItem;

        // Cada categoría crea su propia entidad
        RegistroEco registro = _categoria switch
        {
            CategoriaEco.Transporte => new ActividadTransporte { Medio = tipo, DistanciaKm = cantidad },
            CategoriaEco.Energia => new ConsumoElectrico { TipoAhorro = tipo, EnergiaKwh = cantidad },
            CategoriaEco.Residuos => new RegistroReciclaje { Material = tipo, PesoKg = cantidad },
            _ => new ConsumoAgua { Accion = tipo, Litros = cantidad }
        };

        registro.Descripcion = DescripcionEntry.Text!.Trim();
        registro.Fecha = registro.Fecha = (DateTime?)FechaPicker.Date ?? DateTime.Today;
        registro.Nota = string.IsNullOrWhiteSpace(NotaEditor.Text) ? null : NotaEditor.Text.Trim();
        registro.FotoPath = _fotoPath;

        _registros.Agregar(registro);

        await DisplayAlertAsync("Actividad guardada",
            $"«{registro.Descripcion}» se registró en {CategoriaUi.Nombre(_categoria)}.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void AlCerrar(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}