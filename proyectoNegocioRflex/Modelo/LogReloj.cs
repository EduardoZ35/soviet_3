using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class LogReloj
    {
        public DataTable traerLogsSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idlogReloj, nombreEquipo, detalleLog,fecha,tipoLog from logReloj where respaldado=0 LIMIT 10";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarLogReloj(string nombreEquipo, string detalleLog, string tipoLog)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now, true);
            string sql = "insert into logReloj(nombreEquipo, detalleLog,fecha,tipoLog) values('" + nombreEquipo + "','" + detalleLog + "','" + fecha + "','" + tipoLog + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoRespaldadoLogReloj(int idlogReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update logReloj set respaldado =1 where idlogReloj =" + idlogReloj + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarLogRelojNube(string nombreEquipo, string detalleLog, string tipoLog)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaFormateada = h.formatearFecha(DateTime.Now, true);
            string sql = "insert into logReloj(nombreEquipo, detalleLog,respaldado,fecha,tipoLog) values('" + nombreEquipo + "','" + detalleLog + "',1,'" + fechaFormateada + "','" + tipoLog + "')";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        /**
         * Función que elimina los logs del los relojes que tienen una antiguedad de 7 o mas días
         */ 
        public bool eliminarLogsRelojAntiguosYaRespaldados()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now.AddDays(-7), true);
            string sql = "delete from logReloj where respaldado=1 and updated_at <='" + fecha + "'";
            return ejecutor.ejecutarConsulta(sql);
        }
    }
}
