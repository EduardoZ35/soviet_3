using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace proyectoNegocioRflex.Utilidades
{
    public class ComprobacionRed
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);
        public static bool IsInternetAvailable()
        {
            try
            {
                int description;
                return InternetGetConnectedState(out description, 0); ;
            }
            catch (Exception ex)
            {
                Herramientas h = new Herramientas();
                h.generarLogSincronizador("IsInternetAvailable: " + ex.Message, "ERROR");
                return true;
            }

        }
    }
}
