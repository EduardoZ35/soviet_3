using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoTurno
    {
        public DataTable traerTipoTurnoNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoTurno,descripcion,turnoDia,horaInicio,horaTermino,created_at,updated_at from tipoTurno where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerTipoTurnoLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoTurno,turnoDia,horaInicio,horaTermino from tipoTurno";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoTurnoPorID(int idtipoMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoTurno,descripcion,turnoDia,horaInicio,horaTermino,created_at,updated_at from tipoTurno where idtipoTurno=" + idtipoMarca + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarTipoTurnoLocal(int idtipoTurno, string descripcion, int turnoDia,string horaInicio,string horaTermino, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoTurno(idtipoTurno,descripcion,turnoDia,horaInicio,horaTermino,updated_at) values (" + idtipoTurno + ",'" + 
                descripcion + "'," + turnoDia + ",'" + horaInicio + "','" +  horaTermino + "','" +  updated_at + "')";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarTipoTurnoLocal(int idtipoTurno, string descripcion, int turnoDia, string horaInicio, string horaTermino, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoTurno set descripcion='" + descripcion + "',turnoDia=" + turnoDia + ", horaInicio='" +
                horaInicio + "',horaTermino='" + horaTermino + "', updated_at ='" + updated_at + "' where idtipoTurno=" + idtipoTurno + "";
            return ejecutor.ejecutarConsulta(sql);
        }
    }
}
