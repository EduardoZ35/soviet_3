using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class LogSincronizador
    {

        public bool agregarLogError(string incidencia, string tipoLog)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into logSincronizador(equipo,incidencia,tipoLog) values('" + Environment.MachineName + "','" + incidencia + "','" + tipoLog + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoLogErrorLocal(int idLogSincronizador)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update logSincronizador set respaldado=1 where idLogSincronizador=" + idLogSincronizador + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarLogErrorServidor(string incidencia, string equipo, DateTime fecha, string tipoLog)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaFormateada = h.formatearFecha(fecha, true);
            string sql = "insert into logSincronizador(equipo,incidencia,fecha,respaldado,tipoLog) values('" + Environment.MachineName + "','" + incidencia + "','" + fechaFormateada + "',1,'" + tipoLog + "')";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public DataTable traerLogsSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idLogSincronizador,equipo,incidencia,fecha,tipoLog from logSincronizador where respaldado=0 LIMIT 10";
            return ejecutor.traerDatosDataTable(sql);
        }

        /**
         * Elimina logs ya respaldados con más de 7 días (todos los tipos).
         */
        public bool eliminarLogsSincronizadorAntiguosYaRespaldados()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now.AddDays(-7), true);
            string sql = "delete from logSincronizador where respaldado=1 and updated_at <='" + fecha + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        /**
         * Purge masivo de logs INFO sin respaldar (nunca subirán a la nube).
         * Elimina los de más de 1 día para no perder logs recientes útiles para diagnóstico local.
         */
        public bool purgarLogsInfoSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now.AddDays(-1), true);
            string sql = "delete from logSincronizador where tipoLog='INFO' and respaldado=0 and updated_at <='" + fecha + "'";
            return ejecutor.ejecutarConsulta(sql);
        }
    }
}
