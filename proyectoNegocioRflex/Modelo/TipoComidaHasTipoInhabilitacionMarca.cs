using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoComidaHasTipoInhabilitacionMarca
    {
        public DataTable traerTipoComidaHasTipoInhabilitacionMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida,tipoInhabilitacionMarca_idtipoInhabilitacionMarca,tipoMarca_idtipoMarca,habilitado,created_at,updated_at from tipoComida_has_tipoInhabilitacionMarca;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoComidaHasTipoInhabilitacionMarcaPorID(int idtipoComida_has_tipoInhabilitacionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida,tipoInhabilitacionMarca_idtipoInhabilitacionMarca,tipoMarca_idtipoMarca,habilitado,created_at,updated_at from tipoComida_has_tipoInhabilitacionMarca where idtipoComida_has_tipoInhabilitacionMarca=" + idtipoComida_has_tipoInhabilitacionMarca + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarTipoComidaHasTipoInhabilitacionMarca(int idtipoComida_has_tipoInhabilitacionMarca, int tipoComida_idtipoComida, int tipoInhabilitacionMarca_idtipoInhabilitacionMarca, int tipoMarca_idtipoMarca, int habilitado, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoComida_has_tipoInhabilitacionMarca(idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida,tipoInhabilitacionMarca_idtipoInhabilitacionMarca,tipoMarca_idtipoMarca,habilitado,created_at,updated_at) " +
                "values(" + idtipoComida_has_tipoInhabilitacionMarca + "," + tipoComida_idtipoComida + "," + tipoInhabilitacionMarca_idtipoInhabilitacionMarca + "," + tipoMarca_idtipoMarca + "," + habilitado + ",'" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarTipoComidaHasTipoInhabilitacionMarca(int idtipoComida_has_tipoInhabilitacionMarca, int tipoComida_idtipoComida, int tipoInhabilitacionMarca_idtipoInhabilitacionMarca, int tipoMarca_idtipoMarca, int habilitado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoComida_has_tipoInhabilitacionMarca set tipoComida_idtipoComida=" + tipoComida_idtipoComida + ",tipoInhabilitacionMarca_idtipoInhabilitacionMarca=" + tipoInhabilitacionMarca_idtipoInhabilitacionMarca + ",tipoMarca_idtipoMarca=" + tipoMarca_idtipoMarca + ",habilitado=" + habilitado + ",updated_at='" + updated_at + "' where idtipoComida_has_tipoInhabilitacionMarca=" + idtipoComida_has_tipoInhabilitacionMarca + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerTipoComidaHasTipoInhabilitacionMarcaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida,tipoInhabilitacionMarca_idtipoInhabilitacionMarca,tipoMarca_idtipoMarca,habilitado,created_at,updated_at from tipoComida_has_tipoInhabilitacionMarca where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerUltimoUpdatedTipoComidaHasTipoInhabilitacionMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from tipoComida_has_tipoInhabilitacionMarca;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoComidaHasTipoInhabilitacionMarcaPorIDTipoComida(int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select tipoInhabilitacionMarca_idtipoInhabilitacionMarca from tipoComida_has_tipoInhabilitacionMarca where tipoComida_idtipoComida=" + tipoComida_idtipoComida + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoInhabilitacionMarcaComidaPorIdTipoComidaTipoMarca(int tipoComida_idtipoComida, int tipoMarca_idtipoMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT tipoInhabilitacionMarca.idtipoInhabilitacionMarca, tipoInhabilitacionMarca.inhabilitacion " +
                         "FROM tipoInhabilitacionMarca " +
                         "INNER JOIN " +
                         "tipoComida_has_tipoInhabilitacionMarca " +
                         "ON " +
                         "tipoComida_has_tipoInhabilitacionMarca.tipoInhabilitacionMarca_idtipoInhabilitacionMarca = tipoInhabilitacionMarca.idtipoInhabilitacionMarca " +
                         "WHERE tipoComida_has_tipoInhabilitacionMarca.tipoComida_idtipoComida = " + tipoComida_idtipoComida + " " +
                         "AND tipoComida_has_tipoInhabilitacionMarca.tipoMarca_idtipoMarca = " + tipoMarca_idtipoMarca + ";";
            return ejecutor.traerDatosDataTable(sql);
        }




    }
}
