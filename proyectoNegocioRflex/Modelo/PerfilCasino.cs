using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class PerfilCasino
    {
        public DataTable traerPerfilCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idperfilCasino,nombrePerfilCasino,habilitado,created_at,updated_at from perfilCasino;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerPerfilCasinoPorID(int idperfilCasino)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idperfilCasino,nombrePerfilCasino,habilitado,created_at,updated_at from perfilCasino where idperfilCasino =" + idperfilCasino + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerPerfilesDeCasinoPorEstado(int habilitado)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idperfilCasino,nombrePerfilCasino from perfilCasino where habilitado=" + habilitado + " order by nombrePerfilCasino asc;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerPerfilesDeCasinoNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idperfilCasino,nombrePerfilCasino,habilitado,created_at,updated_at from perfilCasino where updated_at>='" + updated_at + "';";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarPerfilDeCasino(int idperfilCasino, string nombrePerfilCasino, int habilitado, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into perfilCasino(idperfilCasino,nombrePerfilCasino,habilitado,created_at,updated_at)values(" + idperfilCasino + ",'" + nombrePerfilCasino + "'," + habilitado + ",'" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarPerfilDeCasino(int idperfilCasino, string nombrePerfilCasino, int habilitado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update perfilCasino set nombrePerfilCasino='" + nombrePerfilCasino + "',habilitado=" + habilitado + ",updated_at ='" + updated_at + "' where idperfilCasino=" + idperfilCasino + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerUltimoUpdatedAtPerfilDeCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from perfilCasino;";
            return ejecutor.traerDatosDataTable(sql);
        }
    }
}
