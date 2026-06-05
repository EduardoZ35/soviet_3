using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoComida
    {



        public int IDdtipoComida { get; set; }
        public string NombreTipoComida { get; set; }
        //Esta es una variable inventada para poder manejar el tema de las consultas de rango horario
        //Los tipos de comida que tienen el registro de hora de inicio mayor que la de termino
        //o sea son tipos de comida que inician un dìa y terminan al otro.
        public bool HorarioInvertido { get; set; } 


        public DataTable traerTipoComida()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida, nombre,valor,horaInicio,horaTermino,habilitada,diasExpiracionTicket from tipoComida";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoComidaPorEstado(int habilitado)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida, nombre,horaInicioEmision,horaTerminoEmision,habilitada,diasExpiracionTicket from tipoComida where habilitada=" + habilitado + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        //Sincronizador.....

        public DataTable traerTipoComidaPorID(int idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida, updated_at, diasExpiracionTicket from tipoComida where idtipoComida=" + idtipoComida + " limit 1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoComidaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoComida, nombre,horaInicioEmision,horaTerminoEmision,horaInicioCobro,horaTerminoCobro,habilitada,updated_by,created_by,created_at,updated_at,diasExpiracionTicket from tipoComida where updated_at>= '" + updated_at + "';";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public bool agregarNuevoTipoDeComida(int idtipoComida, string nombre, string horaInicioEmision, string horaTerminoEmision, string horaInicioCobro, string horaTerminoCobro, int habilitada, string updated_by, string created_by, string created_at, string updated_at, int diasExpiracionTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoComida(idtipoComida, nombre,horaInicioEmision,horaTerminoEmision,horaInicioCobro,horaTerminoCobro,habilitada,updated_by,created_by,created_at,updated_at,diasExpiracionTicket) values " +
                "(" + idtipoComida + ",'" + nombre + "','" + horaInicioEmision + "','" + horaTerminoEmision + "','" + horaInicioCobro + "','" + horaTerminoCobro + "'," + habilitada + ",'" + updated_by + "','" + created_by + "','" + created_at + "','" + updated_at + "'," + diasExpiracionTicket + ");";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarTipoDeComida(int idtipoComida, string nombre, string horaInicioEmision, string horaTerminoEmision, string horaInicioCobro, string horaTerminoCobro, int habilitada, string updated_by, string updated_at, int diasExpiracionTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoComida set nombre='" + nombre + "', horaInicioEmision='" + horaInicioEmision + "' ,horaTerminoEmision='" + horaTerminoEmision + "', horaInicioCobro='" + horaInicioCobro + "', horaTerminoCobro='" + horaTerminoCobro + "', updated_by='" + updated_by + "', updated_at ='" + updated_at + "', habilitada=" + habilitada + ", diasExpiracionTicket=" + diasExpiracionTicket + " where idtipoComida=" + idtipoComida + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerNombreTipoComidaPorID(int idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select nombre from tipoComida where idtipoComida=" + idtipoComida + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerUltimoUpdatedAtTipoComida()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from tipoComida;";
            return ejecutor.traerDatosDataTable(sql);
        }
    }
}
