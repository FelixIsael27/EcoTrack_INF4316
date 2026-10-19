using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Models
{
    /// <summary>Las cuatro categorías de actividades que maneja EcoTrack.</summary>
    public enum CategoriaEco
    {
        Transporte,
        Energia,
        Residuos,
        Agua
    }

    /// <summary>
    /// Clase base de todos los registros. Guarda lo que comparten las cuatro entidades
    /// y obliga a cada una a decir su categoría, su cantidad, su unidad y su CO₂ (Dioxido de Carbono) evitado.
    /// </summary>
    public abstract class RegistroEco
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreadoEn { get; } = DateTime.Now;

        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Today;
        public string? Nota { get; set; }
        public string? FotoPath { get; set; }

        public abstract CategoriaEco Categoria { get; }
        public abstract string Tipo { get; }
        public abstract double Cantidad { get; }
        public abstract string Unidad { get; }
        public abstract double CO2EvitadoKg { get; }
    }
}
