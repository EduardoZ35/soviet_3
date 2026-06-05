using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Sucursal
    {
        public DataTable traerDatosSucursalActual()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal, empresa_idempresa, nombre, rut_encargado, " +
                "nombre_encargado, calle,  numero_calle,  comuna, ciudad, region, pais, " +
                "sucursalActual from sucursal where sucursalActual=1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarSucursal(int idsucursal, int empresa_idempresa, string nombre, string rut_encargado, string nombre_encargado, string calle, string numero_calle,
            string comuna, string ciudad, string region, string pais, int sucursalActual, int respaldado, string updated_at)
        {
            EjecutoresSql ejecuta = new EjecutoresSql();
            string sql = "insert into sucursal(idsucursal,empresa_idempresa,nombre,rut_encargado,nombre_encargado,calle, numero_calle,comuna, ciudad, region,  pais, sucursalActual, respaldado, updated_at) values " +
                "(" + idsucursal + "," + empresa_idempresa + ",'" + nombre + "','" + rut_encargado + "','" + nombre_encargado + "','" + calle + "','" + numero_calle + "','" + comuna + "','" + ciudad + "','" + region + "','" + pais + "'," + sucursalActual + "," + respaldado + ",'" + updated_at + "')";
            return ejecuta.ejecutarConsulta(sql);
        }

        public bool editarDatosSucursal(string nombre, string rut_encargado,
            string nombre_encargado, string calle, string numero_calle, string comuna,
            string ciudad, string region, string pais, int sucursalActual, int respaldado, int idsucursal, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update sucursal set nombre='" + nombre + "', rut_encargado='" + rut_encargado + "', nombre_encargado='" +
            nombre_encargado + "',calle='" + calle + "', numero_calle='" + numero_calle + "', " +
            "comuna='" + comuna + "', region='" + region + "', pais='" +
            pais + "', sucursalActual=" + sucursalActual + ", respaldado =" + respaldado + ", updated_at='" + updated_at + "' where idsucursal =" + idsucursal + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable traerSucursales()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal, empresa_idempresa, nombre, rut_encargado, " +
                "nombre_encargado, calle,  numero_calle,  comuna, ciudad, region, pais, " +
                "sucursalActual from sucursal";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerSucursalesDesdeNube()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal, empresa_idempresa, nombre, rut_encargado, " +
                "nombre_encargado, calle,  numero_calle,  comuna, ciudad, region, pais, " +
                "sucursalActual,respaldado,updated_at from sucursal";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerSucursalLocalPorID(int idsucursal)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idsucursal,updated_at from sucursal where idsucursal=" + idsucursal + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerSucursalPorIDReloj(int idreloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select sucursal.idsucursal, sucursal.empresa_idempresa, sucursal.nombre, sucursal.rut_encargado, " +
               "sucursal.nombre_encargado, sucursal.calle, sucursal.numero_calle, sucursal.comuna, sucursal.ciudad, sucursal.region, sucursal.pais, " +
               "sucursal.sucursalActual from sucursal inner join reloj on  reloj.sucursal_idsucursal = sucursal.idsucursal " +
               "where reloj.idreloj =" + idreloj;
            return ejecutor.traerDatosDataTable(sql);
        }


    }
}
