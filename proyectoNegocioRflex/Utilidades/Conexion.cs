using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace proyectoNegocioRflex.Utilidades
{

    public class Conexion
    {
        private MySqlConnection conn = new MySqlConnection();
        public string _error;

        public Conexion()
        {
            try
            {
                Herramientas h = new Herramientas();
                conn.ConnectionString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionLocal.txt");
                // Add connection timeout consistent with all other Conexion* classes.
                // Prevents conn.Open() from hanging indefinitely when MariaDB is slow at boot.
                conn.ConnectionString += "; Connection Timeout=15;";
                //  conn.ConnectionString = "server=" + ip + ";User Id=root;password=root;Persist Security Info=True;database=relojControl;SslMode=none";
            }
            catch (Exception ex)
            {
                generarLog("Conexion()" + ex.ToString(), "ERROR");
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

                conn.Open();

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.CommandTimeout = 30; // prevent indefinite hang on table lock or network stall
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
                generarLog("Conexion: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
                resp = false;
            }
            return resp;
        }

        public DataSet traerDatos(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.SelectCommand.CommandTimeout = 30; // prevent indefinite hang
                da.Fill(ds); // Fill BEFORE Dispose (was inverted — bug fix)
                da.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (MySqlException ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion: traerDatos() " + sql + " " + ex.ToString(), "ERROR");
                return null;
            }
            return ds;
        }

        public DataTable traerDataTable(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.SelectCommand.CommandTimeout = 30; // prevent indefinite hang on table lock
                da.Fill(ds);
                da.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (MySqlException ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion: traerDataTable() " + sql + " " + ex.ToString(), "ERROR");
                return null;
            }
            return ds.Tables[0];
        }

        private void generarLog(string error, string tipoLog)
        {
            Herramientas h = new Herramientas();
            h.generarLogReloj(error.ToString(), tipoLog);
        }

        /**
         * Variante de instrucción de ejecución de sql.
         * Las consultas que alteran los esquemas no retornan una cantidad de filas
         * entonces con la versión comun de consultas siempre se entregaría que el comando falló (retorna 0).
         * lo que vamos a hacer es que si la consulta se completa correctamente daremos como que la consulta se ejecutó
         * correctamente... si se cae.. será interpretado como fallo.
         */
        public bool EjecutaSQLModificacionTabla(string sql)
        {
            bool resp;
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.CommandTimeout = 30; // prevent indefinite hang
                int rows = cmd.ExecuteNonQuery();
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
                resp = true;
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                generarLog("Conexion: EjecutaSQL() " + sql + " " + ex.ToString(), "ERROR");
                resp = false;
            }
            return resp;
        }

    }
}
