using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class DetallePerfilCasino
    {
        public DataTable traerDetallePerfilCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado,created_at ,updated_at from detallePerfilCasino;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDetallePerfilCasinoPorID(int iddetallePerfilCasino)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado,created_at ,updated_at from detallePerfilCasino where iddetallePerfilCasino=" + iddetallePerfilCasino + ";";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerDetallePerfilCasinoPorPerfilCasino(int perfilCasino_idperfilCasino)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select tipoComida_idtipoComida, numeroDia, habilitado from detallePerfilCasino where perfilCasino_idperfilCasino=" + perfilCasino_idperfilCasino + " and habilitado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDetallePerfilCasinoNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado,created_at ,updated_at from detallePerfilCasino where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarDetallePerfilCasino(int iddetallePerfilCasino, int perfilCasino_idperfilCasino, int tipoComida_idtipoComida, int numeroDia, int habilitado, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into detallePerfilCasino(iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado,created_at ,updated_at) values(" + iddetallePerfilCasino + "," + perfilCasino_idperfilCasino + "," + tipoComida_idtipoComida + "," + numeroDia + "," + habilitado + ",'" + created_at + "' ,'" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarDetallePerfilCasino(int iddetallePerfilCasino, int perfilCasino_idperfilCasino, int tipoComida_idtipoComida, int numeroDia, int habilitado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update detallePerfilCasino set perfilCasino_idperfilCasino=" + perfilCasino_idperfilCasino + ", tipoComida_idtipoComida=" + tipoComida_idtipoComida + ", numeroDia=" + numeroDia + ", habilitado=" + habilitado + " ,updated_at='" + updated_at + "' where iddetallePerfilCasino=" + iddetallePerfilCasino + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerUltimoUpdatedAtDetallePerfilDeCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from detallePerfilCasino;";
            return ejecutor.traerDatosDataTable(sql);
        }

    }
}
