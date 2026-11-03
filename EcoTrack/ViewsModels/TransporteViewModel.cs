using EcoTrack.Models;
using EcoTrack.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>Formulario de la entidad ActividadTransporte.</summary>
    public class TransporteViewModel : FormularioViewModelBase
    {
        public TransporteViewModel(IRegistroService registros, IDialogService dialogos)
            : base(registros, dialogos) { }

        public override CategoriaEco Categoria => CategoriaEco.Transporte;
        public override string NombreCategoria => "Transporte";
        public override string EtiquetaTipo => "Medio de transporte *";
        public override string EtiquetaCantidad => "Distancia recorrida (km) *";
        public override string EjemploDescripcion => "Ir al trabajo en bicicleta";
        public override List<string> OpcionesTipo { get; } = FactoresEmision.Transporte.Keys.ToList();

        protected override string MensajeTipoObligatorio => "Elige el medio de transporte que usaste.";

        protected override string? ValidarCantidad(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe cuántos kilómetros recorriste.";
            if (!TryNumero(texto, out var km))
                return "La distancia debe ser un número, por ejemplo 5 o 2,5.";
            if (km <= 0)
                return "La distancia debe ser mayor que 0 km.";
            if (km > 500)
                return "Una sola actividad no puede superar los 500 km.";

            return null;
        }

        protected override RegistroEco CrearRegistro(string tipo, double cantidad) =>
            new ActividadTransporte { Medio = tipo, DistanciaKm = cantidad };
    }
}
