using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Models
{
    /// <summary>Entidad 1: Viaje hecho en un medio de transporte limpio.</summary>
    public class ActividadTransporte : RegistroEco
    {
        public string Medio { get; set; } = string.Empty;
        public double DistanciaKm { get; set; }

        public override CategoriaEco Categoria => CategoriaEco.Transporte;
        public override string Tipo => Medio;
        public override double Cantidad => DistanciaKm;
        public override string Unidad => "km";
        public override double CO2EvitadoKg => FactoresEmision.CO2(Categoria, Medio, DistanciaKm);
    }

    /// <summary>Entidad 2: Ahorro de energía eléctrica.</summary>
    public class ConsumoElectrico : RegistroEco
    {
        public string TipoAhorro { get; set; } = string.Empty;
        public double EnergiaKwh { get; set; }

        public override CategoriaEco Categoria => CategoriaEco.Energia;
        public override string Tipo => TipoAhorro;
        public override double Cantidad => EnergiaKwh;
        public override string Unidad => "kWh";
        public override double CO2EvitadoKg => FactoresEmision.CO2(Categoria, TipoAhorro, EnergiaKwh);
    }

    /// <summary>Entidad 3: Material reciclado o compostado.</summary>
    public class RegistroReciclaje : RegistroEco
    {
        public string Material { get; set; } = string.Empty;
        public double PesoKg { get; set; }

        public override CategoriaEco Categoria => CategoriaEco.Residuos;
        public override string Tipo => Material;
        public override double Cantidad => PesoKg;
        public override string Unidad => "kg";
        public override double CO2EvitadoKg => FactoresEmision.CO2(Categoria, Material, PesoKg);
    }

    /// <summary>Entidad 4: Ahorro de agua.</summary>
    public class ConsumoAgua : RegistroEco
    {
        public string Accion { get; set; } = string.Empty;
        public double Litros { get; set; }

        public override CategoriaEco Categoria => CategoriaEco.Agua;
        public override string Tipo => Accion;
        public override double Cantidad => Litros;
        public override string Unidad => "L";
        public override double CO2EvitadoKg => FactoresEmision.CO2(Categoria, Accion, Litros);
    }
}
