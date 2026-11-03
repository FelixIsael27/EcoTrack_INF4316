using EcoTrack.Models;
using EcoTrack.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>Formulario de la entidad ConsumoElectrico.</summary>
    public class EnergiaViewModel : FormularioViewModelBase
    {
        public EnergiaViewModel(IRegistroService registros, IDialogService dialogos)
            : base(registros, dialogos) { }

        public override CategoriaEco Categoria => CategoriaEco.Energia;
        public override string NombreCategoria => "Energía";
        public override string EtiquetaTipo => "Tipo de ahorro *";
        public override string EtiquetaCantidad => "Energía ahorrada (kWh) *";
        public override string EjemploDescripcion => "Apagué el aire 3 horas";
        public override List<string> OpcionesTipo { get; } = FactoresEmision.AccionesEnergia.ToList();

        protected override string MensajeTipoObligatorio => "Elige qué hiciste para ahorrar energía.";

        protected override string? ValidarCantidad(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe cuántos kWh ahorraste.";
            if (!TryNumero(texto, out var kwh))
                return "Los kWh deben ser un número, por ejemplo 1,5.";
            if (kwh <= 0)
                return "El ahorro debe ser mayor que 0 kWh.";
            if (kwh > 1000)
                return "Un solo registro no puede superar los 1000 kWh.";

            return null;
        }

        protected override RegistroEco CrearRegistro(string tipo, double cantidad) =>
            new ConsumoElectrico { TipoAhorro = tipo, EnergiaKwh = cantidad };
    }
}
