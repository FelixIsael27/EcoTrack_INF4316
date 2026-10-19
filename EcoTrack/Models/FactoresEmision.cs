using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Models
{
    /// <summary>
    /// Factores usados para estimar el CO₂ evitado. Estos son valores de referencia APROXIMADOS,
    /// pensados para este proyecto.
    /// </summary>
    public static class FactoresEmision
    {
        // kg de CO₂ evitados por km, frente a hacer el mismo trayecto en un auto de gasolina.
        public static readonly IReadOnlyDictionary<string, double> Transporte = new Dictionary<string, double>
        {
            ["Bicicleta"] = 0.19,
            ["Caminata"] = 0.19,
            ["Metro o tren"] = 0.14,
            ["Vehículo eléctrico"] = 0.10,
            ["Autobús"] = 0.09,
            ["Carro compartido"] = 0.09
        };

        // kg de CO₂ evitados por cada kg reciclado, según el material.
        public static readonly IReadOnlyDictionary<string, double> Materiales = new Dictionary<string, double>
        {
            ["Plástico"] = 1.5,
            ["Vidrio"] = 0.3,
            ["Papel y cartón"] = 0.9,
            ["Metal"] = 2.0,
            ["Orgánico (compost)"] = 0.5
        };

        public static readonly string[] AccionesEnergia =
        {
        "Apagar aparatos",
        "Cambiar a bombillos LED",
        "Desconectar aparatos en stand-by",
        "Usar luz natural",
        "Usar un equipo eficiente"
    };

        public static readonly string[] AccionesAgua =
        {
        "Ducha más corta",
        "Reutilizar agua",
        "Reparar una fuga",
        "Riego eficiente",
        "Cerrar el grifo"
    };

        public const double KgPorKwh = 0.4;
        public const double KgPorLitro = 0.0004;

        public static double CO2(CategoriaEco categoria, string tipo, double cantidad) => categoria switch
        {
            CategoriaEco.Transporte => cantidad * (Transporte.TryGetValue(tipo, out var porKm) ? porKm : 0),
            CategoriaEco.Energia => cantidad * KgPorKwh,
            CategoriaEco.Residuos => cantidad * (Materiales.TryGetValue(tipo, out var porKg) ? porKg : 0),
            _ => cantidad * KgPorLitro
        };
    }
}
