using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    /// <summary>Permite que un ViewModel navegue sin depender del Shell.</summary>
    public interface INavegacionService
    {
        Task VolverAsync();
    }
}
