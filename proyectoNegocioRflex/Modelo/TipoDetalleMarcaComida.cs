using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoDetalleMarcaComida
    {

        public DataTable tiposDetalleMarcaComida()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoDetalleMarcaComida,detalleTipoComida,updated_at from tipoDetalleMarcaComida";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable tiposDetalleMarcaComidaLocalPorID(int idtipoDetalleMarcaComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoDetalleMarcaComida,detalleTipoComida,updated_at from tipoDetalleMarcaComida where idtipoDetalleMarcaComida=" + idtipoDetalleMarcaComida + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable tiposDetalleMarcaComidaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoDetalleMarcaComida,detalleTipoComida,updated_at from tipoDetalleMarcaComida where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool crearTipoDetalleMarcaComida(int idtipoDetalleMarcaComida, string detalleTipoComida, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoDetalleMarcaComida(idtipoDetalleMarcaComida,detalleTipoComida,updated_at) values(" + idtipoDetalleMarcaComida + ",'" + detalleTipoComida + "','" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarTipoDetalleMarcaComida(int idtipoDetalleMarcaComida, string detalleTipoComida, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoDetalleMarcaComida set detalleTipoComida='" + detalleTipoComida + "', updated_at='" + updated_at + "' where idtipoDetalleMarcaComida=" + idtipoDetalleMarcaComida + "";
            return ejecutor.ejecutarConsulta(sql);
        }

    }
}
