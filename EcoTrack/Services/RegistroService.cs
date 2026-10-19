using System;
using System.Collections.Generic;
using System.Text;
using EcoTrack.Models;

namespace EcoTrack.Services
{
    /// <summary>Guarda los registros solo en memoria: se pierden al cerrar la app.</summary>
    public class RegistroService : IRegistroService
    {
        private readonly List<RegistroEco> _registros = new();

        public event Action? RegistrosCambiaron;

        public void Agregar(RegistroEco registro)
        {
            ArgumentNullException.ThrowIfNull(registro);
            _registros.Add(registro);
            RegistrosCambiaron?.Invoke();
        }

        public IReadOnlyList<RegistroEco> ObtenerTodos() =>
            _registros
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.CreadoEn)
                .ToList();

        public IReadOnlyList<RegistroEco> ObtenerPorCategoria(CategoriaEco categoria) =>
            ObtenerTodos().Where(r => r.Categoria == categoria).ToList();
    }
}
