using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class ConfigSlack
    {
        public DataTable traerConfigsSlackPorID(int idconfigSlack)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfigSlack,updated_at from configSlack where idconfigSlack=" + idconfigSlack + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerConfigSlackNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfigSlack,webHook,canal,userName,esCanalReloj,updated_at,habilitado from configSlack where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        //ES RELOJ SE OCUPA PARA DIFERENCIAR SI LA CONFIGURACIÓN ES PARA UN CANAL DE NOTIFICACIÓN DEL RELOJ O 
        //DEL SINCRONIZADOR.
        //esCanalReloj = 1 SI ES RELOJ
        //esCanalReloj = 0 SI ES SINCRONIZADOR.
        public DataTable traerConfigSlackPorEsCanalRelojYHabilitado(int esCanalReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idconfigSlack,webHook,canal,userName,esCanalReloj,updated_at,habilitado from configSlack where esCanalReloj=" + esCanalReloj + " and habilitado=1 limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarConfigSlack(int idconfigSlack, string webHook, string canal, string userName, int esCanalReloj, string updated_at, int habilitado)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "insert into configSlack (idconfigSlack,webHook,canal,userName,esCanalReloj,updated_at,habilitado) " +
                "values(" + idconfigSlack + ",'" + e.Encriptar(webHook) + "','" + e.Encriptar(canal) + "','" + e.Encriptar(userName) + "'," + esCanalReloj + ",'" + updated_at + "'," + habilitado + ")";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarConfigSlack(int idconfigSlack, string webHook, string canal, string userName, int esCanalReloj, string updated_at, int habilitado)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update configSlack set webHook='" + e.Encriptar(webHook) + "' ,canal='" + e.Encriptar(canal) + "',userName='" + e.Encriptar(userName) + "',esCanalReloj=" + esCanalReloj + ",updated_at='" + updated_at + "',habilitado=" + habilitado + " where idconfigSlack=" + idconfigSlack + "";
            return ejecutor.ejecutarConsulta(sql);
        }





    }
}
