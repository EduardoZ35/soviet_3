using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Empresa
    {
        public DataTable traerDatosEmpresa()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idempresa,rut_empresa, razon_social, giro," +
            "rut_repesentante_legal, calle,numero_calle, comuna,region,pais,updated_at,codigo from empresa";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool editarDatosEmpresa(string rut_empresa, string razon_social, string giro,
           string rut_repesentante_legal, string calle, string numero_calle, string comuna, string region, string pais, int respaldado, int idempresa,
           string updated_at, string codigo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update empresa set rut_empresa='" + rut_empresa + "', razon_social='" + razon_social + "', giro='" +
            giro + "',rut_repesentante_legal='" + rut_repesentante_legal + "', calle='" + calle + "', numero_calle='" +
            numero_calle + "', comuna='" + comuna + "', region='" + region + "',pais='" + pais + "', updated_at='" + updated_at + "',codigo='" + codigo + "'  where idempresa =" + idempresa + "";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool agregarDatosEmpresa(string rut_empresa, string razon_social, string giro,
        string rut_repesentante_legal, string calle, string numero_calle, string comuna,
        string region, string pais, string updated_at, string codigo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into empresa  (rut_empresa,razon_social,giro,rut_repesentante_legal,calle,numero_calle,comuna,region,pais,updated_at,codigo) values" +
            "('" + rut_empresa + "','" + razon_social + "', '" + giro + "','" + rut_repesentante_legal + "','" + calle + "','" + numero_calle + "'," +
            "'" + comuna + "','" + region + "','" + pais + "','" + updated_at + "','" + codigo + "')";
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerDatosEmpresaDesdeServidor(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idempresa,rut_empresa, razon_social, giro," +
            "rut_repesentante_legal, calle,numero_calle, comuna,region, pais,updated_at,codigo from empresa where updated_at >='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerDatosEmpresaPorID(int idempresa)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idempresa,rut_empresa, razon_social, giro," +
            "rut_repesentante_legal, calle,numero_calle, comuna,region,pais,updated_at,codigo from empresa where idempresa=" + idempresa + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerUltimoUpdatedAtEmpresa()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from empresa;";
            return ejecutor.traerDatosDataTable(sql);
        }

    }
}
