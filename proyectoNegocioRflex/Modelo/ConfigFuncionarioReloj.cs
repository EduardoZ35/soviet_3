using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class ConfigFuncionarioReloj
    {

        public bool agregarConfigPersona(string persona_rut, int puedeEnrolar, int registra_asistencia,
        int requiere_pin, string pin, string created_by, int respaldado, string passErolar,
        int marcaInicioTerminoComida, string updated_by, int habilitadaRacionCasino, DateTime created_at, DateTime updated_at, string nombreEquipoEdicion, int tipoDetalleMarcaComida_idtipoDetalleMarcaComida, int tipoRolUsuario_idtipoRolUsuario, int sinRestriccionTicket, int marcajeLibre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaCreacionFormateada = h.formatearFecha(created_at, true);
            string fechaEdicionFormateada = h.formatearFecha(updated_at, true);
            string sql = "insert into  config_funcionario_reloj(persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,updated_by,habilitadaRacionCasino,created_at,updated_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre) " +
                "values('" + persona_rut + "'," + puedeEnrolar + "," + registra_asistencia + "," + requiere_pin + ",'" + pin + "','" + created_by + "'," + respaldado + ",'" + passErolar + "'," + marcaInicioTerminoComida + ",'" + nombreEquipoEdicion + "','" + updated_by + "'," + habilitadaRacionCasino + ",'" + fechaCreacionFormateada + "','" + fechaEdicionFormateada + "'," + tipoDetalleMarcaComida_idtipoDetalleMarcaComida + "," + tipoRolUsuario_idtipoRolUsuario + "," + sinRestriccionTicket + "," + marcajeLibre + ")";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool agregarConfigPersonaNube(string persona_rut, int puedeEnrolar, int registra_asistencia,
         int requiere_pin, string pin, string created_by, int respaldado, string passErolar,
         int marcaInicioTerminoComida, string updated_by, int habilitadaRacionCasino, DateTime created_at, DateTime updated_at, int tipoDetalleMarcaComida_idtipoDetalleMarcaComida, int tipoRolUsuario_idtipoRolUsuario, int sinRestriccionTicket, int marcajeLibre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaCreacionFormateada = h.formatearFecha(created_at, true);
            string fechaEdicionFormateada = h.formatearFecha(updated_at, true);
            string sql = "insert into  config_funcionario_reloj(persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,updated_by,habilitadaRacionCasino,created_at,updated_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre) " +
                "values('" + persona_rut + "'," + puedeEnrolar + "," + registra_asistencia + "," + requiere_pin + ",'" + pin + "','" + created_by + "'," + respaldado + ",'" + passErolar + "'," + marcaInicioTerminoComida + ",'" + Environment.MachineName + "','" + updated_by + "'," + habilitadaRacionCasino + ",'" + fechaCreacionFormateada + "','" + fechaEdicionFormateada + "'," + tipoDetalleMarcaComida_idtipoDetalleMarcaComida + "," + tipoRolUsuario_idtipoRolUsuario + "," + sinRestriccionTicket + "," + marcajeLibre + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
        }


        public bool editarConfigPersona(int puedeEnrolar, int registra_asistencia, int requiere_pin,
            string pin, string updated_by, int respaldado, string passErolar, string persona_rut, int marcaInicioTerminoComida, int habilitadaRacionCasino, string equipo, DateTime updated_at, int tipoDetalleMarcaComida_idtipoDetalleMarcaComida, int tipoRolUsuario_idtipoRolUsuario, int sinRestriccionTicket, int marcajeLibre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaFormateada = h.formatearFecha(updated_at, true);
            string sql = "update config_funcionario_reloj set puedeEnrolar=" + puedeEnrolar + ", registra_asistencia=" + registra_asistencia +
                ", requiere_pin=" + requiere_pin + ", pin='" + pin + "', updated_by='" + updated_by + "', respaldado= " + respaldado +
                ",passErolar='" + passErolar + "',marcaInicioTerminoComida=" + marcaInicioTerminoComida + ",nombreEquipoEdicion='" + Environment.MachineName + "',habilitadaRacionCasino=" + habilitadaRacionCasino + ",updated_at='" + fechaFormateada + "',tipoDetalleMarcaComida_idtipoDetalleMarcaComida=" + tipoDetalleMarcaComida_idtipoDetalleMarcaComida + ",tipoRolUsuario_idtipoRolUsuario=" + tipoRolUsuario_idtipoRolUsuario + ",sinRestriccionTicket=" + sinRestriccionTicket + ",marcajeLibre=" + marcajeLibre + " where persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarConfigPersonaActualizarEstadoRespaldado(int idconfig_funcionario_reloj, DateTime updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string updated_atFormateado = h.formatearFecha(updated_at, true);
            string sql = "update config_funcionario_reloj set respaldado=1, updated_at='" + updated_atFormateado + "' where idconfig_funcionario_reloj=" + idconfig_funcionario_reloj + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerConfigsPersonaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,habilitadaRacionCasino,updated_by,idconfig_funcionario_reloj,updated_at,created_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre " +
                "from config_funcionario_reloj where updated_at>='" + updated_at + "' order by updated_at asc";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerConfigsPersonaLocalSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,habilitadaRacionCasino,updated_by,idconfig_funcionario_reloj,updated_at,created_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre " +
                "from config_funcionario_reloj where nombreEquipoEdicion = '" + Environment.MachineName + "' and respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerConfigPersonaLocalPorRut(string rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,habilitadaRacionCasino,updated_by,idconfig_funcionario_reloj,updated_at,created_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre " +
                "from config_funcionario_reloj where persona_rut ='" + rut + "' limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerConfigPersonaNubePorRut(string rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, puedeEnrolar, registra_asistencia, requiere_pin,pin,created_by,respaldado,passErolar,marcaInicioTerminoComida,nombreEquipoEdicion,habilitadaRacionCasino,updated_by,idconfig_funcionario_reloj,updated_at,created_at,tipoDetalleMarcaComida_idtipoDetalleMarcaComida,tipoRolUsuario_idtipoRolUsuario,sinRestriccionTicket,marcajeLibre " +
                "from config_funcionario_reloj where persona_rut = '" + rut + "' limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool editarConfigPersonaNube(int puedeEnrolar, int registra_asistencia, int requiere_pin,
            string pin, string updated_by, int respaldado, string passErolar, string persona_rut, int marcaInicioTerminoComida, int habilitadaRacionCasino, DateTime updated_at, int tipoDetalleMarcaComida_idtipoDetalleMarcaComida, int tipoRolUsuario_idtipoRolUsuario, int sinRestriccionTicket, int marcajeLibre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaFormateada = h.formatearFecha(updated_at, true);
            string sql = "update config_funcionario_reloj set puedeEnrolar=" + puedeEnrolar + ", registra_asistencia=" + registra_asistencia +
                ", requiere_pin=" + requiere_pin + ", pin='" + pin + "', updated_by='" + updated_by + "', respaldado= " + respaldado +
                ",passErolar='" + passErolar + "',marcaInicioTerminoComida=" + marcaInicioTerminoComida + ",nombreEquipoEdicion='" + Environment.MachineName + "',habilitadaRacionCasino=" + habilitadaRacionCasino + ",updated_at ='" + fechaFormateada + "',tipoDetalleMarcaComida_idtipoDetalleMarcaComida=" + tipoDetalleMarcaComida_idtipoDetalleMarcaComida + ",tipoRolUsuario_idtipoRolUsuario=" + tipoRolUsuario_idtipoRolUsuario + ",sinRestriccionTicket=" + sinRestriccionTicket + ",marcajeLibre=" + marcajeLibre + " where persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool editarConfigPersonaDatoCasino(string updated_by, string persona_rut, int habilitadaRacionCasino, string updated_at, int tipoDetalleMarcaComida_idtipoDetalleMarcaComida, int sinRestriccionTicket, int marcajeLibre)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update config_funcionario_reloj set updated_by='" + updated_by + "', nombreEquipoEdicion='" + Environment.MachineName + "',respaldado=0, habilitadaRacionCasino=" + habilitadaRacionCasino + ", updated_at='" + updated_at + "',tipoDetalleMarcaComida_idtipoDetalleMarcaComida=" + tipoDetalleMarcaComida_idtipoDetalleMarcaComida + ",sinRestriccionTicket=" + sinRestriccionTicket + ", marcajeLibre=" + marcajeLibre + " where persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerUltimoUpdatedAtConfigFuncionarioReloj()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from config_funcionario_reloj where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }
    }
}
