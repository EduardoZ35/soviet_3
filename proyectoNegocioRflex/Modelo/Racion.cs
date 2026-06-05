using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Racion
    {
        public DataTable traerRacion(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idracion, persona_idpersona,tipoRacion_idtipoRacion " +
                "from racion where persona_rut ='" + persona_rut + "'";
            return ejecutor.traerDatosDataTable(sql);
        }
    }
}
