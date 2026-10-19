using EcoTrack.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Helpers
{
    public record RegistroVista(string Titulo, string Subtitulo, string Impacto, string Icono, Color ColorCategoria)
    {
        public static RegistroVista Desde(RegistroEco r) => new(
            Titulo: r.Descripcion,
            Subtitulo: $"{Formato.FechaRelativa(r.Fecha)} · {Formato.Numero(r.Cantidad)} {r.Unidad} · {r.Tipo}"
                       + (string.IsNullOrEmpty(r.FotoPath) ? "" : " · con foto"),
            Impacto: $"−{Formato.Co2(r.CO2EvitadoKg)} kg",
            Icono: CategoriaUi.Icono(r.Categoria),
            ColorCategoria: CategoriaUi.ColorDe(r.Categoria));
    }
}
