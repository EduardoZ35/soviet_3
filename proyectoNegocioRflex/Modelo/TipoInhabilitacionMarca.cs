using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoInhabilitacionMarca
    {
        public DataTable traerTiposInhabilitacionMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoInhabilitacionMarca,inhabilitacion,updated_at from tipoInhabilitacionMarca";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTiposInhabilitacionMarcaPorID(int idtipoInhabilitacionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoInhabilitacionMarca,inhabilitacion,updated_at from tipoInhabilitacionMarca where idtipoInhabilitacionMarca=" + idtipoInhabilitacionMarca + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTiposInhabilitacionMarcaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoInhabilitacionMarca,inhabilitacion,updated_at from tipoInhabilitacionMarca where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarTiposInhabilitacionMarcaLocal(int idtipoInhabilitacionMarca, string inhabilitacion, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoInhabilitacionMarca(idtipoInhabilitacionMarca,inhabilitacion,updated_at) values(" + idtipoInhabilitacionMarca + ",'" + inhabilitacion + "','" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarTiposInhabilitacionMarcaLocal(int idtipoInhabilitacionMarca, string inhabilitacion, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update  tipoInhabilitacionMarca set inhabilitacion='" + inhabilitacion + "',updated_at='" + updated_at + "' where idtipoInhabilitacionMarca=" + idtipoInhabilitacionMarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }



        public DataTable seederTiposInhabilitacionMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Jornada')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Desayuno')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Almuerzo')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Once')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Cena')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Inicio Jornada')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Término Jornada')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Término Desayuno')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Término Almuerzo')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Término Once')";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoInhabilitacionMarca(inhabilitacion) values('Término Cena')";
            ejecutor.ejecutarConsulta(sql);
            return traerTiposInhabilitacionMarca();
        }


        /// <summary>
        /// Función que busca el tipo de inhabilitacion de marca dependiendo del tipo de marca y del tipo de comida
        /// </summary>
        /// <param name="tipoMarca_idtipoMarca"></param>
        /// <param name="tipoComida_idtipoComida"></param>
        /// <returns>Datatable</returns>
        public DataTable traerTipoInhabilitacionMarcaPorTipoMarcaTipoComida(int tipoMarca_idtipoMarca, int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select tipoInhabilitacionMarca.idtipoInhabilitacionMarca, " +
            " tipoInhabilitacionMarca.inhabilitacion " +
            " from tipoInhabilitacionMarca " +
            " inner join " +
            " tipoComida_has_tipoInhabilitacionMarca " +
            " on " +
            " tipoComida_has_tipoInhabilitacionMarca.tipoInhabilitacionMarca_idtipoInhabilitacionMarca = tipoInhabilitacionMarca.idtipoInhabilitacionMarca " +
            " where " +
            " tipoComida_has_tipoInhabilitacionMarca.tipoMarca_idtipoMarca = " + tipoMarca_idtipoMarca  + " " +
            " and " +
            " tipoComida_has_tipoInhabilitacionMarca.tipoComida_idtipoComida = " + tipoComida_idtipoComida + ";";
            return ejecutor.traerDatosDataTable(sql);
        }
    }
}
