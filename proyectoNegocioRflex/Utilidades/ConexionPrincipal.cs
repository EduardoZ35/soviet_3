using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Data;
using proyectoNegocioRflex.Modelo;

namespace proyectoNegocioRflex.Utilidades
{

    public class ConexionPrincipal
    {
        private MySqlConnection conn = new MySqlConnection();
        public string _error;
        public ConexionPrincipal(string codigoEmpresa)
        {
            try
            {
                //tests
                // conn.ConnectionString = "server=54.94.150.117;User Id=root;password=kinflex;Persist Security Info=True;database=" + _codigoEmpresa + "_tisal; SslMode=none";
                //team rflex
                // conn.ConnectionString = "server=54.207.16.152;User Id=root;password=kinflex;Persist Security Info=True;database=" + _codigoEmpresa + "; SslMode=none";

                Herramientas h = new Herramientas();
                conn.ConnectionString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionPrincipal.txt");
                //conn.ConnectionString = "Server =  54.207.16.152; Database = " + _codigoEmpresa + "; Uid = root; Password = kinflex; SSL Mode = Required; CertificateFile = C:\\rflexapps\\pems\\client-cert.pem;";

            }
            catch (Exception ex)
            {
                generarLog("Conexion Principal: ConexionPrincipal() " + ex.ToString(), "ERROR");
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
            bool resp = false;
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
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion Principal: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
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
                generarLog("Conexion Principal: traerDataTable() " + sql + " " + ex.ToString(), "ERROR");
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

        private bool comprobarConexionServidor()
        {
            bool tieneConexion = false;
            Herramientas h = new Herramientas();
            //Comprobamos si el equipo tiene internet
            if (h.pingEquipo("8.8.8.8"))
            {
                string conexionOriginal = conn.ConnectionString;
                conn.ConnectionString += "; Connection Timeout=12;";
                try
                {
                    //Comprobamos el acceso a la base de datos...
                    if (conn.Ping())
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                        conn.Open();
                        conn.Close();
                        conn.ConnectionString = conexionOriginal;
                        tieneConexion = true;
                        conn.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    conn.Dispose();
                    generarLog("ConexionPrincipal: comprobarConexionServidor() " + ex.ToString(), "ERROR");
                }
            }
            else
            {
                generarLog("ConexionPrincipal: comprobarConexionServidor() Equipo Sin Internet No se pudo hacer PING google", "ERROR");
            }

            return tieneConexion;
        }

    }
}


