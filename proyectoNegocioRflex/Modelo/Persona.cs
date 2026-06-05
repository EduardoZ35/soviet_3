using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using proyectoNegocioRflex.Utilidades;
namespace proyectoNegocioRflex.Modelo
{
    public class Persona
    {

        public DataTable traerPersona(string rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona from persona where rut='" + rut + "' and habilitada=1 limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable existeRegistroPersona(string rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,updated_at from persona where rut='" + rut + "' limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable loginEnrolamiento(string rut, string pass)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona.idpersona,persona.rut, persona.nombre, persona.apellidoPaterno,persona.apellidoMaterno, config_funcionario_reloj.tipoRolUsuario_idtipoRolUsuario " +
                "from persona,config_funcionario_reloj " +
                "where persona.rut='" + rut + "' " +
                "and persona.habilitada=1 " +
                "and persona.rut = config_funcionario_reloj.persona_rut " +
                "and config_funcionario_reloj.puedeEnrolar=1 " +
                "and config_funcionario_reloj.passErolar='" + pass + "'";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarPersona(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno, string rut,
            string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos,
            int respaldado, DateTime activoDesde, string centroCosto, string puesto, string nombreEquipo, DateTime created_at, DateTime updated_at, int habilitada, int empresa_idempresa)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaCreacion = h.formatearFecha(created_at, true);
            string fechaActualizacion = h.formatearFecha(updated_at, true);
            string sql = "insert into persona(nombre, segundoNombre, apellidoPaterno, apellidoMaterno, rut," +
             "correo, alias, fechaNacimiento, telefonoUno, telefonoDos,respaldado,activoDesde,centroCosto,puesto,nombreEquipoEdicion,created_at,updated_at,habilitada,empresa_idempresa) values('" + nombre + "'" +
             ",'" + segundoNombre + "','" + apellidoPaterno + "','" + apellidoMaterno + "','" + rut + "','"
              + correo + "','" + alias + "' ,'" + h.formatearFecha(fechaNacimiento, false) + "','" + telefonoUno + "','" + telefonoDos + "'," + respaldado + ",'" +
              h.formatearFecha(activoDesde, false) + "','" + centroCosto + "','" + puesto + "','" + nombreEquipo + "','" + fechaCreacion + "','" + fechaActualizacion + "'," + habilitada + "," + empresa_idempresa + ")";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarPersona(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno,
             string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos, DateTime activoDesde, int respaldado,
             string centroCosto, string puesto, string rut, string nombreEquipo, string updated_at, int habilitada, int empresa_idempresa)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();

            string sql = "update persona set nombre='" + nombre + "', segundoNombre='" + segundoNombre + "'," +
             " apellidoPaterno='" + apellidoPaterno + "', apellidoMaterno='" + apellidoMaterno + "', rut ='" + rut + "', " +
             " correo='" + correo + "', alias='" + alias + "', fechaNacimiento='" + h.formatearFecha(fechaNacimiento, false) + "', telefonoUno='" +
             telefonoUno + "', telefonoDos= '" + telefonoDos + "',respaldado=" + respaldado + ",activoDesde='" +
             h.formatearFecha(activoDesde, false) + "',centroCosto='" + centroCosto + "',puesto='" + puesto + "',updated_at='" + updated_at + "',nombreEquipoEdicion='" + nombreEquipo + "',habilitada=" + habilitada + ",empresa_idempresa=" + empresa_idempresa + " where rut='" + rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerDatosTrabajadorConConfig(string rutCompleto)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona.idpersona,persona.nombre,persona.segundoNombre,persona.apellidoPaterno," +
            " persona.apellidoMaterno, persona.correo,persona.alias,persona.fechaNacimiento,persona.telefonoUno,persona.telefonoDos,persona.activoDesde," +
            " persona.habilitada,config_funcionario_reloj.puedeEnrolar,config_funcionario_reloj.registra_asistencia," +
            " config_funcionario_reloj.requiere_pin,config_funcionario_reloj.pin," +
            " config_funcionario_reloj.passErolar, persona.centroCosto,persona.puesto,config_funcionario_reloj.marcaInicioTerminoComida,config_funcionario_reloj.habilitadaRacionCasino,config_funcionario_reloj.tipoDetalleMarcaComida_idtipoDetalleMarcaComida,config_funcionario_reloj.tipoRolUsuario_idtipoRolUsuario,persona.empresa_idempresa,config_funcionario_reloj.sinRestriccionTicket,empresa.razon_social,config_funcionario_reloj.marcajeLibre " +
            " from persona,config_funcionario_reloj,empresa where" +
            " persona.rut = '" + rutCompleto + "' and persona.rut = config_funcionario_reloj.persona_rut and " +
            " persona.empresa_idempresa = empresa.idempresa limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool seederPersonaAdmin()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into persona (rut,nombre,segundoNombre,apellidoPaterno,apellidoMaterno,centroCosto,puesto,correo,alias,fechaNacimiento," +
                "telefonoUno,telefonoDos,activoDesde,habilitada,respaldado,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa) values " +
                "('mBDR6TH/fdc=','Qs+Zheurwm4=','Qs+Zheurwm4=','Qs+Zheurwm4=','Qs+Zheurwm4=','CxQ+2Y2eYbY=','ow4rKFVZ0mJCQHzuG80N/g=='," +
                "'RiZ3gWdfMJRR2ZyoxF0m75l29MJ01zE0s+Hf7UpwRTc=','vMKxB9qeQfQ=','2018-04-09 00:00:01','HeKWZRbIf6g='," +
                "'xZvBTpmNxAU=','2018-04-09 00:00:01',1,0,'2018-04-09 16:04:38','2018-04-09 16:04:38','Base',1)";
            bool resp = ejecutor.ejecutarConsulta(sql);
            if (resp)
            {
                sql = " insert into config_funcionario_reloj (persona_rut,puedeEnrolar,registra_asistencia,requiere_pin,pin,passErolar," +
                    "created_by,updated_by,created_at,updated_at,respaldado,nombreEquipoEdicion) values " +
                    "('mBDR6TH/fdc=', 1,0,0, 'ESkaI6DxjEk=', 'Qs+Zheurwm4=', 'mBDR6TH/fdc=', 'mBDR6TH/fdc=', '2018-04-09 16:04:38', '2018-04-09 16:04:38', 1,'Base')";
                resp = ejecutor.ejecutarConsulta(sql);
            }
            return resp;
        }

        // Funciones para sincronizador.
        public DataTable personasSinSincronizarLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,  nombre,segundoNombre,apellidoPaterno, apellidoMaterno, " +
            "centroCosto,puesto,correo,alias, fechaNacimiento, telefonoUno,telefonoDos, " +
            "activoDesde, habilitada,respaldado,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa from persona where respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarPersonaNube(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno, string rut,
        string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos
        , DateTime activoDesde, string centroCosto, string puesto, int habilitada, DateTime created_at, DateTime updated_at, int empresa_idempresa)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into persona(nombre, segundoNombre, apellidoPaterno, apellidoMaterno, rut," +
             "correo, alias, fechaNacimiento, telefonoUno, telefonoDos,respaldado,activoDesde,centroCosto,puesto,habilitada,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa) values('" + nombre + "'" +
             ",'" + segundoNombre + "','" + apellidoPaterno + "','" + apellidoMaterno + "','" + rut + "','"
              + correo + "','" + alias + "' ,'" + h.formatearFecha(fechaNacimiento, false) + "','" + telefonoUno + "','" + telefonoDos + "', 1 ,'" +
              h.formatearFecha(activoDesde, false) + "','" + centroCosto + "','" + puesto + "'," + habilitada + ",'" + h.formatearFecha(created_at, true) + "','" + h.formatearFecha(updated_at, true) +
              "','" + Environment.MachineName + "'," + empresa_idempresa + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
        }


