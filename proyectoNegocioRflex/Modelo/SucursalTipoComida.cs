using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class SucursalTipoComida
    {

        public DataTable traerSucursalTipoComidaPorID(int idsucursal_tipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal_tipoComida, sucursal_idsucursal, tipoComida_idtipoComida, horaInicioEmision, horaTerminoEmision, horaInicioCobro, horaTerminoCobro, habilitada, created_at, updated_at, diasExpiracionTicket from sucursal_tipoComida where idsucursal_tipoComida=" + idsucursal_tipoComida + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerSucursalTipoComidaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal_tipoComida, sucursal_idsucursal, tipoComida_idtipoComida, horaInicioEmision, horaTerminoEmision, horaInicioCobro, horaTerminoCobro, habilitada, created_at, updated_at, diasExpiracionTicket from sucursal_tipoComida where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerUltimoUpdatedAtSucursalTipoComida()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from sucursal_tipoComida;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool guardarSucursalTipoComidaLocal(int idsucursal_tipoComida, int sucursal_idsucursal, int tipoComida_idtipoComida,
            string horaInicioEmision, string horaTerminoEmision,
            string horaInicioCobro, string horaTerminoCobro,
            int habilitada, string created_at, string updated_at, int diasExpiracionTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into sucursal_tipoComida(idsucursal_tipoComida, sucursal_idsucursal, tipoComida_idtipoComida, horaInicioEmision, horaTerminoEmision, horaInicioCobro, horaTerminoCobro, habilitada, created_at, updated_at, diasExpiracionTicket) values(" +
              idsucursal_tipoComida + "," + sucursal_idsucursal + "," + tipoComida_idtipoComida + ",'" + horaInicioEmision + "','" + horaTerminoEmision + "','" + horaInicioCobro + "','" + horaTerminoCobro + "'," + habilitada + ",'" + created_at + "','" + updated_at + "', " + diasExpiracionTicket + ");";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarSucursalTipoComidaLocal(int idsucursal_tipoComida, int sucursal_idsucursal, int tipoComida_idtipoComida, string horaInicioEmision, string horaTerminoEmision, string horaInicioCobro, string horaTerminoCobro, int habilitada, string updated_at, int diasExpiracionTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update sucursal_tipoComida set sucursal_idsucursal=" + sucursal_idsucursal + ", tipoComida_idtipoComida=" + tipoComida_idtipoComida + ", horaInicioEmision ='" + horaInicioEmision + "', horaTerminoEmision='" + horaTerminoEmision + "', horaInicioCobro='" + horaInicioCobro + "', horaTerminoCobro='" + horaTerminoCobro + "', habilitada=" + habilitada + ", updated_at='" + updated_at + "', diasExpiracionTicket=" + diasExpiracionTicket + " where idsucursal_tipoComida=" + idsucursal_tipoComida + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerSucursalTipoComidaLocalPorEstadoYSucursal(int habilitado, int idsucursal)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            // string sql = "select idsucursal_tipoComida, tipoComida_idtipoComida, horaInicioEmision,horaTerminoEmision, habilitada from sucursal_tipoComida where habilitada=" + habilitado + " and sucursal_idsucursal=" + idsucursal + ";";
            string sql = "select sucursal_tipoComida.idsucursal_tipoComida, sucursal_tipoComida.tipoComida_idtipoComida, " +
            " sucursal_tipoComida.horaInicioEmision, sucursal_tipoComida.horaTerminoEmision, " +
            " sucursal_tipoComida.habilitada, tipoComida.nombre, sucursal_tipoComida.diasExpiracionTicket from sucursal_tipoComida " +
            " inner join tipoComida on tipoComida.idtipoComida = sucursal_tipoComida.tipoComida_idtipoComida " +
            " where sucursal_tipoComida.habilitada = " + habilitado + " and sucursal_tipoComida.sucursal_idsucursal =" + idsucursal + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDiasVencimientoTicketTipoComidaPorIdTipoComidaYSucursal(int sucursal_idsucursal, int tipoComida_idtipoComida)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select diasExpiracionTicket from sucursal_tipoComida where sucursal_idsucursal=" + sucursal_idsucursal + " and tipoComida_idtipoComida=" + tipoComida_idtipoComida + " and habilitada=1;";
            return ejecutor.traerDatosDataTable(sql);
        }

    }
}
