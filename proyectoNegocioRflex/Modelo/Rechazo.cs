using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Rechazo
    {

        public bool agregarRechazo(int empresa_idempresa, int sucursal_idsucursal, string persona_rut, int rechazo_idtipoRechazo, int reloj_idreloj
                , string centrocosto_funcionario, string puesto_funcionario, int respaldado, string fechaRechazo = null, string zonaHoraria = null, int esHoraServidor = 0)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            Herramientas h = new Herramientas();
            string hash = e.Encriptar(h.quitarCaracteresEspecialesFechaParaHash(fechaRechazo.ToString()) + empresa_idempresa.ToString() + sucursal_idsucursal.ToString() + persona_rut + rechazo_idtipoRechazo.ToString() + reloj_idreloj.ToString());
            string rutEncriptado = e.Encriptar(persona_rut.ToUpper());
            string sql = "insert into rechazo(empresa_idempresa,sucursal_idsucursal,persona_rut,rechazo_idtipoRechazo,reloj_idreloj," +
                "centrocosto_funcionario,puesto_funcionario,fecha_marca,hash,respaldado,nombreEquipoEdicion,notificado,zonaHoraria,esHoraServidor) values (" +
                empresa_idempresa + "," +
                sucursal_idsucursal + ",'" +
                rutEncriptado + "'," +
                rechazo_idtipoRechazo + "," +
                reloj_idreloj + ",'" +
                centrocosto_funcionario + "','" +
                puesto_funcionario + "','" + fechaRechazo + "','" + hash + "'," + respaldado + ",'" + Environment.MachineName + "',0,'" + zonaHoraria + "'," + esHoraServidor + ")";
            return ejecutor.ejecutarConsulta(sql);
            // concatenar y cifrar rut, fecha,sentido,
        }


        public bool agregarRechazoNube(int empresa_idempresa, int sucursal_idsucursal, string persona_rut, int rechazo_idtipoRechazo, int reloj_idreloj
        , string centrocosto_funcionario, string puesto_funcionario, int respaldado, string fecha_marca, string hash, string nombreEquipoEdicion, int notificado, string zonaHoraria, int esHoraServidor)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "insert into rechazo(empresa_idempresa,sucursal_idsucursal,persona_rut,rechazo_idtipoRechazo,reloj_idreloj," +
                "centrocosto_funcionario,puesto_funcionario,fecha_marca,hash,respaldado,nombreEquipoEdicion,notificado,zonaHoraria,esHoraServidor) values (" +
                empresa_idempresa + "," +
                sucursal_idsucursal + ",'" +
                persona_rut + "'," +
                rechazo_idtipoRechazo + "," +
                reloj_idreloj + ",'" +
                centrocosto_funcionario + "','" +
                puesto_funcionario + "','" + fecha_marca + "','" + hash + "'," + respaldado + ",'" + nombreEquipoEdicion + "'," + notificado + ",'" + zonaHoraria + "'," + esHoraServidor + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
            // concatenar y cifrar rut, fecha,sentido,
        }

        public bool actualizarEstadoNotificacion(int idrechazo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update rechazo set notificado=1 where idrechazo=" + idrechazo + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoNotificacionNube(string hash)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update rechazo set notificado=1 where hash='" + hash + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        /**
         * Función que retorna los rechazos pendientes de notificación. 
         * Por el momento sólo retornamos los tipos 1 y 2 (huella no reconocida o PIN)
         */
        public DataTable traerRechazosSinEnviar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select rechazo.idrechazo,persona.nombre,persona.segundoNombre,persona.apellidoPaterno,persona.apellidoMaterno, " +
            "persona.rut,persona.correo,reloj.nombre,tipoRechazo.mensajeRechazo,rechazo.fecha_marca,rechazo.hash, " +
            "rechazo.notificado, sucursal.nombre , reloj.ubicacion , sucursal.calle,sucursal.numero_calle,sucursal.comuna,sucursal.ciudad,sucursal.region,sucursal.pais,empresa.razon_social,empresa.rut_empresa,persona.empresa_idempresa " +
            "from rechazo,persona,reloj,tipoRechazo,sucursal,empresa where " +
            "rechazo.rechazo_idtipoRechazo in (1, 2) and " +
            "rechazo.persona_rut = persona.rut and " +
            "rechazo.reloj_idreloj = reloj.idreloj and " +
            "rechazo.rechazo_idtipoRechazo = tipoRechazo.idtipoRechazo and " +
            "reloj.sucursal_idsucursal = sucursal.idsucursal and " +
            "rechazo.empresa_idempresa = empresa.idempresa and " +
            "rechazo.notificado = 0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerRechazosSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idrechazo , empresa_idempresa , sucursal_idsucursal , " +
            "persona_rut , rechazo_idtipoRechazo , reloj_idreloj, centrocosto_funcionario," +
            "puesto_funcionario , fecha_marca , hash , respaldado , nombreEquipoEdicion , notificado,zonaHoraria,esHoraServidor from rechazo where respaldado = 0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarEstadoRespaldoRechazo(int idrechazo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update rechazo set respaldado=1 where idrechazo=" + idrechazo + "";
            return ejecutor.ejecutarConsulta(sql);
        }

    }
}
