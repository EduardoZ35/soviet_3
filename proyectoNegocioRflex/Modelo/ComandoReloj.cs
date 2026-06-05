using proyectoNegocioRflex.Utilidades;
using System;
using System.Data;

namespace proyectoNegocioRflex.Modelo
{
    /// <summary>
    /// Queries sobre la tabla cloud `comando_reloj`.
    /// Todos los métodos usan la conexión cloud write (conexionRelojServidor.txt).
    /// </summary>
    public class ComandoReloj
    {
        /// <summary>
        /// Devuelve el id del primer comando pendiente de ejecución para el reloj dado,
        /// o null si no hay ninguno. Un comando es pendiente si:
        ///   - cancelado = 0
        ///   - shutdown_enviado = 0
        ///   - ejecutar_en está entre (NOW() - 30 minutos) y NOW()
        /// La ventana de 30 min evita reinicios sorpresivos si el reloj estuvo offline.
        /// </summary>
        public int? TraerComandoPendiente(int idReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql =
                "SELECT id FROM comando_reloj " +
                "WHERE idreloj = " + idReloj +
                " AND cancelado = 0" +
                " AND shutdown_enviado = 0" +
                " AND ejecutar_en <= NOW()" +
                " AND ejecutar_en >= NOW() - INTERVAL 30 MINUTE" +
                " ORDER BY ejecutar_en ASC" +
                " LIMIT 1";
            DataTable dt = ejecutor.traerDatosDataTableServidor(sql);
            if (dt == null || dt.Rows.Count == 0) return null;
            var raw = dt.Rows[0][0];
            if (raw == null || raw == DBNull.Value) return null;
            return Convert.ToInt32(raw);
        }

        /// <summary>
        /// Devuelve el id del primer comando que fue enviado a shutdown pero luego cancelado,
        /// aún no abortado. El sincronizador debe llamar `shutdown /a` para estos.
        /// Condición: cancelado=1 AND shutdown_enviado=1 AND abortado=0.
        /// </summary>
        public int? TraerComandoParaAbortar(int idReloj)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql =
                "SELECT id FROM comando_reloj " +
                "WHERE idreloj = " + idReloj +
                " AND cancelado = 1" +
                " AND shutdown_enviado = 1" +
                " AND abortado = 0" +
                " ORDER BY id ASC" +
                " LIMIT 1";
            DataTable dt = ejecutor.traerDatosDataTableServidor(sql);
            if (dt == null || dt.Rows.Count == 0) return null;
            var raw = dt.Rows[0][0];
            if (raw == null || raw == DBNull.Value) return null;
            return Convert.ToInt32(raw);
        }

        /// <summary>
        /// Marca un comando como shutdown_enviado=1. Llamar inmediatamente después
        /// de ejecutar `shutdown /r /f /t 60`.
        /// </summary>
        public bool MarcarShutdownEnviado(int id)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql =
                "UPDATE comando_reloj SET shutdown_enviado = 1, updated_at = NOW()" +
                " WHERE id = " + id;
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        /// <summary>
        /// Marca un comando como abortado=1. Llamar inmediatamente después
        /// de ejecutar `shutdown /a`.
        /// </summary>
        public bool MarcarAbortado(int id)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql =
                "UPDATE comando_reloj SET abortado = 1, updated_at = NOW()" +
                " WHERE id = " + id;
            return ejecutor.ejecutarConsultaServidor(sql);
        }
    }
}
