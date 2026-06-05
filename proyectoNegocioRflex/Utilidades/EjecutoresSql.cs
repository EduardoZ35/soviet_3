using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Utilidades
{
    public class EjecutoresSql
    {

        public DataTable traerDatosDataTable(string sql)
        {
            Conexion con = new Conexion();
            return con.traerDataTable(sql);
        }

        public bool ejecutarConsulta(string sql)
        {
            Conexion con = new Conexion();
            return con.EjecutaSQL(sql);
        }

        public DataTable traerDatosDataTableServidor(string sql)
        {
            ConexionServidor con = new ConexionServidor();
            return con.traerDataTable(sql);
        }
   
        public bool ejecutarConsultaServidor(string sql)
        {
            ConexionServidor con = new ConexionServidor();
            return con.EjecutaSQL(sql);
        }

        public DataTable traerDatosDataTableBDIntegracion(string sql, string codigoEmpresa)
        {
            ConexionIntegracion con = new ConexionIntegracion();
            return con.traerDataTable(sql);
        }

        public bool ejecutarConsultaBDIntegracion(string sql,string codigoEmpresa)
        {
            ConexionIntegracion con = new ConexionIntegracion();
            return con.EjecutaSQL(sql);
        }

        public DataTable traerDatosDataTableBDPrincipal(string sql, string codigoEmpresa)
        {
            ConexionPrincipal con = new ConexionPrincipal(codigoEmpresa);
            return con.traerDataTable(sql);
        }

        public bool ejecutarConsultaBDPrincipal(string sql, string codigoEmpresa)
        {
            ConexionPrincipal con = new ConexionPrincipal(codigoEmpresa);
            return con.EjecutaSQL(sql);
        }

        public DataTable traerDatosConexionDinamica(string sql, string directorioConexion)
        {
            ConexionDinamica con = new ConexionDinamica(directorioConexion);
            return con.traerDataTable(sql);
        }

        public bool EjecutaSQLModificacionTabla(string sql)
        {
            Conexion con = new Conexion();
            return con.EjecutaSQLModificacionTabla(sql);
        }

        
    }
}
