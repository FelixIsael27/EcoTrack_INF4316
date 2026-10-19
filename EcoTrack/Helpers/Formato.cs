using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Helpers
{
    /// <summary>Formato de números y fechas en español para mostrar en pantalla.</summary>
    public static class Formato
    {
        private static readonly CultureInfo Es = CrearCultura();

        private static CultureInfo CrearCultura()
        {
            try { return new CultureInfo("es"); }
            catch (CultureNotFoundException) { return CultureInfo.CurrentCulture; }
        }

        public static string Numero(double valor) => valor.ToString("0.##", Es);

        /// <summary>CO₂ con hasta 2 decimales; los valores diminutos se muestran como "&lt;0,01".</summary>
        public static string Co2(double valor) => valor > 0 && valor < 0.01 ? "<0,01" : Numero(valor);

        public static string FechaLarga(DateTime fecha)
        {
            var texto = fecha.ToString("dddd d 'de' MMMM", Es);
            return char.ToUpper(texto[0], Es) + texto[1..];
        }

        public static string FechaRelativa(DateTime fecha)
        {
            var dia = fecha.Date;
            if (dia == DateTime.Today) return "Hoy";
            if (dia == DateTime.Today.AddDays(-1)) return "Ayer";
            return dia.ToString("d MMM", Es);
        }
    }
}
