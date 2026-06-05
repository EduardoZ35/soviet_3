using MySql.Data.MySqlClient;
using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class ImagenHuella
    {
        private string _ip;
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _conString;
        public string ConString
        {
            get { return _conString; }
            set { _conString = value; }
        }

        private string _conStringServ;
        public string ConStringServ
        {
            get { return _conStringServ; }
            set { _conStringServ = value; }
        }

        public ImagenHuella()
        {

        }

        public bool guardarHuella(int empresa_idempresa, string persona_rut, int numeroDedo, byte[] imagen_huella,
            string created_by, string updated_by, int respaldado, string equipo, DateTime created_at, DateTime updated_at)
        {

            Herramientas h = new Herramientas();
            _conString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionLocal.txt");
            MySqlConnection con = new MySqlConnection(_conString);
            MySqlCommand cmd;
            try
            {
                string CmdString = "insert into imagenHuella(empresa_idempresa,persona_rut,numeroDedo,imagen_huella,created_by,updated_by,respaldado,nombreEquipoEdicion,created_at,updated_at) values " +
                   "( @empresa_idempresa,@persona_rut,@numeroDedo,@imagen_huella ,@created_by,@updated_by,@respaldado,@nombreEquipoEdicion,@created_at,@updated_at)";

                cmd = new MySqlCommand(CmdString, con);
                cmd.Parameters.Add("@empresa_idempresa", MySqlDbType.Int16);
                cmd.Parameters.Add("@persona_rut", MySqlDbType.VarChar);
                cmd.Parameters.Add("@numeroDedo", MySqlDbType.Int16);
                cmd.Parameters.Add("@imagen_huella", MySqlDbType.Blob);
                cmd.Parameters.Add("@created_by", MySqlDbType.VarChar);
                cmd.Parameters.Add("@updated_by", MySqlDbType.VarChar);
                cmd.Parameters.Add("@respaldado", MySqlDbType.Bit);
                cmd.Parameters.Add("@created_at", MySqlDbType.DateTime);
                cmd.Parameters.Add("@updated_at", MySqlDbType.DateTime);
                cmd.Parameters.Add("@nombreEquipoEdicion", MySqlDbType.VarChar);


                cmd.Parameters["@empresa_idempresa"].Value = empresa_idempresa;
                cmd.Parameters["@persona_rut"].Value = persona_rut;
                cmd.Parameters["@numeroDedo"].Value = numeroDedo;
                cmd.Parameters["@imagen_huella"].Value = imagen_huella;
                cmd.Parameters["@created_by"].Value = created_by;
                cmd.Parameters["@updated_by"].Value = updated_by;
                cmd.Parameters["@respaldado"].Value = respaldado;
                cmd.Parameters["@created_at"].Value = created_at.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters["@updated_at"].Value = updated_at.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters["@nombreEquipoEdicion"].Value = equipo;
                con.Open();

                int RowsAffected = cmd.ExecuteNonQuery();

                bool resp = RowsAffected > 0;
                if (RowsAffected > 0)
                    con.Close();
                cmd.Dispose();
                con.Dispose();
                return resp;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();
                h.generarLogReloj("guardarHuella() " + ex.ToString(), "ERROR");
                return false;
            }
        }

        public DataTable traerHuellaPorRutPersona(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,numeroDedo,imagen_huella from imagenHuella where persona_rut='" + persona_rut + "' limit 10";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool eliminarHuellaTrabajadorRut(string persona_rut, int numeroDedo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "delete from imagenHuella where persona_rut='" + persona_rut + "' and numeroDedo=" + numeroDedo + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerHuellaPorRutYNumeroDedo(string persona_rut, int numeroDedo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella from imagenHuella where persona_rut='" + persona_rut + "' and numeroDedo=" + numeroDedo + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool editarHuella(byte[] imagen_huella, string updated_by, int respaldado, int idimagenHuella)
        {
            Herramientas h = new Herramientas();
            _conString = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionLocal.txt");
            MySqlConnection con = new MySqlConnection(_conString);
            MySqlCommand cmd;
            try
            {
                string CmdString = "update imagenHuella set imagen_huella= @imagen_huella ,updated_by= @updated_by, respaldado= @respaldado,nombreEquipoEdicion=@nombreEquipoEdicion where idimagenHuella= @idimagenHuella";

                cmd = new MySqlCommand(CmdString, con);
                cmd.Parameters.Add("@idimagenHuella", MySqlDbType.Int16);
                cmd.Parameters.Add("@imagen_huella", MySqlDbType.Blob);
                cmd.Parameters.Add("@updated_by", MySqlDbType.VarChar);
                cmd.Parameters.Add("@respaldado", MySqlDbType.Int16);
                cmd.Parameters.Add("@nombreEquipoEdicion", MySqlDbType.VarChar);

                cmd.Parameters["@idimagenHuella"].Value = idimagenHuella;
                cmd.Parameters["@imagen_huella"].Value = imagen_huella;
                cmd.Parameters["@updated_by"].Value = updated_by;
                cmd.Parameters["@respaldado"].Value = respaldado;
                cmd.Parameters["@nombreEquipoEdicion"].Value = Environment.MachineName;
                con.Open();

                int RowsAffected = cmd.ExecuteNonQuery();
                bool resp = RowsAffected > 0;
                if (RowsAffected > 0)
                    con.Close();

                cmd.Dispose();
                return resp;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                h.generarLogReloj("editarHuella()" + ex.ToString(), "ERROR");
                return false;
            }
        }

        public bool guardarHuellaServidor(int empresa_idempresa, string persona_rut, int numeroDedo, byte[] imagen_huella,
        string created_by, string updated_by, int respaldado, DateTime created_at, DateTime updated_at, string equipo)
        {
            Herramientas h = new Herramientas();
            _conStringServ = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionRelojServidor.txt");
            MySqlConnection con = new MySqlConnection(_conStringServ);
            MySqlCommand cmd;
            try
            {
                string CmdString = "insert into imagenHuella(empresa_idempresa,persona_rut,numeroDedo,imagen_huella,created_by,updated_by,respaldado,nombreEquipoEdicion,created_at,updated_at) values " +
                   "( @empresa_idempresa,@persona_rut,@numeroDedo,@imagen_huella ,@created_by,@updated_by,@respaldado,@nombreEquipoEdicion,@created_at,@updated_at)";

                cmd = new MySqlCommand(CmdString, con);
                cmd.Parameters.Add("@empresa_idempresa", MySqlDbType.Int16);
                cmd.Parameters.Add("@persona_rut", MySqlDbType.VarChar);
                cmd.Parameters.Add("@numeroDedo", MySqlDbType.Int16);
                cmd.Parameters.Add("@imagen_huella", MySqlDbType.Blob);
                cmd.Parameters.Add("@created_by", MySqlDbType.VarChar);
                cmd.Parameters.Add("@updated_by", MySqlDbType.VarChar);
                cmd.Parameters.Add("@respaldado", MySqlDbType.Bit);
                cmd.Parameters.Add("@nombreEquipoEdicion", MySqlDbType.VarChar);
                cmd.Parameters.Add("@created_at", MySqlDbType.DateTime);
                cmd.Parameters.Add("@updated_at", MySqlDbType.DateTime);

                cmd.Parameters["@empresa_idempresa"].Value = empresa_idempresa;
                cmd.Parameters["@persona_rut"].Value = persona_rut;
                cmd.Parameters["@numeroDedo"].Value = numeroDedo;
                cmd.Parameters["@imagen_huella"].Value = imagen_huella;
                cmd.Parameters["@created_by"].Value = created_by;
                cmd.Parameters["@updated_by"].Value = updated_by;
                cmd.Parameters["@respaldado"].Value = respaldado;
                cmd.Parameters["@nombreEquipoEdicion"].Value = equipo;
                cmd.Parameters["@created_at"].Value = created_at.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters["@updated_at"].Value = updated_at.ToString("yyyy-MM-dd HH:mm:ss");
                con.Open();

                int RowsAffected = cmd.ExecuteNonQuery();
                bool resp = RowsAffected > 0;
                if (RowsAffected > 0)
                    con.Close();

                cmd.Dispose();
                return resp;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                h.generarLogReloj("guardarHuellaServidor() " + ex.ToString(), "ERROR");
                return false;
            }
        }


        public DataTable traerHuellasSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,empresa_idempresa,persona_rut,numeroDedo,imagen_huella,nombreEquipoEdicion " +
                "created_by,updated_by,created_at,updated_at,respaldado from imagenHuella where respaldado=0 order by persona_rut asc";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerHuellasSinSincronizarServidor(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,empresa_idempresa,persona_rut,numeroDedo,imagen_huella,nombreEquipoEdicion, " +
                "created_by,updated_by,created_at,updated_at,respaldado from imagenHuella where updated_at>='" + updated_at + "' order by persona_rut asc";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        /**
         * Función que utilizamos para consultar si hay nuevos registros de huellas. 
         * Es simplificado ya que hay otra consulta que trae los datos reales de las huellas dependiendo del rut de trabajador
         * esta consulta la usaremos sólo para saber si hay datos nuevos.
         */
        public DataTable traerIndiciosDeRegistrosDeHuellaSinSincronizarServidorSimplificado(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,persona_rut from imagenHuella where updated_at>='" + updated_at + "' order by updated_at, persona_rut asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public DataTable consultarRegistroHuellaExistenteServidorPorRutNumeroDedo(string rut, int numeroDedo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella from imagenHuella where persona_rut ='" + rut + "'  and numeroDedo=" + numeroDedo + " limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerHuellaPorRutPersonaCompleto(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,empresa_idempresa,persona_rut,numeroDedo,imagen_huella,nombreEquipoEdicion, " +
                "created_by,updated_by,created_at,updated_at,respaldado from imagenHuella where persona_rut='" + persona_rut + "'";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerHuellaPorRutPersonaCompletoServidor(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idimagenHuella,empresa_idempresa,persona_rut,numeroDedo,imagen_huella,nombreEquipoEdicion, " +
                "created_by,updated_by,created_at,updated_at,respaldado from imagenHuella where persona_rut='" + persona_rut + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool eliminarHuellasTrabajadorRutLocal(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "delete from imagenHuella where persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool eliminarHuellasTrabajadorRutServidor(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "delete from imagenHuella where persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarEstadoRegistroHuellaLocal(int idimagenHuella, DateTime updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string updatedFormateado = h.formatearFecha(updated_at, true);
            string sql = "update imagenHuella set respaldado=1,updated_at='" + updatedFormateado + "' where idimagenHuella=" + idimagenHuella + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerUltimoUpdatedAtImagenHuella()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from imagenHuella where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        // ==================== Set-diff download methods ====================

        /// <summary>
        /// Returns DISTINCT (persona_rut, numeroDedo) from cloud filtered by
        /// FuncionarioHasReloj for the given device. Use for Asistencia/Casino.
        /// persona_rut is plain text (NOT encrypted) in the cloud DB.
        /// </summary>
        public DataTable traerCombinacionesRutDedoEnNubePorReloj(int idReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT DISTINCT ih.persona_rut, ih.numeroDedo " +
                         "FROM imagenHuella ih " +
                         "INNER JOIN FuncionarioHasReloj fhr ON fhr.persona_rut = ih.persona_rut " +
                         "WHERE fhr.reloj_idreloj = " + idReloj + " " +
                         "ORDER BY ih.persona_rut, ih.numeroDedo";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        /// <summary>
        /// Returns DISTINCT (persona_rut, numeroDedo) from cloud — no FHR filter.
        /// Use for Enrolador devices that need all fingerprints.
        /// persona_rut is plain text (NOT encrypted) in the cloud DB.
        /// </summary>
        public DataTable traerCombinacionesRutDedoEnNube()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT DISTINCT persona_rut, numeroDedo FROM imagenHuella ORDER BY persona_rut, numeroDedo";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        /// <summary>
        /// Returns DISTINCT (persona_rut, numeroDedo) from local DB.
        /// persona_rut is encrypted in local DB — caller must decrypt for comparison.
        /// Columns: [0]=persona_rut (enc), [1]=numeroDedo
        /// </summary>
        public DataTable traerCombinacionesRutDedoLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT DISTINCT persona_rut, numeroDedo FROM imagenHuella ORDER BY persona_rut, numeroDedo";
            return ejecutor.traerDatosDataTable(sql);
        }

        /// <summary>
        /// Returns full row for one (rut, numeroDedo) combo from cloud.
        /// rut must be plain text (unencrypted).
        /// Columns: [0]=idimagenHuella,[1]=empresa_idempresa,[2]=persona_rut,[3]=numeroDedo,
        ///          [4]=imagen_huella,[5]=nombreEquipoEdicion,[6]=created_by,[7]=updated_by,
        ///          [8]=created_at,[9]=updated_at,[10]=respaldado
        /// </summary>
        public DataTable traerHuellaCompletaServidorPorRutDedo(string rut, int numeroDedo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT idimagenHuella,empresa_idempresa,persona_rut,numeroDedo,imagen_huella," +
                         "nombreEquipoEdicion,created_by,updated_by,created_at,updated_at,respaldado " +
                         "FROM imagenHuella WHERE persona_rut='" + EscaparValor(rut) + "' AND numeroDedo=" + numeroDedo + " LIMIT 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        // ==================== Upsert upload method ====================

        /// <summary>
        /// Updates an existing cloud row by primary key (no DELETE needed).
        /// Replaces the old delete+reinsert pattern that failed silently due to missing DELETE permission.
        /// </summary>
        public bool actualizarHuellaServidor(int idimagenHuella, byte[] imagen_huella, int respaldado, DateTime updated_at)
        {
            Herramientas h = new Herramientas();
            _conStringServ = h.traerCadenaConexion("C:\\rflexapps\\config\\conexionRelojServidor.txt");
            MySqlConnection con = new MySqlConnection(_conStringServ);
            MySqlCommand cmd;
            try
            {
                string CmdString = "UPDATE imagenHuella SET imagen_huella=@img, respaldado=@resp, updated_at=@ua " +
                                   "WHERE idimagenHuella=@id";
                cmd = new MySqlCommand(CmdString, con);
                cmd.Parameters.Add("@img", MySqlDbType.Blob);
                cmd.Parameters.Add("@resp", MySqlDbType.Int16);
                cmd.Parameters.Add("@ua", MySqlDbType.DateTime);
                cmd.Parameters.Add("@id", MySqlDbType.Int32);
                cmd.Parameters["@img"].Value = imagen_huella;
                cmd.Parameters["@resp"].Value = respaldado;
                cmd.Parameters["@ua"].Value = updated_at.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters["@id"].Value = idimagenHuella;
                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open) con.Close();
                cmd.Dispose();
                con.Dispose();
                return rows > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();
                h.generarLogReloj("actualizarHuellaServidor() " + ex.ToString(), "ERROR");
                return false;
            }
        }

        private static string EscaparValor(string s)
        {
            return s?.Replace("'", "''") ?? string.Empty;
        }

    }
}
