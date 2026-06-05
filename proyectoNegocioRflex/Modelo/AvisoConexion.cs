using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class AvisoConexion
    {
        //Pendiente quitar la variable terminoFechaRegistro.
        public DataTable traerAvisoConexionPorFecha(string iniciofechaRegistro, string terminoFechaRegistro,string directorioConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            //Prueba de modificación de consulta..
            ///ahora preguntamos por mayor o igual a la fecha del inicio del registro.
            /////y no por rango horario .. veamos como funciona.
            string sql = "select idavisoConexion,reloj_idreloj,fechaLocalEquipo,funcionandoConBateria,comentario,porcentajeBateria,espacioDisponible,updated_at from avisoConexion where updated_at >= '" + iniciofechaRegistro + "'";
            return ejecutor.traerDatosConexionDinamica(sql, directorioConexion);
        }

        public DataTable traerUltimoAvisoConexionRelojPorFecha(int reloj_idreloj ,string iniciofechaRegistro, string terminoFechaRegistro)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idavisoConexion,reloj_idreloj,fechaLocalEquipo,funcionandoConBateria,comentario,porcentajeBateria,espacioDisponible from avisoConexion where reloj_idreloj="+ reloj_idreloj + " and  updated_at between '" + iniciofechaRegistro + "' and '" + terminoFechaRegistro + "' order by idavisoConexion desc limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarAvisoConexionPorFecha(int reloj_idreloj,int funcionandoConBateria,string comentario,int porcentajeBateria, int espacioDisponible)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now,true);
            string sql = "insert into avisoConexion(reloj_idreloj,fechaLocalEquipo,funcionandoConBateria,comentario,porcentajeBateria,espacioDisponible) values(" + reloj_idreloj  + ",'" + fecha + "'," + funcionandoConBateria + ",'" + comentario +"'," + porcentajeBateria + "," + espacioDisponible + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarAvisoConexionPorIdAvisoConexion(int reloj_idreloj, int funcionandoConBateria, string comentario, int porcentajeBateria, int espacioDisponible, int idavisoConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fecha = h.formatearFecha(DateTime.Now, true);
            string sql = "update avisoConexion set fechaLocalEquipo='" + fecha + "', funcionandoConBateria=" + funcionandoConBateria + ",comentario='" + comentario + "',porcentajeBateria=" + porcentajeBateria + ",espacioDisponible=" + espacioDisponible + ",updated_at=now() where idavisoConexion =" + idavisoConexion + "";
            return ejecutor.ejecutarConsultaServidor(sql);
        }
    }
}
