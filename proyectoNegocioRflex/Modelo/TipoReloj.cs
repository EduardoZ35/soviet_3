using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoReloj
    {
        public DataTable traerTipoRelojLocalPorID(int idtipoReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoReloj,nombre,created_at,updated_at from tipoReloj where idtipoReloj=" + idtipoReloj + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoRelojNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoReloj, nombre, created_at, updated_at from tipoReloj where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerUltimoUpdatedAtTipoReloj()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from tipoReloj;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool guardarTipoRelojLocal(int idtipoReloj, string nombre, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoReloj(idtipoReloj,nombre,created_at,updated_at) values(" + idtipoReloj + ",'" + nombre + "','" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarTipoRelojLocal(int idtipoReloj, string nombre, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoReloj set nombre='" + nombre + "', updated_at= '" + updated_at + "' where idtipoReloj=" + idtipoReloj + ";";
            return ejecutor.ejecutarConsulta(sql);
        }



    }
}