        public bool editarPersonaNube(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno,
             string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos, DateTime activoDesde,
             string centroCosto, string puesto, string rut, int habilitada, int empresa_idempresa)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaSistema = h.obtenerHoraServidor();
            string sql = "update persona set nombre='" + nombre + "', segundoNombre='" + segundoNombre + "'," +
             " apellidoPaterno='" + apellidoPaterno + "', apellidoMaterno='" + apellidoMaterno + "', rut ='" + rut + "', " +
             " correo='" + correo + "', alias='" + alias + "', fechaNacimiento='" + h.formatearFecha(fechaNacimiento, false) + "', telefonoUno='" +
             telefonoUno + "', telefonoDos= '" + telefonoDos + "',respaldado=1,activoDesde='" +
             h.formatearFecha(activoDesde, false) + "',centroCosto='" + centroCosto + "',puesto='" + puesto + "',updated_at='" + fechaSistema + "',nombreEquipoEdicion='" + Environment.MachineName + "'" +
             ",habilitada=" + habilitada + ",empresa_idempresa=" + empresa_idempresa + " where rut='" + rut + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        // Funciones para sincronizador.
        public DataTable traerDatosPersonasDesdeNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,  nombre,segundoNombre,apellidoPaterno, apellidoMaterno, " +
            "centroCosto,puesto,correo,alias, fechaNacimiento, telefonoUno,telefonoDos, " +
            "activoDesde, habilitada,respaldado,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa from persona where updated_at>='" + updated_at + "' order by updated_at asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public DataTable traerDatosPersonasDesdeNubePorRut(string rutSinCifrar)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,  nombre,segundoNombre,apellidoPaterno, apellidoMaterno, " +
            "centroCosto,puesto,correo,alias, fechaNacimiento, telefonoUno,telefonoDos, " +
            "activoDesde, habilitada,respaldado,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa from persona where rut='" + rutSinCifrar + "' limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerDatosPersonasDesdeLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,  nombre,segundoNombre,apellidoPaterno, apellidoMaterno, " +
            "centroCosto,puesto,correo,alias, fechaNacimiento, telefonoUno,telefonoDos, " +
            "activoDesde, habilitada,respaldado,created_at,updated_at,nombreEquipoEdicion,empresa_idempresa from persona where nombreEquipoEdicion <> '" + Environment.MachineName + "'";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTodasPersonas()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,nombre,segundoNombre,apellidoPaterno,apellidoMaterno,puesto " +
                         "from persona where habilitada=1 order by apellidoPaterno asc";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTodasPersonasAdmin()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,rut,nombre,segundoNombre,apellidoPaterno,apellidoMaterno,puesto " +
                         "from persona order by apellidoPaterno asc";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarEstadoRespaldadoPersonaLocal(string rut, DateTime updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            Herramientas h = new Herramientas();
            string updatedFormateado = h.formatearFecha(updated_at, true);
            string sql = "update persona set respaldado=1, updated_at='" + updatedFormateado + "' where rut='" + e.Encriptar(rut) + "'";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerUltimoUpdatedAtPersona()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from persona where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDatosBasicosPersonaPorRut(string rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idpersona,nombre,segundoNombre,apellidoPaterno,apellidoMaterno from persona where rut='" + rut + "' and habilitada=1 limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }



    }
}
