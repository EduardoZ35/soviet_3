using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;

namespace proyectoNegocioRflex.Modelo
{
    /// <summary>
    /// Almacena el último updated_at de nube recibido por tabla.
    /// Evita el bug de timezone: el valor guardado es el timestamp REAL de nube,
    /// no un MAX(local) convertido con .AddHours().
    /// Uso:
    ///   var wm = new SyncWatermark();
    ///   wm.CrearTablaLocal();                           // una vez, al inicio
    ///   string desde = wm.TraerWatermark("Persona");   // cutoff para el WHERE
    ///   ... descargar batch donde updated_at > desde ...
    ///   wm.GuardarWatermark("Persona", maxUpdatedAtDelBatch);
    /// </summary>
    public class SyncWatermark
    {
        private const string FechaFallback = "2017-01-01 00:00:00";

        /// <summary>
        /// Crea la tabla sync_watermark en la BD local si no existe.
        /// Llamar una vez en el arranque del sincronizador.
        /// </summary>
        public void CrearTablaLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = @"CREATE TABLE IF NOT EXISTS `relojControl`.`sync_watermark` (
                `tabla` VARCHAR(100) NOT NULL,
                `ultimo_cloud_updated_at` DATETIME NOT NULL DEFAULT '2017-01-01 00:00:00',
                PRIMARY KEY (`tabla`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
            ejecutor.EjecutaSQLModificacionTabla(sql);
        }

        /// <summary>
        /// Devuelve el último updated_at de nube registrado para la tabla dada.
        /// Si no hay registro previo retorna "2017-01-01 00:00:00" (bootstrap completo).
        /// </summary>
        public string TraerWatermark(string tabla)
        {
            try
            {
                EjecutoresSql ejecutor = new EjecutoresSql();
                string sql = "SELECT ultimo_cloud_updated_at FROM relojControl.sync_watermark " +
                             "WHERE tabla='" + EscaparValor(tabla) + "' LIMIT 1";
                DataTable dt = ejecutor.traerDatosDataTable(sql);
                if (dt == null || dt.Rows.Count == 0)
                    return FechaFallback;
                string val = dt.Rows[0][0]?.ToString();
                if (string.IsNullOrWhiteSpace(val) || val == "0001-01-01 00:00:00")
                    return FechaFallback;
                // Asegurar formato MySQL
                if (DateTime.TryParse(val, out DateTime dt2))
                    return dt2.ToString("yyyy-MM-dd HH:mm:ss");
                return FechaFallback;
            }
            catch
            {
                return FechaFallback;
            }
        }

        /// <summary>
        /// Persiste el mayor updated_at del último batch descargado.
        /// cloudTimestamp: string en formato "yyyy-MM-dd HH:mm:ss" tal como viene del servidor.
        /// No modificar — es el valor real de nube, sin conversiones de zona horaria.
        /// </summary>
        public void GuardarWatermark(string tabla, string cloudTimestamp)
        {
            if (string.IsNullOrWhiteSpace(cloudTimestamp)) return;
            // Validar que sea una fecha parseable antes de persistir
            if (!DateTime.TryParse(cloudTimestamp, out DateTime parsed)) return;
            string valorSeguro = parsed.ToString("yyyy-MM-dd HH:mm:ss");

            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "INSERT INTO relojControl.sync_watermark (tabla, ultimo_cloud_updated_at) " +
                         "VALUES ('" + EscaparValor(tabla) + "', '" + valorSeguro + "') " +
                         "ON DUPLICATE KEY UPDATE ultimo_cloud_updated_at = '" + valorSeguro + "'";
            ejecutor.ejecutarConsulta(sql);
        }

        /// <summary>
        /// Dado un DataTable con una columna updated_at (columna 0 del resultado),
        /// extrae el MAX y lo guarda como watermark de la tabla indicada.
        /// Si el DataTable está vacío, no modifica el watermark existente.
        /// </summary>
        public void ActualizarWatermarkDesdeBatch(string tabla, DataTable batch, string columnaUpdatedAt = "updated_at")
        {
            if (batch == null || batch.Rows.Count == 0) return;
            if (!batch.Columns.Contains(columnaUpdatedAt)) return;

            DateTime maxFecha = DateTime.MinValue;
            foreach (DataRow row in batch.Rows)
            {
                var val = row[columnaUpdatedAt];
                if (val == null || val == DBNull.Value) continue;
                if (DateTime.TryParse(val.ToString(), out DateTime f) && f > maxFecha)
                    maxFecha = f;
            }
            if (maxFecha == DateTime.MinValue) return;
            GuardarWatermark(tabla, maxFecha.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// Devuelve todos los watermarks de la BD local en un único round-trip.
        /// Tablas sin registro aparecen ausentes del diccionario (el caller debe tratar ausencia como fallback).
        /// </summary>
        public Dictionary<string, string> TraerTodosLosWatermarks()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                EjecutoresSql ejecutor = new EjecutoresSql();
                DataTable dt = ejecutor.traerDatosDataTable(
                    "SELECT tabla, ultimo_cloud_updated_at FROM relojControl.sync_watermark");
                if (dt == null) return result;
                foreach (DataRow row in dt.Rows)
                {
                    string tabla = row[0]?.ToString();
                    string val   = row[1]?.ToString();
                    if (string.IsNullOrEmpty(tabla) || string.IsNullOrWhiteSpace(val)) continue;
                    if (val == "0001-01-01 00:00:00") continue;
                    if (DateTime.TryParse(val, out DateTime d))
                        result[tabla] = d.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            catch { /* return partial/empty result — caller falls back to FechaFallback */ }
            return result;
        }

        private static string EscaparValor(string s)
        {
            return s?.Replace("'", "''") ?? string.Empty;
        }
    }
}
