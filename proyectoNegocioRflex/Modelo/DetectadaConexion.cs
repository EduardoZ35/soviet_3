using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class DetectadaConexion
    {

        /**
         * Función que se encarga de administrar el registro de la tabla detectadaConexion
         */
        public bool actualizarEstadoTieneConexion(int tieneConexion)
        {
            try
            {
                DataTable dt = traeRegistroEstadoConexion();
                if (dt != null)
                {
                    //Si se encuentra un registro .. actualizamos..
                    //de lo contrario agregamos el registro.
                    if (dt.Rows.Count > 0)
                    {
                        return actualizarRegistroEstadoTieneConexion(tieneConexion);
                    }
                    else
                    {
                        return agregarRegistroEstadoTieneConexion(tieneConexion);
                    }
                }
                else
                {
                    Herramientas h = new Herramientas();
                    h.generarLogSincronizador("actualizarEstadoTieneConexion() " + Environment.MachineName + " No se pudo consultar la tabla detectadaConexion", "ERROR");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Herramientas h = new Herramientas();
                h.generarLogSincronizador("actualizarEstadoTieneConexion() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                return false;
            }
        }


        public DataTable traeRegistroEstadoConexion()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select iddetectadaConexion,updated_at,tieneConexion from detectadaConexion order by iddetectadaConexion desc limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarRegistroEstadoTieneConexion(int tieneConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string sql = "update detectadaConexion set tieneConexion=" + tieneConexion + ", updated_at='" + h.formatearFecha(DateTime.Now, true) + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarRegistroEstadoTieneConexion(int tieneConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into detectadaConexion(tieneConexion)values(" + tieneConexion + ")";
            return ejecutor.ejecutarConsulta(sql);
        }


    }
}
