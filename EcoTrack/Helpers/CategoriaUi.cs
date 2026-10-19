using EcoTrack.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Helpers
{
    /// <summary>Nombre, icono y color de cada categoría (Señalando la parte visual, separada de los modelos).</summary>
    public static class CategoriaUi
    {
        public static string Nombre(CategoriaEco categoria) => categoria switch
        {
            CategoriaEco.Transporte => "Transporte",
            CategoriaEco.Energia => "Energía",
            CategoriaEco.Residuos => "Residuos",
            _ => "Agua"
        };

        public static string Icono(CategoriaEco categoria) => categoria switch
        {
            CategoriaEco.Transporte => "cat_transporte.png",
            CategoriaEco.Energia => "cat_energia.png",
            CategoriaEco.Residuos => "cat_residuos.png",
            _ => "cat_agua.png"
        };

        // Las claves de Colors.xaml se llaman igual que los valores del enum.
        public static Color ColorDe(CategoriaEco categoria) => Tema.ColorDe(categoria.ToString());
    }
}
