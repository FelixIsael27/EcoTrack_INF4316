using EcoTrack.Models;
using EcoTrack.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>Formulario de la entidad ConsumoAgua.</summary>
    public class AguaViewModel : FormularioViewModelBase
    {
        public AguaViewModel(IRegistroService registros, IDialogService dialogos)
            : base(registros, dialogos) { }

        public override CategoriaEco Categoria => CategoriaEco.Agua;
        public override string NombreCategoria => "Agua";
        public override string EtiquetaTipo => "Acción *";
        public override string EtiquetaCantidad => "Agua ahorrada (litros) *";
        public override string EjemploDescripcion => "Ducha de 4 minutos";
        public override List<string> OpcionesTipo { get; } = FactoresEmision.AccionesAgua.ToList();

        protected override string MensajeTipoObligatorio => "Elige cómo ahorraste agua.";

        protected override string? ValidarCantidad(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe cuántos litros ahorraste.";
            if (!TryNumero(texto, out var litros))
                return "Los litros deben ser un número, por ejemplo 40.";
            if (litros <= 0)
                return "Los litros ahorrados deben ser mayores que 0.";
            if (litros > 10000)
                return "Un solo registro no puede superar los 10 000 litros.";

            return null;
        }

        protected override RegistroEco CrearRegistro(string tipo, double cantidad) =>
            new ConsumoAgua { Accion = tipo, Litros = cantidad };
    }
}
