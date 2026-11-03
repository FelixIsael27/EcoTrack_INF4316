using EcoTrack.Models;
using EcoTrack.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>Formulario de la entidad RegistroReciclaje.</summary>
    public class ReciclajeViewModel : FormularioViewModelBase
    {
        public ReciclajeViewModel(IRegistroService registros, IDialogService dialogos)
            : base(registros, dialogos) { }

        public override CategoriaEco Categoria => CategoriaEco.Residuos;
        public override string NombreCategoria => "Residuos";
        public override string EtiquetaTipo => "Material *";
        public override string EtiquetaCantidad => "Peso reciclado (kg) *";
        public override string EjemploDescripcion => "Separé plásticos y cartón";
        public override List<string> OpcionesTipo { get; } = FactoresEmision.Materiales.Keys.ToList();

        protected override string MensajeTipoObligatorio => "Elige el material que reciclaste.";

        protected override string? ValidarCantidad(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe cuántos kilos reciclaste.";
            if (!TryNumero(texto, out var kg))
                return "El peso debe ser un número, por ejemplo 2 o 0,5.";
            if (kg <= 0)
                return "El peso debe ser mayor que 0 kg.";
            if (kg > 500)
                return "Un solo registro no puede superar los 500 kg.";

            return null;
        }

        protected override RegistroEco CrearRegistro(string tipo, double cantidad) =>
            new RegistroReciclaje { Material = tipo, PesoKg = cantidad };
    }
}
