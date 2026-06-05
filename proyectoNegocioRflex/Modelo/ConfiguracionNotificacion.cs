using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class ConfiguracionNotificacion
    {
        public DataTable traerConfigNotificacion()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfiguracionNotificacion,updated_at,correo,pass,servidor,puerto,usuario from configuracionNotificacion";
            return ejecutor.traerDatosDataTable(sql);
        }

       public DataTable traerConfigNotificacionLocalPorID(int idconfiguracionNotificacion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfiguracionNotificacion,updated_at from configuracionNotificacion where idconfiguracionNotificacion=" + idconfiguracionNotificacion + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerConfigNotificacionServidor(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfiguracionNotificacion, correo,pass,servidor,puerto,updated_at,usuario from configuracionNotificacion where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarConfigNotificacionLocal(string correo, string pass, string servidor, string puerto, string updated_at, int idconfiguracionNotificacion,string usuario)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "insert into configuracionNotificacion(idconfiguracionNotificacion,correo,pass,servidor,puerto,updated_at,usuario) values(" + idconfiguracionNotificacion + ",'" + e.Encriptar(correo) + "','" + e.Encriptar(pass) + "','" + e.Encriptar(servidor) + "','" + e.Encriptar(puerto) + "','" + updated_at + "','" + e.Encriptar(usuario) + "')";
            return ejecutor.ejecutarConsulta(sql);           
        }

        public bool actualizarConfigNotificacionLocal(string correo, string pass, string servidor, string puerto, string updated_at, int idconfiguracionNotificacion, string usuario)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update configuracionNotificacion set correo='" + e.Encriptar(correo) + "',pass='" + e.Encriptar(pass) + "',servidor='" + e.Encriptar(servidor) + "',puerto='" + e.Encriptar(puerto) + "' ,updated_at='" + updated_at + "',usuario='" + usuario + "' where idconfiguracionNotificacion='" + idconfiguracionNotificacion + "'";
            return ejecutor.ejecutarConsulta(sql);
        }
    }
}
