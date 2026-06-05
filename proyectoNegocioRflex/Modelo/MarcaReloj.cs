using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class MarcaReloj
    {
        public bool agregarMarcaIntegracion(string COD_EMPRESA_SYS, string NUM_TARJETA, string FECHA_MARCA,
           int RUT_FUNCIONARIO, string HORA_MARCA, string SENTIDO_MARCA, string NUM_RELOJ, string TIPO_MARCA, string ID_RESPONSABLE,
          string FECHA_TRANSACCION, string FECHA_INGRESO, string COD_SUCURSAL_SYS, string IP, string NOMBRE_PC, string OBSERVACION,
          string created_at, string updated_at)
        {
            //Este campo es el concatenado  de la fecha de la marca con la hora.
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string campoFechaHora = h.formatearFechaSinHora(DateTime.Parse(FECHA_MARCA)) + " " + HORA_MARCA;
            string sql = "insert into marcaReloj(COD_EMPRESA_SYS,NUM_TARJETA, FECHA_MARCA, RUT_FUNCIONARIO, HORA_MARCA, SENTIDO_MARCA, NUM_RELOJ, TIPO_MARCA, ID_RESPONSABLE, " +
           "FECHA_TRANSACCION, FECHA_INGRESO, COD_SUCURSAL_SYS, IP, NOMBRE_PC,  OBSERVACION,created_at,  updated_at,FECHA_HORA) values('" + COD_EMPRESA_SYS + "','" + NUM_TARJETA + "', " +
           "'" + FECHA_MARCA + "'," + RUT_FUNCIONARIO + ",'" + HORA_MARCA + "','" + SENTIDO_MARCA + "','" + NUM_RELOJ + "','" + TIPO_MARCA + "','" + ID_RESPONSABLE + "','" + FECHA_TRANSACCION + "','" + FECHA_INGRESO + "','" + COD_SUCURSAL_SYS + "','" + IP + "'," +
           "'" + NOMBRE_PC + "','" + OBSERVACION + "','" + created_at + "','" + updated_at + "','" + campoFechaHora + "')";
            return ejecutor.ejecutarConsultaBDIntegracion(sql, COD_EMPRESA_SYS);
        }


        public DataTable traerDatosMarcaParaIntegracion(int  idmarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select empresa.codigo, marca.persona_rut, marca.fecha_marca, marca.tipoMarca_idtipoMarca,"+
            "marca.reloj_idreloj, marca.marcaConPin, marca.sucursal_idsucursal, reloj.ip_reloj, reloj.nombre "+
            "from marca, empresa, reloj "+
            "where marca.idmarca=" + idmarca + " and " +
            "marca.reloj_idreloj= reloj.idreloj and " +
            "marca.empresa_idempresa = empresa.idempresa";
            return ejecutor.traerDatosDataTable(sql);
        }


        //     SELECT `marcaReloj`.`idmarca`,
        //    `marcaReloj`.`COD_EMPRESA_SYS`,
        //    `marcaReloj`.`NUM_TARJETA`,
        //    `marcaReloj`.`FECHA_MARCA`,
        //    `marcaReloj`.`RUT_FUNCIONARIO`,
        //    `marcaReloj`.`HORA_MARCA`,
        //    `marcaReloj`.`SENTIDO_MARCA`,
        //    `marcaReloj`.`NUM_RELOJ`,
        //    `marcaReloj`.`TIPO_MARCA`,
        //    `marcaReloj`.`ID_RESPONSABLE`,
        //    `marcaReloj`.`FECHA_TRANSACCION`,
        //    `marcaReloj`.`FECHA_INGRESO`,
        //    `marcaReloj`.`COD_SUCURSAL_SYS`,
        //    `marcaReloj`.`IP`,
        //    `marcaReloj`.`NOMBRE_PC`,
        //    `marcaReloj`.`OBSERVACION`,
        //    `marcaReloj`.`created_at`,
        //    `marcaReloj`.`updated_at`
        //FROM `teamrflex_tisal`.`marcaReloj`;

    }
}
