using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoRolUsuario
    {

        public DataTable traerDatosTipoRolUsuario()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRolUsuario, nombreTipoRolUsuario from tipoRolUsuario ";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoRolUsuarioDesdeNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRolUsuario,nombreTipoRolUsuario,created_at,updated_at from tipoRolUsuario where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerTipoRolUsuarioDesdeNubeTodos()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRolUsuario, nombreTipoRolUsuario, created_at, updated_at from tipoRolUsuario";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerTipoRolUsuarioPorID(int idtipoRolUsuario)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRolUsuario,updated_at from tipoRolUsuario where idtipoRolUsuario=" + idtipoRolUsuario + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarTipoRolUsuarioLocal(int idtipoRolUsuario, string nombreTipoRolUsuario, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoRolUsuario(idtipoRolUsuario,nombreTipoRolUsuario,updated_at) values (" + idtipoRolUsuario + ",'" + nombreTipoRolUsuario + "','" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarTipoRolUsuarioLocal(int idtipoRolUsuario, string nombreTipoRolUsuario, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoRolUsuario set nombreTipoRolUsuario='" + nombreTipoRolUsuario + "',updated_at='" + updated_at + "' where idtipoRolUsuario=" + idtipoRolUsuario;
            return ejecutor.ejecutarConsulta(sql);
        }

    }
}
