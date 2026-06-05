using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Servidor
    {
        public DataTable traerDatosServidor()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idservidor, direccion,usuario,pass,puerto from servidor";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerFechaTimezoneDesdeServidor()
        {
            EjecutoresSql ejecuta = new EjecutoresSql();
            string sql = "SELECT now(), TIME_FORMAT(TIMEDIFF(NOW(), UTC_TIMESTAMP),'%h:%i') AS zonaHoraria;";
            return ejecuta.traerDatosDataTableServidor(sql);
        }
    }
}
