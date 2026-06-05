using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class CorreoAlerta
    {

        public DataTable traerCorreosAlertasHabilitadosLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idcorreoAlerta, nombreEncargadoCorreo, correo,habilitado,updated_at from correoAlerta where habilitado=1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerCorreosAlertasNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idcorreoAlerta, nombreEncargadoCorreo, correo,habilitado,updated_at from correoAlerta where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerCorreosAlertasPorIDLocal(int idcorreoAlerta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idcorreoAlerta, nombreEncargadoCorreo, correo,habilitado,updated_at from correoAlerta where idcorreoAlerta=" + idcorreoAlerta + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarCorreoAlertaLocal(int idcorreoAlerta, string nombreEncargadoCorreo, string correo, int habilitado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "insert into correoAlerta( idcorreoAlerta, nombreEncargadoCorreo, correo,habilitado,updated_at)values (" + idcorreoAlerta + ",'" + e.Encriptar(nombreEncargadoCorreo) + "','" + e.Encriptar(correo) + "'," + habilitado + ",'" + updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarCorreoAlertaLocal(int idcorreoAlerta, string nombreEncargadoCorreo, string correo, int habilitado, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            string sql = "update correoAlerta set nombreEncargadoCorreo ='" + e.Encriptar(nombreEncargadoCorreo) + "', correo='" + e.Encriptar(correo) + "', habilitado=" + habilitado + " ,updated_at='" + updated_at + "' where idcorreoAlerta=" + idcorreoAlerta + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        //SELECT `correoAlerta`.`idcorreoAlerta`,
        //`correoAlerta`.`nombreEncargadoCorreo`,
        //`correoAlerta`.`correo`,
        //`correoAlerta`.`habilitado`,
        //`correoAlerta`.`updated_at`
        //FROM `relojControl`.`correoAlerta`;



    }
}
