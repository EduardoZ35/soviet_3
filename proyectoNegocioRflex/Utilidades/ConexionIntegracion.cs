using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Data;
using proyectoNegocioRflex.Modelo;

namespace proyectoNegocioRflex.Utilidades
{

    public class ConexionIntegracion
    {
        private MySqlConnection conn = new MySqlConnection();
        public string _error;
        public ConexionIntegracion()
        {
            try
            {
                Herramientas h = new Herramientas();
                conn.ConnectionString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionIntegracion.txt");
            }
            catch (Exception ex)
            {
                generarLog("Conexion Integracion() " + ex.ToString(), "ERROR");
                _error = ex.Message;
            }
        }

        public MySqlConnection getConn
        {
            get
            {
                return conn;
            }
        }


        public bool EjecutaSQL(string sql)
        {
            bool resp;
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.ConnectionString += "; Connection Timeout=12;";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int rows = cmd.ExecuteNonQuery();
                resp = rows > 0;
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion Integracion: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
                resp = false;
            }
            return resp;
        }

        public DataTable traerDataTable(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.ConnectionString += "; Connection Timeout=12;";
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.Fill(ds);
                da.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (MySqlException ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion Integracion: traerDataTable() " + sql + " " + ex.ToString(), "ERROR");
                return null;
            }
            return ds.Tables[0];
        }

        private void generarLog(string error, string tipoLog)
        {
            Herramientas h = new Herramientas();
            h.generarLogReloj(error.ToString(), tipoLog);
        }

    }
}