using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class ResolucionMarca
    {

        public DataTable traerResolucionesMarcaLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idresolucionMarca,nombreResolucion, created_at,updated_at from resolucionMarca";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerResolucionesMarcaLocalPorID(int idresolucionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idresolucionMarca, nombreResolucion, created_at, updated_at from resolucionMarca where idresolucionMarca=" + idresolucionMarca + "";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerResolucionesMarcaServidor(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idresolucionMarca,nombreResolucion, created_at,updated_at from resolucionMarca where updated_at >'" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarResolucionMarca(string nombreResolucion, DateTime created_at, DateTime updated_at, int idresolucionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string createdAtFormateado = h.formatearFecha(created_at, true);
            string updatedAtFormateado = h.formatearFecha(updated_at, true);
            string sql = "insert into resolucionMarca(idresolucionMarca,nombreResolucion,created_at,updated_at) values(" + idresolucionMarca + ",'" + nombreResolucion + "','" + createdAtFormateado + "','" + updatedAtFormateado + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarResolucionMarca(string nombreResolucion, DateTime updated_at, int idresolucionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string updatedAtFormateado = h.formatearFecha(updated_at, true);
            string sql = "update resolucionMarca set nombreResolucion='" + nombreResolucion + "', updated_at='" + updatedAtFormateado + "' where idresolucionMarca=" + idresolucionMarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }
    }
}
