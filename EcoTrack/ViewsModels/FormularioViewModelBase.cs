using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoTrack.Helpers;
using EcoTrack.Models;
using EcoTrack.Services;
using Microsoft.Maui.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>
    /// Lógica común de los formularios: campos, validación por campo, errores inline,
    /// vista previa del ahorro y guardado. Cada entidad hereda de aquí y define solo lo suyo.
    /// </summary>
    public abstract partial class FormularioViewModelBase : ObservableObject
    {
        private readonly IRegistroService _registros;
        private readonly IDialogService _dialogos;

        // Los errores solo se muestran después del primer intento de guardar;
        // a partir de ahí cada campo se vuelve a validar mientras se edita.
        private bool _intentoGuardar;

        protected FormularioViewModelBase(IRegistroService registros, IDialogService dialogos)
        {
            _registros = registros;
            _dialogos = dialogos;
        }

        // ───────────── Lo que define cada entidad ─────────────

        public abstract CategoriaEco Categoria { get; }
        public abstract string NombreCategoria { get; }
        public abstract string EtiquetaTipo { get; }
        public abstract string EtiquetaCantidad { get; }
        public abstract string EjemploDescripcion { get; }
        public abstract List<string> OpcionesTipo { get; }

        protected abstract string MensajeTipoObligatorio { get; }
        protected abstract string? ValidarCantidad(string? texto);
        protected abstract RegistroEco CrearRegistro(string tipo, double cantidad);

        public string PlaceholderDescripcion => $"Ej.: {EjemploDescripcion}";
        public DateTime FechaMinima => DateTime.Today.AddYears(-1);
        public DateTime FechaMaxima => DateTime.Today;

        // ───────────── Campos del formulario ─────────────

        [ObservableProperty] private string _descripcion = string.Empty;
        [ObservableProperty] private string? _tipo;
        [ObservableProperty] private string _cantidad = string.Empty;
        [ObservableProperty] private DateTime? _fecha = DateTime.Today;
        [ObservableProperty] private string _nota = string.Empty;
        [ObservableProperty] private string? _fotoPath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TieneFoto))]
        [NotifyPropertyChangedFor(nameof(SinFoto))]
        private ImageSource? _fotoSource;

        [ObservableProperty] private string _ahorroTexto = "— kg CO₂";

        public bool TieneFoto => FotoSource is not null;
        public bool SinFoto => FotoSource is null;

        // ───────────── Errores inline (uno por campo) ─────────────

        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorDescripcion))] private string _errorDescripcion = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorTipo))] private string _errorTipo = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorCantidad))] private string _errorCantidad = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorFecha))] private string _errorFecha = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorNota))] private string _errorNota = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(TieneErrorFoto))] private string _errorFoto = string.Empty;

        public bool TieneErrorDescripcion => ErrorDescripcion.Length > 0;
        public bool TieneErrorTipo => ErrorTipo.Length > 0;
        public bool TieneErrorCantidad => ErrorCantidad.Length > 0;
        public bool TieneErrorFecha => ErrorFecha.Length > 0;
        public bool TieneErrorNota => ErrorNota.Length > 0;
        public bool TieneErrorFoto => ErrorFoto.Length > 0;

        // ───────────── Reglas de validación, una por campo ─────────────

        private string? ValidarDescripcion(string? texto)
        {
            var limpio = texto?.Trim() ?? string.Empty;

            if (limpio.Length == 0)
                return $"Escribe qué hiciste, por ejemplo «{EjemploDescripcion}».";
            if (limpio.Length < 3)
                return "La descripción debe tener al menos 3 caracteres.";
            if (limpio.Length > 60)
                return $"La descripción no puede superar los 60 caracteres (ahora tiene {limpio.Length}).";
            if (!limpio.Any(char.IsLetter))
                return "La descripción debe incluir al menos una letra.";

            return null;
        }

        private string? ValidarTipo(string? tipo) =>
            string.IsNullOrWhiteSpace(tipo) ? MensajeTipoObligatorio : null;

        private string? ValidarFecha(DateTime? fecha)
        {
            if (fecha is null)
                return "Elige la fecha en que hiciste la actividad.";
            if (fecha.Value.Date > FechaMaxima)
                return "La fecha no puede ser futura: solo puedes registrar lo que ya hiciste.";
            if (fecha.Value.Date < FechaMinima.Date)
                return "La fecha no puede tener más de un año de antigüedad.";

            return null;
        }

        private static string? ValidarNota(string? nota)
        {
            var largo = nota?.Trim().Length ?? 0;
            return largo > 200
                ? $"La nota no puede superar los 200 caracteres (ahora tiene {largo})."
                : null;
        }

        /// <summary>Acepta tanto "2,5" como "2.5".</summary>
        protected static bool TryNumero(string? texto, out double valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            var normal = texto.Trim().Replace(',', '.');
            return double.TryParse(normal, NumberStyles.Float, CultureInfo.InvariantCulture, out valor)
                   && !double.IsNaN(valor)
                   && !double.IsInfinity(valor);
        }

        // ───────────── Validación en vivo (después del primer intento) ─────────────

        partial void OnDescripcionChanged(string value)
        {
            if (_intentoGuardar) ErrorDescripcion = ValidarDescripcion(value) ?? string.Empty;
        }

        partial void OnTipoChanged(string? value)
        {
            if (_intentoGuardar) ErrorTipo = ValidarTipo(value) ?? string.Empty;
            ActualizarAhorro();
        }

        partial void OnCantidadChanged(string value)
        {
            if (_intentoGuardar) ErrorCantidad = ValidarCantidad(value) ?? string.Empty;
            ActualizarAhorro();
        }

        partial void OnFechaChanged(DateTime? value)
        {
            if (_intentoGuardar) ErrorFecha = ValidarFecha(value) ?? string.Empty;
        }

        partial void OnNotaChanged(string value)
        {
            if (_intentoGuardar) ErrorNota = ValidarNota(value) ?? string.Empty;
        }

        private bool ValidarTodo()
        {
            ErrorDescripcion = ValidarDescripcion(Descripcion) ?? string.Empty;
            ErrorTipo = ValidarTipo(Tipo) ?? string.Empty;
            ErrorCantidad = ValidarCantidad(Cantidad) ?? string.Empty;
            ErrorFecha = ValidarFecha(Fecha) ?? string.Empty;
            ErrorNota = ValidarNota(Nota) ?? string.Empty;

            return !(TieneErrorDescripcion || TieneErrorTipo || TieneErrorCantidad
                     || TieneErrorFecha || TieneErrorNota);
        }

        private void LimpiarErrores()
        {
            ErrorDescripcion = string.Empty;
            ErrorTipo = string.Empty;
            ErrorCantidad = string.Empty;
            ErrorFecha = string.Empty;
            ErrorNota = string.Empty;
            ErrorFoto = string.Empty;
        }

        // ───────────── Vista previa del ahorro ─────────────

        private void ActualizarAhorro()
        {
            var co2 = TryNumero(Cantidad, out var valor) && valor > 0
                ? FactoresEmision.CO2(Categoria, Tipo ?? string.Empty, valor)
                : 0;

            AhorroTexto = co2 > 0 ? $"≈ {Formato.Co2(co2)} kg CO₂" : "— kg CO₂";
        }

        // ───────────── Foto ─────────────

        [RelayCommand]
        private async Task ElegirFotoAsync()
        {
            ErrorFoto = string.Empty;

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
                FotoPath = ruta;
                FotoSource = ImageSource.FromStream(() => File.OpenRead(ruta));
            }
            catch (Exception)
            {
                ErrorFoto = "No se pudo abrir la galería. Revisa los permisos e inténtalo de nuevo.";
            }
        }

        [RelayCommand]
        private void QuitarFoto()
        {
            FotoPath = null;
            FotoSource = null;
            ErrorFoto = string.Empty;
        }

        // ───────────── Guardar y limpiar ─────────────

        [RelayCommand]
        private async Task GuardarAsync()
        {
            _intentoGuardar = true;

            if (!ValidarTodo())
                return;

            TryNumero(Cantidad, out var cantidad);

            var registro = CrearRegistro(Tipo!, cantidad);
            registro.Descripcion = Descripcion.Trim();
            registro.Fecha = Fecha!.Value.Date;
            registro.Nota = string.IsNullOrWhiteSpace(Nota) ? null : Nota.Trim();
            registro.FotoPath = FotoPath;

            _registros.Agregar(registro);

            var mensaje = $"«{registro.Descripcion}» se registró en {NombreCategoria}. " +
                          $"Evitaste ≈ {Formato.Co2(registro.CO2EvitadoKg)} kg de CO₂.";

            Limpiar();
            await _dialogos.MostrarAlertaAsync("¡Actividad guardada!", mensaje);
        }

        /// <summary>Deja el formulario como nuevo: sin datos y sin errores.</summary>
        public void Limpiar()
        {
            _intentoGuardar = false;

            Descripcion = string.Empty;
            Tipo = null;
            Cantidad = string.Empty;
            Fecha = DateTime.Today;
            Nota = string.Empty;
            QuitarFoto();

            LimpiarErrores();
            AhorroTexto = "— kg CO₂";
        }

        /// <summary>Al cambiar de categoría se conserva lo que es igual en todas.</summary>
        public void ConservarDatosComunesDe(FormularioViewModelBase otro)
        {
            Descripcion = otro.Descripcion;
            Fecha = otro.Fecha;
            Nota = otro.Nota;
            FotoPath = otro.FotoPath;
            FotoSource = otro.FotoSource;
        }
    }
}
