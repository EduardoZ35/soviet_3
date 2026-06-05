using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoMarca
    {
        public DataTable traerTipoMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoMarca,descripcion,updated_at from tipoMarca";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoMarcaPorID(int idtipoMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoMarca,descripcion,updated_at from tipoMarca where idtipoMarca=" + idtipoMarca + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoMarcaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoMarca,descripcion,updated_at from tipoMarca where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarTipoMarcaLocal(int idtipoMarca, string descripcion, int respaldado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoMarca(idtipoMarca,  descripcion,  respaldado,  updated_at) values(" + idtipoMarca + ",'" + descripcion + "'," + respaldado + ",'" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarTipoMarcaLocal(int idtipoMarca, string descripcion, int respaldado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoMarca set  descripcion='" + descripcion + "',  respaldado=" + respaldado + ",  updated_at='" + updated_at + "' where idtipoMarca=" + idtipoMarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable seederTipoMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoMarca(descripcion,respaldado) values('Marca de Entrada',1)";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoMarca(descripcion,respaldado) values('Marca de Salida',1)";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoMarca(descripcion,respaldado) values('Marca de Inicio de Comida',1)";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoMarca(descripcion,respaldado) values('Marca de Término de Comida',1)";
            ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoMarca(descripcion,respaldado) values('Marca Casino',1)";
            ejecutor.ejecutarConsulta(sql);
            return traerTipoMarca();
        }

        //Preguntar por datos por defecto

    }
}
