using System;
using System.Data;
using proyectoNegocioRflex.Modelo;

namespace proyectoNegocioRflex.Sincronizacion
{
    public class IdentificadorReloj
    {
        public static TipoDispositivo DeterminarTipo(int soloEnrolador, int relojCasino)
        {
            if (soloEnrolador == 1) return TipoDispositivo.Enrolador;
            if (relojCasino == 1)   return TipoDispositivo.Casino;
            return TipoDispositivo.Asistencia;
        }

        public DeviceIdentity Identificar()
        {
            Reloj r = new Reloj();
            DataTable dt = r.traerIdentificacionRelojPorNombre();

            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException(
                    $"Reloj no encontrado en servidor remoto para equipo '{Environment.MachineName}'. " +
                    "Registrar el equipo en la plataforma rFlex.");

            var fila = dt.Rows[0];
            bool bloqueado = fila[4].ToString() == "1";
            if (bloqueado)
                throw new InvalidOperationException(
                    $"Reloj '{Environment.MachineName}' está bloqueado. Contactar soporte rFlex.");

            return new DeviceIdentity
            {
                IdReloj    = int.Parse(fila[0].ToString()),
                SucursalId = int.Parse(fila[1].ToString()),
                Tipo       = DeterminarTipo(int.Parse(fila[2].ToString()), int.Parse(fila[3].ToString())),
                Bloqueado  = bloqueado,
                AutoInicia = fila[5].ToString() == "1",
                TipoRelojId = int.Parse(fila[6].ToString())
            };
        }
    }
}
