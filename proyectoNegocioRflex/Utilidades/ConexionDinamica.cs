using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Utilidades
{
    public class ConexionDinamica
    {
        private MySqlConnection conn = new MySqlConnection();
        public string _error;

        public ConexionDinamica(string cadenaConexion)
        {
            try
            {
                Herramientas h = new Herramientas();
                conn.ConnectionString = h.traerCadenaConexion(cadenaConexion);
            }
            catch
            {
                // cadena de conexión inválida o archivo no encontrado — se retorna conexión vacía
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
                if ((rows == 0))
                {
                    resp = false;
                }
                else
                {
                    resp = true;
                }
                conn.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                conn.Close();
                generarLog("Conexion Dinamica: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
                return false;
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
            }
            catch (MySqlException ex)
            {
                conn.Close();
                generarLog("Conexion Dinamica: traerDataTable() " + sql + " " + ex.ToString(), "ERROR");
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
