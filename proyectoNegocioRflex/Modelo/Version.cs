using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Version
    {

        public DataTable traerRegistroVersionSinRespaldar(int reloj_idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj_idreloj,relojControl, sincronizador,sincronizadorCasino,created_at,updated_at from version where respaldado=0 and reloj_idreloj=" + reloj_idreloj + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable preguntarDatoVersionExistenteLocal(int reloj_idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj_idreloj,relojControl, sincronizador,sincronizadorCasino,created_at,updated_at from version where reloj_idreloj=" + reloj_idreloj + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable preguntarDatoVersionExistenteNube(int reloj_idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select reloj_idreloj,relojControl, sincronizador,sincronizadorCasino,created_at,updated_at from version where reloj_idreloj=" + reloj_idreloj + ";";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarDatosVersionLocal(int reloj_idreloj, string version, string proyecto)
        {
            //En teoría esta función sólo se va a usar una vez.
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql;
            switch (proyecto)
            {
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR:
                    sql = "insert into version (reloj_idreloj,relojControl,sincronizador,sincronizadorCasino, created_at, updated_at) values(" + reloj_idreloj + ",'','" + version + "','', now(),now())";
                    break;
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO:
                    sql = "insert into version (reloj_idreloj,relojControl,sincronizador,sincronizadorCasino, created_at, updated_at) values(" + reloj_idreloj + ",'','','" + version + "' , now(),now())";
                    break;
                default:
                    sql = "insert into version (reloj_idreloj,relojControl,sincronizador,sincronizadorCasino, created_at, updated_at) values(" + reloj_idreloj + ",'" + version + "','','',now() ,now())";
                    break;
            }
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarDatosVersionNube(int reloj_idreloj, string relojControl, string sincronizador, string sincronizadorCasino, string created_at, string updated_at)
        {
            //En teoría esta función sólo se va a usar una vez.
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into version (reloj_idreloj,relojControl,sincronizador,sincronizadorCasino, created_at, updated_at, respaldado) values(" + reloj_idreloj + ",'" + relojControl + "','" + sincronizador + "','" + sincronizadorCasino + "','" + created_at + "','" + updated_at + "',1)";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarRegistroVersionLocal(string proyecto, string version, string updated_at, int reloj_idreloj)
        {
            string sql;
            EjecutoresSql ejecutor = new EjecutoresSql();
            switch (proyecto)
            {
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR:
                    sql = "update version set sincronizador='" + version + "',respaldado=0, updated_at='" + updated_at + "' where reloj_idreloj= " + reloj_idreloj + ";";
                    break;
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO:
                    sql = "update version set sincronizadorCasino='" + version + "',respaldado=0, updated_at='" + updated_at + "' where reloj_idreloj= " + reloj_idreloj + ";";
                    break;
                default:
                    sql = "update version set relojControl='" + version + "', respaldado=0, updated_at='" + updated_at + "' where reloj_idreloj= " + reloj_idreloj + ";";
                    break;
            }
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarRegistroVersionNube(string versionReloj, string versionSincronizador, string versionSincronizadorCasino, string updated_at, int reloj_idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update version set relojControl='" + versionReloj + "', sincronizador='" + versionSincronizador + "',sincronizadorCasino='" + versionSincronizadorCasino + "',respaldado=1 , updated_at='" + updated_at + "' where reloj_idreloj= " + reloj_idreloj + ";";
            return ejecutor.ejecutarConsultaServidor(sql); ;
        }


        public bool actualizarRegistroRespaldadoLocal(int reloj_idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update version set respaldado= 1 where reloj_idreloj= " + reloj_idreloj + ";";
            return ejecutor.ejecutarConsulta(sql); ;
        }

    }
}
