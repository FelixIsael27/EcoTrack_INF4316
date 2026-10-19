using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Helpers
{
    /// <summary>Lee colores y estilos definidos en Colors.xaml y Styles.xaml desde código C Sharp (C#).</summary>
    public static class Tema
    {
        public static Color ColorDe(string clave, string respaldo = "#808080")
        {
            var recursos = Application.Current?.Resources;
            if (recursos is not null && recursos.TryGetValue(clave, out var valor) && valor is Color color)
                return color;
            return Color.FromArgb(respaldo);
        }

        public static Style? Estilo(string clave)
        {
            var recursos = Application.Current?.Resources;
            if (recursos is not null && recursos.TryGetValue(clave, out var valor))
                return valor as Style;
            return null;
        }
    }
}
