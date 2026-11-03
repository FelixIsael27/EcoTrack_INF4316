using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    public class NavegacionService : INavegacionService
    {
        public Task VolverAsync() => Shell.Current.GoToAsync("..");
    }
}
