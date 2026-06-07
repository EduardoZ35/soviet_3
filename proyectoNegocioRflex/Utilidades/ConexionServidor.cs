using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Data;
using proyectoNegocioRflex.Modelo;

namespace proyectoNegocioRflex.Utilidades
{

    public class ConexionServidor
    {
        private MySqlConnection conn = new MySqlConnection();
        public string _error;

        public ConexionServidor()
        {
            try
            {
                // tests 
                // conn.ConnectionString = "server=54.94.150.117;User Id=root;password=kinflex;Persist Security Info=True;database=relojControl;SslMode=none";
                //   conn.ConnectionString = "server=54.94.150.117;User Id=root;password=kinflex;Persist Security Info=True;database=relojControl;SslMode=none";
                //   conn.ConnectionString = "Server = 54.94.150.117; Database = relojControl; Uid = root; Pwd = kinflex; SSL Mode = Required; CertificateFile = C:\\Users\rflex\\Documents\\pems\\rflexDev.pem;";
                //team rflex
                //  conn.ConnectionString = "Server =  54.207.16.152; Database = relojControl; Uid = root; Password = kinflex; SSL Mode = Required; CertificateFile = C:\\rflexapps\\pems\\client-cert.pem;";
                Herramientas h = new Herramientas();
                conn.ConnectionString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionRelojServidor.txt");
            }
            catch (Exception ex)
            {
                generarLog("ConexionServidor() " + ex.ToString(), "ERROR");
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
                cmd.CommandTimeout = 30; // prevent indefinite hang on slow cloud writes (heartbeat, marks)
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
                generarLog("ConexionServidor: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
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
                da.SelectCommand.CommandTimeout = 50;
                da.Fill(ds);
                da.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (MySqlException ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("ConexionServidor: traerDataTable() " + sql + " " + ex.ToString(), "ERROR");
                return null;
            }
            return ds.Tables[0];
        }

        private DataTable traerDatosServidor()
        {
            Servidor s = new Servidor();
            DataTable dt = s.traerDatosServidor();
            return dt;
        }

        private void generarLog(string error, string tipoLog)
        {
            Herramientas h = new Herramientas();
            h.generarLogReloj(error.ToString(), tipoLog);
        }
    }
}
