using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class RacionFija
    {

        public DataTable traerRacionFija()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija,numeroDia,persona_rut,tipoComida_idtipoComida,habilitado,nombreEquipoEdicion,respaldado,created_by,updated_by,created_at,updated_at from racionFija where respaldado=0;";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerRacionFijaHabilitadaPorRutDiaYTipoComida(string persona_rut, int numeroDia, int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string rut_encryptado = e.Encriptar(persona_rut.ToUpper());
            string sql = "select idracionFija from racionFija where persona_rut='" + rut_encryptado + "' and numeroDia=" + numeroDia + " and tipoComida_idtipoComida=" + tipoComida_idtipoComida + " and habilitado=1 limit 1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerRacionFijaNubeSinRespaldoLocal(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija,numeroDia,persona_rut,tipoComida_idtipoComida,habilitado,nombreEquipoEdicion,respaldado,created_by,updated_by,created_at,updated_at from racionFija where updated_at>='" + updated_at + "' order by updated_at asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerRacionFijaPorRutTrabajador(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija,numeroDia,tipoComida_idtipoComida,habilitado from racionFija where persona_rut='" + persona_rut + "';";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarRacionFijaLocal(int numeroDia, string persona_rut, int tipoComida_idtipoComida, int habilitado, string nombreEquipoEdicion, int respaldado, string created_by, string updated_by, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into racionFija( numeroDia,persona_rut,tipoComida_idtipoComida,habilitado,nombreEquipoEdicion,respaldado,created_by,updated_by,created_at,updated_at ) values " +
                "(" + numeroDia + ",'" + persona_rut + "'," + tipoComida_idtipoComida + "," + habilitado + ",'" + nombreEquipoEdicion + "'," + respaldado + ",'" + created_by + "','" + updated_by + "','" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarRacionFijaLocal(int habilitado, string nombreEquipoEdicion, int respaldado, string updated_by, string updated_at, int idracionFija)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update racionFija set habilitado=" + habilitado + ",nombreEquipoEdicion='" + nombreEquipoEdicion + "', respaldado=" + respaldado + ",updated_by='" + updated_by + "' ,updated_at='" + updated_at + "' where idracionFija =" + idracionFija + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarRacionFijaNube(int numeroDia, string persona_rut, int tipoComida_idtipoComida, int habilitado, string nombreEquipoEdicion, int respaldado, string created_by, string updated_by, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into racionFija( numeroDia,persona_rut,tipoComida_idtipoComida,habilitado,nombreEquipoEdicion,respaldado,created_by,updated_by,created_at,updated_at ) values " +
                "(" + numeroDia + ",'" + persona_rut + "'," + tipoComida_idtipoComida + "," + habilitado + ",'" + nombreEquipoEdicion + "'," + respaldado + ",'" + created_by + "','" + updated_by + "','" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool editarRacionFijaNube(int habilitado, string nombreEquipoEdicion, string updated_by, string updated_at, int idracionFija)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update racionFija set habilitado=" + habilitado + ",nombreEquipoEdicion='" + nombreEquipoEdicion + "' ,updated_by='" + updated_by + "' ,updated_at='" + updated_at + "' where idracionFija =" + idracionFija + "";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public DataTable traerRacionesFijasLocalesSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija,numeroDia,persona_rut,tipoComida_idtipoComida,habilitado,nombreEquipoEdicion,respaldado,created_by,updated_by,created_at,updated_at from racionFija where respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable preguntarRegistroRacionFijaExistenteNube(string persona_rut, int numeroDia, int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija from racionFija where persona_rut ='" + persona_rut + "' and numeroDia =" + numeroDia + " and tipoComida_idtipoComida=" + tipoComida_idtipoComida + " limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool editarEstadoRespaldadoRacionFijaLocal(int idracionFija, DateTime updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string updatedFormateado = h.formatearFecha(updated_at, true);
            string sql = "update racionFija set respaldado=1,updated_at='" + updatedFormateado + "' where idracionFija =" + idracionFija + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable preguntarRegistroRacionFijaExistentLocal(string persona_rut, int numeroDia, int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracionFija,updated_at from racionFija where persona_rut ='" + persona_rut + "' and numeroDia =" + numeroDia + " and tipoComida_idtipoComida=" + tipoComida_idtipoComida + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerUltimoUpdatedAtRacionFija()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from racionFija where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }

    }
}
