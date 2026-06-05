using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class AlertaError
    {

        public DataTable traerAlertasSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idalertaError, alerta, reloj_idreloj,empresa_idempresa,sucursal_idsucursal,respaldado,enviado,fechaAlerta,checksum,categoriaAlerta_idcategoriaAlerta from alertaError where respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerAlertasSinNotificar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select alertaError.idalertaError, alertaError.alerta,reloj.nombre,reloj.ubicacion ,empresa.razon_social,sucursal.nombre, alertaError.fechaAlerta, alertaError.checksum,empresa.rut_empresa,sucursal.calle,sucursal.numero_calle,sucursal.comuna,sucursal.ciudad,sucursal.region,sucursal.pais from alertaError,reloj,empresa,sucursal where alertaError.reloj_idreloj = reloj.idreloj and alertaError.empresa_idempresa = empresa.idempresa and alertaError.sucursal_idsucursal = sucursal.idsucursal and alertaError.enviado=0";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarAlertaLocal(string alerta, int reloj_idreloj, int empresa_idempresa, int sucursal_idsucursal, int respaldado, int enviado, string fechaAlerta, int categoriaAlerta_idcategoriaAlerta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion e = new Encriptacion();
            //ElChecksum se formará por el nombre de reloj_idreloj + empresa_idempresa + sucursal_idsucursal + la fecha de la alerta
            string checksum = e.Encriptar(reloj_idreloj + empresa_idempresa.ToString() + sucursal_idsucursal.ToString() + fechaAlerta);
            string sql = "insert into alertaError(alerta,reloj_idreloj,empresa_idempresa,sucursal_idsucursal,respaldado,enviado,fechaAlerta,checksum,categoriaAlerta_idcategoriaAlerta) values('" + alerta + "'," + reloj_idreloj + "," + empresa_idempresa + "," + sucursal_idsucursal + "," + respaldado + "," + enviado + ",'" + fechaAlerta + "','" + checksum + "'," + categoriaAlerta_idcategoriaAlerta + ")";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoRespaldadoAlertaLocal(int idalertaError)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update alertaError set respaldado=1 where idalertaError=" + idalertaError + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoEnviadoAlertaLocal(int idalertaError)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update alertaError set enviado=1 where idalertaError=" + idalertaError + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarAlertaNube(string alerta, int reloj_idreloj, int empresa_idempresa, int sucursal_idsucursal, int respaldado, int enviado, string fechaAlerta, string checksum, int categoriaAlerta_idcategoriaAlerta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into alertaError(alerta,reloj_idreloj,empresa_idempresa,sucursal_idsucursal,respaldado,enviado,fechaAlerta,checksum,categoriaAlerta_idcategoriaAlerta) values('" + alerta + "'," + reloj_idreloj + "," + empresa_idempresa + "," + sucursal_idsucursal + "," + respaldado + "," + enviado + ",'" + fechaAlerta + "','" + checksum + "'," + categoriaAlerta_idcategoriaAlerta + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarEstadoRespaldadoAlertaNube(string checksum)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update alertaError set respaldado=1 where checksum='" + checksum + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarEstadoEnviadoAlertaNube(string checksum)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update alertaError set enviado=1 where checksum='" + checksum + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

    }

}
