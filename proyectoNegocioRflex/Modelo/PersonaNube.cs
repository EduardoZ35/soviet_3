using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class PersonaNube
    {
        public DataTable traerDatosDesdeTablaPersonaPrincipal(string rut, string codigoEmpresa)
        {
            EjecutoresSql ejecuta = new EjecutoresSql();
            string sql= "select nombre,segundoNombre,apellidoPaterno,apellidoMaterno,rut,correo,alias,fechaNacimiento,telefono1,telefono2 from persona where rut='" + rut + "' limit 1";
            return ejecuta.traerDatosDataTableBDPrincipal(sql, codigoEmpresa);
        }
    }
}
