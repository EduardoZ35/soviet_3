using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{

    public class Reloj
    {
        public bool agregarReloj(int idreloj, int empresa_idempresa, int sucursal_idsucursal, string nombre, string ubicacion, int estado,
           string mac_reloj, string ip_reloj, DateTime fecha_activo, int relojCasino, DateTime updated_at, int marcaComida, int marca_jornada, int mostrar_login_inicio,
           int iniciarDesdePantallaDeMarca, int respaldado, int bloqueado, string nombreEquipo, int soloEnrolador, int resolucionMarca_idresolucionMarca, int autoIniciaPrograma, int horaInicioRangoActualizacion,
           int horaTerminoRangoActualizacion, int imprimeTicketAsistencia, int tipoReloj_idtipoReloj, int compruebaConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string fechaActivoFormateado = h.formatearFecha(fecha_activo, true);
            string sql = "insert into reloj(idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada," +
             "mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado," +
             "bloqueado,nombreEquipoEdicion,relojCasino,updated_at,soloEnrolador,resolucionMarca_idresolucionMarca,autoIniciaPrograma,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,imprimeTicketAsistencia,tipoReloj_idtipoReloj,compruebaConexion) " +
             "values(" + idreloj + "," + empresa_idempresa + "," + sucursal_idsucursal + ",'" + nombre + "','" + ubicacion + "'," + estado + ",'" +
              mac_reloj + "','" + ip_reloj + "','" + fechaActivoFormateado + "'," + marcaComida + "," + marca_jornada + "," +
              mostrar_login_inicio + "," + iniciarDesdePantallaDeMarca + "," + respaldado + "," +
              bloqueado + ",'" + nombreEquipo + "'," + relojCasino + ",'" + fechaActualizacion + "'," + soloEnrolador + "," + resolucionMarca_idresolucionMarca + "," + autoIniciaPrograma + "," + horaInicioRangoActualizacion + "," + horaTerminoRangoActualizacion + "," + imprimeTicketAsistencia + "," + tipoReloj_idtipoReloj + "," + compruebaConexion + ");";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool editarReloj(int sucursal_idsucursal, string nombre, string ubicacion, int estado,
          string mac_reloj, string ip_reloj, DateTime fecha_activo, int relojCasino, DateTime updated_at, int marcaComida, int marca_jornada, int mostrar_login_inicio,
          int iniciarDesdePantallaDeMarca, int respaldado, int bloqueado, string nombreEquipo, int soloEnrolador, int resolucionMarca_idresolucionMarca, int autoIniciaPrograma,
          int horaInicioRangoActualizacion, int horaTerminoRangoActualizacion, int imprimeTicketAsistencia, int compruebaConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string fechaActivo = h.formatearFecha(fecha_activo, true);
            string sql = "update reloj set sucursal_idsucursal=" + sucursal_idsucursal + ", nombre='" + nombre + "', ubicacion='" + ubicacion + "', estado=" + estado +
             ",mac_reloj='" + mac_reloj + "', ip_reloj='" + ip_reloj + "', fecha_activo='" + fechaActivo + "' ,marca_comida=" + marcaComida + ",marca_jornada=" + marca_jornada +
             ",mostrar_login_inicio=" + mostrar_login_inicio + ",iniciarDesdePantallaDeMarca=" + iniciarDesdePantallaDeMarca + ",respaldado=" + respaldado +
             ",bloqueado=" + bloqueado + ",nombreEquipoEdicion='" + nombreEquipo + "',relojCasino=" + relojCasino + " , updated_at ='" + fechaActualizacion +
             "',soloEnrolador=" + soloEnrolador + ",resolucionMarca_idresolucionMarca=" + resolucionMarca_idresolucionMarca + ",autoIniciaPrograma=" + autoIniciaPrograma + ",horaInicioRangoActualizacion=" +
             horaInicioRangoActualizacion + ",horaTerminoRangoActualizacion=" + horaTerminoRangoActualizacion + ",imprimeTicketAsistencia=" + imprimeTicketAsistencia + ",compruebaConexion=" + compruebaConexion + " where nombre ='" + nombre + "'";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerDatosRelojPorNombre(string nombre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada,mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado,bloqueado,nombreEquipoEdicion,relojCasino,updated_at,soloEnrolador,imprimeTicketAsistencia,resolucionMarca_idresolucionMarca,autoIniciaPrograma " +
             " ,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,compruebaConexion from reloj where nombre ='" + nombre + "' limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDatosRelojPorIdReloj(int idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada,mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado,bloqueado,nombreEquipoEdicion,relojCasino,updated_at,soloEnrolador,imprimeTicketAsistencia,resolucionMarca_idresolucionMarca,autoIniciaPrograma " +
             " ,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,compruebaConexion from reloj where idreloj ='" + idreloj + "' limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerRelojesEmpresa()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj.idreloj, " +
            "empresa.razon_social, " +
            "sucursal.nombre as sucursal, " +
            "reloj.nombre as nombreReloj, " +
            "reloj.ubicacion," +
            "reloj.estado, " +
            "reloj.ip_reloj, " +
            "reloj.iniciarDesdePantallaDeMarca, " +
            "reloj.marca_comida, " +
            "reloj.marca_jornada, " +
            "reloj.mostrar_login_inicio, " +
            "reloj.bloqueado, " +
            "reloj.updated_at, " +
            "reloj.nombreEquipoEdicion, " +
            "reloj.relojCasino, " +
            "reloj.soloEnrolador, " +
            "reloj.ip_reloj, " +
            "reloj.mac_reloj, " +
            "reloj.resolucionMarca_idresolucionMarca, " +
            "reloj.autoIniciaPrograma," +
            "reloj.horaInicioRangoActualizacion," +
            "reloj.horaTerminoRangoActualizacion, " +
            "reloj.imprimeTicketAsistencia, " +
            "reloj.compruebaConexion " +
            "from reloj,empresa,sucursal " +
            "where reloj.empresa_idempresa = empresa.idempresa and " +
            "reloj.sucursal_idsucursal = sucursal.idsucursal";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerRelojesEmpresaSinRelojesCasinoNiEnrolador()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj.idreloj, " +
            "empresa.razon_social, " +
            "sucursal.nombre as sucursal, " +
            "reloj.nombre as nombreReloj, " +
            "reloj.ubicacion," +
            "reloj.estado, " +
            "reloj.ip_reloj, " +
            "reloj.iniciarDesdePantallaDeMarca, " +
            "reloj.marca_comida, " +
            "reloj.marca_jornada, " +
            "reloj.mostrar_login_inicio, " +
            "reloj.bloqueado, " +
            "reloj.updated_at, " +
            "reloj.nombreEquipoEdicion, " +
            "reloj.relojCasino, " +
            "reloj.soloEnrolador, " +
            "reloj.ip_reloj, " +
            "reloj.mac_reloj, " +
            "reloj.resolucionMarca_idresolucionMarca, " +
            "reloj.autoIniciaPrograma," +
            "reloj.horaInicioRangoActualizacion," +
            "reloj.horaTerminoRangoActualizacion, " +
            "reloj.imprimeTicketAsistencia, " +
            "reloj.compruebaConexion " + 
            "from reloj,empresa,sucursal " +
            "where reloj.empresa_idempresa = empresa.idempresa and " +
            "reloj.sucursal_idsucursal = sucursal.idsucursal and " +
            "reloj.relojCasino=0 and " +
            "reloj.soloEnrolador=0;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarConfiguracionReloj(int estado, int marca_comida, int marca_jornada, int mostrar_login_inicio, int iniciarDesdePantallaDeMarca, int relojCasino, int sucrusal_idsucursal, string nombreEquipo, DateTime updated_at, int respaldado, string ubicacion, int soloEnrolador, string nombreReloj, string ipreloj, string mac_reloj, int resolucionMarca_idresolucionMarca, int autoIniciaPrograma,
            int horaInicioRangoActualizacion, int horaTerminoRangoActualizacion, int imprimeTicketAsistencia, int compruebaConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string sql = "update reloj set estado=" + estado + ",marca_jornada=" + marca_jornada + " , marca_comida=" + marca_comida +
                ",mostrar_login_inicio=" + mostrar_login_inicio +
                ",iniciarDesdePantallaDeMarca=" + iniciarDesdePantallaDeMarca +
                ",respaldado=" + respaldado + ", nombreEquipoEdicion='" + nombreEquipo + "', relojCasino=" + relojCasino +
                ",sucursal_idsucursal=" + sucrusal_idsucursal + ", updated_at='" + fechaActualizacion + "',ubicacion='" + ubicacion + "',soloEnrolador=" + soloEnrolador + ",ip_reloj='" + ipreloj + "',mac_reloj='" + mac_reloj + "',resolucionMarca_idresolucionMarca=" + resolucionMarca_idresolucionMarca + ",autoIniciaPrograma=" + autoIniciaPrograma +
                ",horaInicioRangoActualizacion=" + horaInicioRangoActualizacion + " ,horaTerminoRangoActualizacion=" + horaTerminoRangoActualizacion + ",imprimeTicketAsistencia=" + imprimeTicketAsistencia + ",compruebaConexion=" + compruebaConexion + " where nombre='" + nombreReloj + "'";
            return ejecutor.ejecutarConsulta(sql);
        }



        public bool actualizarConfiguracionRelojPorID(int estado, int marca_comida, int marca_jornada, int mostrar_login_inicio, int iniciarDesdePantallaDeMarca, int relojCasino, int sucrusal_idsucursal, string nombreEquipo, DateTime updated_at, int respaldado, string ubicacion, int soloEnrolador, string nombreReloj, string ipreloj, string mac_reloj, int resolucionMarca_idresolucionMarca, int autoIniciaPrograma,
            int horaInicioRangoActualizacion, int horaTerminoRangoActualizacion, int idreloj, int imprimeTicketAsistencia, int tipoReloj_idtipoReloj, int empresa_idempresa, int compruebaConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string sql = "update reloj set estado=" + estado + ",marca_jornada=" + marca_jornada + " , marca_comida=" + marca_comida +
                ",mostrar_login_inicio=" + mostrar_login_inicio +
                ",iniciarDesdePantallaDeMarca=" + iniciarDesdePantallaDeMarca +
                ",respaldado=" + respaldado + ", nombreEquipoEdicion='" + nombreEquipo + "', relojCasino=" + relojCasino +
                ",sucursal_idsucursal=" + sucrusal_idsucursal + ", updated_at='" + fechaActualizacion + "',ubicacion='" + ubicacion + "',soloEnrolador=" + soloEnrolador + ",ip_reloj='" + ipreloj + "',mac_reloj='" + mac_reloj + "',resolucionMarca_idresolucionMarca=" + resolucionMarca_idresolucionMarca + ",autoIniciaPrograma=" + autoIniciaPrograma +
                ",horaInicioRangoActualizacion=" + horaInicioRangoActualizacion + " ,horaTerminoRangoActualizacion=" + horaTerminoRangoActualizacion + ",nombre='" + nombreReloj + "',imprimeTicketAsistencia=" + imprimeTicketAsistencia + ",tipoReloj_idtipoReloj=" + tipoReloj_idtipoReloj + ",empresa_idempresa=" + empresa_idempresa + ", compruebaConexion=" + compruebaConexion + " where idreloj='" + idreloj + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarConfiguracionRelojNube(int estado, int marca_comida, int marca_jornada, int mostrar_login_inicio, string ip_reloj, int iniciarDesdePantallaDeMarca, int relojCasino, int sucrusal_idsucursal, DateTime updated_at, int respaldado, string ubicacion, string nombreReloj, int resolucionMarca_idresolucionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string sql = "update reloj set estado=" + estado + ",marca_jornada=" + marca_jornada + " , marca_comida=" + marca_comida +
                ",mostrar_login_inicio=" + mostrar_login_inicio +
                ",iniciarDesdePantallaDeMarca=" + iniciarDesdePantallaDeMarca + ", " +
                "respaldado=" + respaldado + ", relojCasino=" + relojCasino +
                ",sucursal_idsucursal=" + sucrusal_idsucursal + ", updated_at='" + fechaActualizacion + "',ubicacion='" + ubicacion + "',ip_reloj='" + ip_reloj + "',resolucionMarca_idresolucionMarca=" + resolucionMarca_idresolucionMarca + " where nombre='" + nombreReloj + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public DataTable traerDatosRelojPorNombreNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada,mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado,bloqueado,nombreEquipoEdicion,relojCasino " +
             ",updated_at,soloEnrolador,resolucionMarca_idresolucionMarca,autoIniciaPrograma,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,imprimeTicketAsistencia,compruebaConexion from reloj where nombre ='" + Environment.MachineName + "' and updated_at >='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public DataTable traerDatosRelojPorNombrePendienteSubir(string nombre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada,mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado,bloqueado,nombreEquipoEdicion,relojCasino,updated_at,soloEnrolador,resolucionMarca_idresolucionMarca,autoIniciaPrograma,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,imprimeTicketAsistencia,compruebaConexion " +
             "from reloj where nombre ='" + nombre + "' and respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDatosRelojEnNubePorSincronizar(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj,empresa_idempresa, sucursal_idsucursal, nombre, ubicacion, estado," +
             "mac_reloj, ip_reloj, fecha_activo,marca_comida,marca_jornada,mostrar_login_inicio,iniciarDesdePantallaDeMarca,respaldado,bloqueado,nombreEquipoEdicion,relojCasino,updated_at,soloEnrolador,resolucionMarca_idresolucionMarca,autoIniciaPrograma,horaInicioRangoActualizacion,horaTerminoRangoActualizacion,imprimeTicketAsistencia,tipoReloj_idtipoReloj,compruebaConexion " +
             "from reloj where updated_at >='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool actualizarConfiguracionRelojEstadoRespaldado()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update reloj set respaldado=1 where nombre='" + Environment.MachineName + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerEstadoBloqueoDatosRelojPorNombreNube()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select bloqueado,idreloj,autoIniciaPrograma,soloEnrolador,relojCasino,mostrar_login_inicio,compruebaConexion from reloj where nombre ='" + Environment.MachineName + "' limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        // Retorna: idreloj(0), sucursal_idsucursal(1), soloEnrolador(2), relojCasino(3), bloqueado(4), autoIniciaPrograma(5), tipoReloj_idtipoReloj(6)
        public DataTable traerIdentificacionRelojPorNombre()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT idreloj, sucursal_idsucursal, soloEnrolador, relojCasino, " +
                         "bloqueado, autoIniciaPrograma, COALESCE(tipoReloj_idtipoReloj, 1) AS tipoReloj_idtipoReloj " +
                         "FROM reloj WHERE nombre = '" + Environment.MachineName + "' LIMIT 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool BootstrapCompletado()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT bootstrap_completado FROM reloj WHERE nombre = '" +
                         Environment.MachineName + "' LIMIT 1";
            DataTable dt = ejecutor.traerDatosDataTable(sql);
            if (dt == null || dt.Rows.Count == 0) return false;
            return dt.Rows[0][0].ToString() == "1";
        }

        public bool MarcarBootstrapCompletado(int idReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "UPDATE reloj SET bootstrap_completado = 1 WHERE idreloj = " + idReloj;
            return ejecutor.ejecutarConsulta(sql);
        }

        public void AplicarMigracionBootstrapCompletado()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string checkSql = "SELECT COUNT(*) FROM information_schema.COLUMNS " +
                              "WHERE TABLE_SCHEMA = DATABASE() " +
                              "AND TABLE_NAME = 'reloj' " +
                              "AND COLUMN_NAME = 'bootstrap_completado'";
            DataTable dt = ejecutor.traerDatosDataTable(checkSql);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "0")
            {
                string alterSql = "ALTER TABLE reloj ADD COLUMN bootstrap_completado TINYINT(1) DEFAULT 0";
                ejecutor.ejecutarConsulta(alterSql);
            }
        }

        // P/Invoke para leer estado real de energía sin depender de System.Windows.Forms
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;       // 0=batería, 1=corriente, 255=desconocido
            public byte BatteryFlag;        // 128=sin batería física, 255=desconocido
            public byte BatteryLifePercent; // 0–100, 255=desconocido
            public byte SystemStatusFlag;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
        }

        public bool ReportarHeartbeatNube(int idReloj)
        {
            long espacioMB = 0;
            try
            {
                var drive = new System.IO.DriveInfo("C");
                espacioMB = drive.AvailableFreeSpace / (1024L * 1024L);
            }
            catch { /* ignorar si no se puede leer disco */ }

            int enBateria = 0;
            int porcentaje = 100;
            string comentario = "PC conectado a corriente. Sin bateria fisica.";
            try
            {
                if (GetSystemPowerStatus(out SYSTEM_POWER_STATUS pwr))
                {
                    bool sinBateria = pwr.BatteryFlag == 128 || pwr.BatteryFlag == 255;
                    enBateria   = (!sinBateria && pwr.ACLineStatus == 0) ? 1 : 0;
                    porcentaje  = (pwr.BatteryLifePercent <= 100) ? pwr.BatteryLifePercent : 100;
                    if (sinBateria)
                        comentario = "PC conectado a corriente. Sin bateria fisica.";
                    else if (enBateria == 1)
                        comentario = $"PC en bateria. {porcentaje}%.";
                    else
                        comentario = $"PC conectado a corriente. Bateria: {porcentaje}%.";
                }
            }
            catch { /* ignorar error leyendo estado de energia */ }

            string sql =
                "INSERT INTO avisoConexion " +
                "(reloj_idreloj, fechaLocalEquipo, comentario, funcionandoConBateria, porcentajeBateria, espacioDisponible) " +
                "VALUES (" +
                idReloj + ", " +
                "NOW(), " +
                "'" + comentario.Replace("'", "''") + "', " +
                enBateria + ", " +
                porcentaje + ", " +
                espacioMB + ")";

            EjecutoresSql ejecutor = new EjecutoresSql();
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarConfiguracionRelojEstadoRespaldadoPorId(int ipReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update reloj set respaldado=1 where idreloj='" + ipReloj + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerRelojesEmpresaPorCodigoEmpresa(string directorioConexion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj.idreloj, " +
            "reloj.nombre as nombreReloj, " +
            "reloj.ubicacion," +
            "reloj.estado, " +
            "reloj.marca_comida, " +
            "reloj.marca_jornada, " +
            "reloj.relojCasino, " +
            "reloj.soloEnrolador, " +
            "reloj.bloqueado, " +
            "reloj.autoIniciaPrograma, " +
            "reloj.horaInicioRangoActualizacion, " +
            "reloj.horaTerminoRangoActualizacion " +
            "from reloj ";
            return ejecutor.traerDatosConexionDinamica(sql, directorioConexion);
        }

        public DataTable consultarSiHayRelojCasinoEmpresa()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj from reloj where relojCasino=1 limit 1 ";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable consultarSiHayRelojConMarcaDeComidas()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj from reloj where marca_comida=1 limit 1 ";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarConfiguracionResolucionPantallaMarca(int resolucionMarca_idresolucionMarca, string updated_at, int idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaActualizacion = h.formatearFecha(DateTime.Parse(updated_at), true);
            string sql = "update reloj set resolucionMarca_idresolucionMarca=" + resolucionMarca_idresolucionMarca + ", respaldado=0, nombreEquipoEdicion='" + Environment.MachineName + "', updated_at='" + fechaActualizacion + "' where idreloj=" + idreloj + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerResolucionPantallaMarcaRelojPorID(int idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select resolucionMarca_idresolucionMarca from reloj where idreloj =" + idreloj + " limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public DataTable traerDatosSucursalReloj()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select sucursal.idsucursal, sucursal.empresa_idempresa, sucursal.nombre, sucursal.rut_encargado, sucursal.nombre_encargado, sucursal.calle," +
            "sucursal.numero_calle, sucursal.comuna, sucursal.ciudad, sucursal.region, sucursal.pais,sucursal.sucursalActual from sucursal " +
            "inner join reloj on reloj.sucursal_idsucursal = sucursal.idsucursal where reloj.nombre = '" + Environment.MachineName + "'"; ;
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerDatoEsEnroladorRelojPorIdReloj(int idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select soloEnrolador from reloj where idreloj ='" + idreloj + "' limit 1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerIdRelojPorNombre(string nombre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idreloj from reloj where nombre='" + nombre + "' limit 1 ";
            return ejecutor.traerDatosDataTable(sql);
        }





    }
}
