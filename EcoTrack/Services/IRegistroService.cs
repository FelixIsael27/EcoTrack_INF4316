using System;
using System.Collections.Generic;
using System.Text;
using EcoTrack.Models;

namespace EcoTrack.Services
{
    public interface IRegistroService
    {
        /// <summary>Se dispara cada vez que se agrega un registro, para que las pantallas se refresquen.</summary>
        event Action? RegistrosCambiaron;

        void Agregar(RegistroEco registro);
        IReadOnlyList<RegistroEco> ObtenerTodos();
        IReadOnlyList<RegistroEco> ObtenerPorCategoria(CategoriaEco categoria);
    }
}
