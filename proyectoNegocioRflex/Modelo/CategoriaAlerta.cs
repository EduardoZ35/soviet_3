using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class CategoriaAlerta
    {
        public DataTable traerCategoriaAlertaNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idcategoriaAlerta, nombre,activo,created_at,updated_at from categoriaAlerta where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerUpdatedAtCategoriaAlertaPorID(int idcategoriaAlerta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select updated_at from categoriaAlerta where idcategoriaAlerta =" + idcategoriaAlerta + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarCategoriaAlertaLocal(int idcategoriaAlerta, string nombre, int activo, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into categoriaAlerta (idcategoriaAlerta, nombre,activo,created_at,updated_at) values(" + idcategoriaAlerta + ",'" + nombre + "'," + activo + ",'" + created_at + "','" + updated_at + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool editarCategoriaAlertaLocal(string nombre, int activo, string updated_at, int idcategoriaAlerta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update categoriaAlerta set nombre='" + nombre + "',activo=" + activo + " ,updated_at='" + updated_at + "' where idcategoriaAlerta=" + idcategoriaAlerta + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerUltimoUpdatedAtCategoriaAlerta()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from categoriaAlerta;";
            return ejecutor.traerDatosDataTable(sql);
        }

    }
}
