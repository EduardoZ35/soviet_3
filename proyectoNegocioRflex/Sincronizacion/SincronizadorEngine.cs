using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;

namespace proyectoNegocioRflex.Sincronizacion
{
    public class SincronizadorEngine
    {
        private readonly DeviceIdentity _device;
        private readonly Action<string> _tablaIniciada;
        private readonly Action<int> _marcarBootstrap;
        private readonly Action _watchdogKillAction;
        private readonly Action _blockKillAction; // Exit(0) on block — SCM won't restart

        private SincronizadorLegacy _legacy;
        private readonly Action _deltaAction; // test override for EjecutarDelta
        private volatile bool _corriendo; // volatile: ensures Detener() write is visible to loop threads immediately
        private volatile bool _pesadoCorriendo; // guard: prevents concurrent EjecutarSyncPesado instances
        private CancellationTokenSource _cts;

        // Catalog delta refresh: run bootstrapped F2 tables every N delta cycles (~15 min at 90s/cycle)
        private int _catalogDeltaCounter = 0;
        private const int CatalogDeltaInterval = 10;
        // Periodic clock sync: every ~20 min (13 cycles × 90s) correct drift silently
        private int _timeSyncCounter = 0;
        private const int TimeSyncInterval = 13;
        // Tables already handled by the hardcoded delta loop — skip in catalog refresh to avoid double-run.
        // RacionFija also excluded: massive cloud download, stays in SyncCompleto only.
        private static readonly HashSet<string> _deltaHandledTablas = new HashSet<string>
        {
            "Persona", "ConfigFuncionarioReloj", "ImagenHuella",
            "FuncionarioHasReloj", "Marca", "Rechazo", "LogSincronizador",
            "RacionFija", "InhabilitacionMarca"
        };
        // Cached step map — built once when _legacy is available, reused every catalog refresh cycle.
        private Dictionary<string, Action> _stepMapCache;

        // Watchdog state
        // _cycleActiveSince: 0 = cycle not running; nonzero = UtcNow.Ticks when current cycle started.
        // Single atomic field eliminates the race condition between the old separate
        // _cycleActive (volatile bool) + _cycleStartedTicks (long) pair, where the
        // watchdog could read _cycleActive=true but a stale _cycleStartedTicks from a
        // previous cycle (or from the field's default 0), triggering a spurious kill.
        private long _cycleActiveSince;
        private Timer _watchdogTimer;
        private const int WatchdogTimeoutMinutes = 10;

        // Injectable overrides for ChequearComandoReinicio — used in tests to avoid real DB/process calls.
        // In production these are null and the real ComandoReloj + Process.Start are used.
        private readonly Func<int, int?> _comandoPendienteOverride;
        private readonly Func<int, int?> _comandoAbortarOverride;
        private readonly Action<string> _shutdownOverride; // receives "/r /f /t 60" or "/a"

        public event Action<EstadoSync> OnEstadoCambiado;

        // Constructor for tests — no SincronizadorLegacy needed
        // deltaAction: inject a custom delta body (e.g. a blocking call to simulate hangs)
        // watchdogKillAction: inject a mock instead of calling Environment.Exit(1)
        // blockKillAction: inject a mock instead of calling Environment.Exit(0)
        // comandoPendienteOverride: inject fake TraerComandoPendiente (avoids cloud DB)
        // comandoAbortarOverride: inject fake TraerComandoParaAbortar (avoids cloud DB)
        // shutdownOverride: inject fake shutdown action (avoids calling real shutdown.exe)
        public SincronizadorEngine(
            DeviceIdentity device,
            Action<string> tablaIniciada,
            Action<int> marcarBootstrap,
            Action watchdogKillAction = null,
            Action deltaAction = null,
            Action blockKillAction = null,
            Func<int, int?> comandoPendienteOverride = null,
            Func<int, int?> comandoAbortarOverride = null,
            Action<string> shutdownOverride = null)
        {
            _device = device;
            _tablaIniciada = tablaIniciada ?? (t => { });
            _marcarBootstrap = marcarBootstrap;
            _watchdogKillAction = watchdogKillAction ?? (() => Environment.Exit(1));
            _blockKillAction    = blockKillAction    ?? (() => Environment.Exit(0));
            _deltaAction = deltaAction;
            _comandoPendienteOverride = comandoPendienteOverride;
            _comandoAbortarOverride   = comandoAbortarOverride;
            _shutdownOverride         = shutdownOverride;
        }

        // Constructor for production
        public SincronizadorEngine(DeviceIdentity device, Action<string> logAction)
        {
            _device = device;
            _legacy = new SincronizadorLegacy(device, logAction);
            _tablaIniciada = logAction ?? (t => { });
            _marcarBootstrap = (id) =>
            {
                var r = new Reloj();
                r.MarcarBootstrapCompletado(id);
            };
            _watchdogKillAction = () => Environment.Exit(1);
            _blockKillAction    = () => Environment.Exit(0);
        }

        public void EjecutarBootstrap()
        {
            RaiseEstado(new EstadoSync { Fase = FaseSync.Bootstrap, Mensaje = "Iniciando bootstrap..." });

            // Heartbeat inmediato: insertar en avisoConexion antes del sync largo
            TrySync("Heartbeat", () => new Modelo.Reloj().ReportarHeartbeatNube(_device.IdReloj));

            var fase1 = TablasPrioritarias.GetFase1(_device.Tipo);

            if (_legacy == null)
            {
                // Test mode: call _tablaIniciada per table, swallow exceptions to test resilience
                foreach (var tabla in fase1)
                {
                    try { _tablaIniciada(tabla); }
                    catch { /* continue with next table */ }
                }
            }
            else
            {
                EjecutarTablas(fase1);
            }

            _marcarBootstrap(_device.IdReloj);
            RaiseEstado(new EstadoSync { Fase = FaseSync.Bootstrap, Mensaje = "Bootstrap completado.", PorcentajeFase = 100 });
        }

        public void EjecutarSyncCompleto()
        {
            RaiseEstado(new EstadoSync { Fase = FaseSync.SyncCompleto, Mensaje = "Iniciando sync completo..." });
            if (_legacy != null)
                EjecutarTablas(TablasPrioritarias.GetFase2(_device.Tipo), FaseSync.SyncCompleto);
            RaiseEstado(new EstadoSync { Fase = FaseSync.SyncCompleto, Mensaje = "Sync completo.", PorcentajeFase = 100 });
        }

        public void EjecutarDelta()
        {
            if (_legacy == null)
            {
                _deltaAction?.Invoke(); // test hook — default no-op
                return;
            }
            RaiseEstado(new EstadoSync { Fase = FaseSync.Delta, Mensaje = "Delta sync..." });
            TrySync("Heartbeat",             () => new Modelo.Reloj().ReportarHeartbeatNube(_device.IdReloj));
            TrySync("ConfigCheck",           () => ChequearConfigRemota());
            // Flujo de enrolamiento: tablas escritas localmente con respaldado=0 — no-op si no hay registros nuevos
            TrySync("Persona",               () => _legacy.SincronizarPersonas());
            TrySync("ConfigFuncionarioReloj", () => _legacy.SincronizarConfigFuncionarios());
            TrySync("ImagenHuella",           () => _legacy.SubirHuellasLocalesANube());
            TrySync("FuncionarioHasReloj",    () => _legacy.SubirPermisosLocalesANube());
            // RacionFija excluida del delta: descarga cloud masiva (100k+ filas) bloquea ciclos de 90s.
            // Corre en SyncCompleto (background) y SyncPesado.
            // Asistencia + inhabilitaciones (requieren baja latencia)
            TrySync("Marca",                  () => _legacy.SincronizarMarcas());
            TrySync("Rechazo",                () => _legacy.SincronizarRechazos());
            TrySync("InhabilitacionMarca",    () => _legacy.SincronizarInhabilitaciones());
            ActualizarWatermarkLocal("InhabilitacionMarca"); // actualiza sync_watermark para dashboard
            TrySync("Log",                    () => _legacy.SincronizarLogs());

            // Catalog refresh: every CatalogDeltaInterval cycles, sync F2 tables that have
            // been bootstrapped (watermark set). Skips tables in _deltaHandledTablas (incl. RacionFija).
            if (++_catalogDeltaCounter >= CatalogDeltaInterval)
            {
                _catalogDeltaCounter = 0;
                if (_stepMapCache == null) _stepMapCache = BuildStepMap(); // build once, reuse every cycle
                // Single DB round-trip for all watermarks instead of one query per table.
                var allWatermarks = new SyncWatermark().TraerTodosLosWatermarks();
                foreach (var tabla in TablasPrioritarias.GetFase2(_device.Tipo))
                {
                    if (_deltaHandledTablas.Contains(tabla)) continue;
                    // Skip if not yet bootstrapped (no watermark or fallback sentinel).
                    if (!allWatermarks.TryGetValue(tabla, out string watermark)) continue;
                    if (watermark == "2017-01-01 00:00:00") continue; // bootstrap sentinel — not yet synced
                    // Only update watermark if a real sync step exists — avoids false "synced" stamps.
                    if (!_stepMapCache.TryGetValue(tabla, out var step)) continue;
                    TrySync(tabla, step);
                    ActualizarWatermarkLocal(tabla);
                }
            }

            // Periodic clock sync: every ~20 min check drift against cloud UTC and correct if needed.
            // Wrapped in try/catch — a faulted TimeSync task would otherwise escape as AggregateException.
            if (++_timeSyncCounter >= TimeSyncInterval)
            {
                _timeSyncCounter = 0;
                try
                {
                    var timeSyncTask = Task.Run(() => TimeSync.SyncTime());
                    if (timeSyncTask.Wait(TimeSpan.FromSeconds(5)))
                        _tablaIniciada("TimeSync: " + timeSyncTask.Result);
                }
                catch { /* TimeSync failed — silent, never blocks delta cycle */ }
            }
        }

        // watchdogTimeout / watchdogCheckInterval: override in tests to avoid waiting 10 minutes
        public void IniciarLoop(
            int intervaloSegundos = 90,
            TimeSpan? watchdogTimeout = null,
            TimeSpan? watchdogCheckInterval = null)
        {
            var timeout       = watchdogTimeout       ?? TimeSpan.FromMinutes(WatchdogTimeoutMinutes);
            var checkInterval = watchdogCheckInterval ?? TimeSpan.FromMinutes(1);

            _cts = new CancellationTokenSource();
            _corriendo = true;

            // Ensure sync_watermark table exists before any sync operation runs
            try { new SyncWatermark().CrearTablaLocal(); } catch { /* non-fatal */ }

            // Watchdog: si un delta cycle no completa en el timeout, mata el proceso
            // para que SCM lo reinicie automáticamente.
            _watchdogTimer = new Timer(_ =>
            {
                try
                {
                    long startedAt = Interlocked.Read(ref _cycleActiveSince);
                    if (startedAt == 0L) return;
                    TimeSpan elapsed = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - startedAt);
                    if (elapsed < timeout) return;

                    // Kill FIRST — don't let logging block the process restart.
                    // In production this calls Environment.Exit(1); in tests it sets a flag.
                    _watchdogKillAction();

                    // Best-effort log after kill is triggered (may not complete if process exits)
                    Task.Run(() => LogError("Watchdog", new TimeoutException(
                        $"Delta cycle lleva {elapsed.TotalMinutes:F1} min sin completar — proceso reiniciado.")));
                }
                catch { /* never let timer callback throw */ }
            }, null, checkInterval, checkInterval);

            Task.Run(() =>
            {
                while (_corriendo && !_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        Interlocked.Exchange(ref _cycleActiveSince, DateTime.UtcNow.Ticks);
                        EjecutarDelta();
                    }
                    catch (Exception ex) { LogError("DeltaLoop", ex); }
                    finally { Interlocked.Exchange(ref _cycleActiveSince, 0L); }
                    Thread.Sleep(intervaloSegundos * 1000);
                }
            }, _cts.Token);
        }

        public void EjecutarSyncPesado()
        {
            if (_legacy == null) return;
            RaiseEstado(new EstadoSync { Fase = FaseSync.SyncCompleto, Mensaje = "Sync pesado: FuncionarioHasReloj..." });
            TrySync("FuncionarioHasReloj", () => _legacy.SincronizarFuncionarioHasReloj());

            // Set-diff download: FHR-filtered for Asistencia/Casino, unfiltered for Enrolador.
            // Runs here (not Bootstrap) to avoid blocking heartbeat.
            RaiseEstado(new EstadoSync { Fase = FaseSync.SyncCompleto, Mensaje = "Sync pesado: ImagenHuella set-diff..." });
            if (_device != null)
            {
                // TipoRelojId==1 cubre enroladores con soloEnrolador=0 (ej. Dávila ids 88,96).
                // Sin esto, DescargarHuellasNuevasDeNube filtra por idReloj y cada dispositivo
                // solo tiene sus propias huellas → operador no detecta duplicados → doble enrolamiento.
                bool esEnrolador = _device.Tipo == TipoDispositivo.Enrolador
                                || _device.TipoRelojId == 1;
                TrySync("ImagenHuellaSetDiff",
                    () => _legacy.DescargarHuellasNuevasDeNube(esEnrolador ? 0 : _device.IdReloj));
            }

            RaiseEstado(new EstadoSync { Fase = FaseSync.SyncCompleto, Mensaje = "Sync pesado completado.", PorcentajeFase = 100 });
        }

        // Sequential slow loop — sleeps AFTER completion so runs never overlap.
        // hardCycleTimeout: CancellationToken timeout per individual run (default 4h).
        public void IniciarLoopPesado(
            int intervaloMinutos = 60,
            TimeSpan? hardCycleTimeout = null)
        {
            var cycleTimeout = hardCycleTimeout ?? TimeSpan.FromHours(4);

            Task.Run(() =>
            {
                while (_corriendo && !_cts.Token.IsCancellationRequested)
                {
                    using (var cycleCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token))
                    {
                        cycleCts.CancelAfter(cycleTimeout);
                        try
                        {
                            // Guard: skip if a previous iteration is still running (e.g. abandoned
                            // after 4h timeout). Without this, two concurrent EjecutarSyncPesado
                            // instances write to the same local tables → duplicate-key errors.
                            if (_pesadoCorriendo)
                            {
                                LogError("SyncPesadoLoop", new InvalidOperationException(
                                    "Saltando ciclo pesado — iteración anterior aún en ejecución."));
                            }
                            else
                            {
                                _pesadoCorriendo = true;
                                // EjecutarSyncPesado doesn't accept CancellationToken — run it on a
                                // separate thread and wait with the timeout token. If the token fires
                                // the wait throws but the underlying task is abandoned (still running
                                // until its own DB timeout or service stop). _pesadoCorriendo stays
                                // true so the next iteration is skipped — prevents concurrent runs.
                                var syncTask = Task.Run(() =>
                                {
                                    try { EjecutarSyncPesado(); }
                                    finally { _pesadoCorriendo = false; }
                                });
                                syncTask.Wait(cycleCts.Token);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Distinguish clean service stop from actual 4h timeout in logs.
                            if (_cts.Token.IsCancellationRequested)
                                LogError("SyncPesadoLoop", new OperationCanceledException(
                                    "Sync pesado interrumpido por parada del servicio."));
                            else
                                LogError("SyncPesadoLoop", new TimeoutException(
                                    $"Sync pesado superó {cycleTimeout.TotalHours:F0}h — cancelado."));
                        }
                        catch (AggregateException ae) when (ae.InnerException != null)
                        {
                            LogError("SyncPesadoLoop", ae.InnerException);
                        }
                        catch (Exception ex)
                        {
                            LogError("SyncPesadoLoop", ex);
                        }
                    }
                    // Sleep after completion — interval measured from end of run, not start
                    for (int s = 0; s < intervaloMinutos * 60; s += 5)
                    {
                        if (!_corriendo || _cts.Token.IsCancellationRequested) return;
                        Thread.Sleep(5000);
                    }
                }
            }, _cts.Token);
        }

        public void Detener()
        {
            _corriendo = false;
            Interlocked.Exchange(ref _cycleActiveSince, 0L);
            _watchdogTimer?.Dispose();
            _watchdogTimer = null;
            _cts?.Cancel();
            RaiseEstado(new EstadoSync { Fase = FaseSync.Detenido, Mensaje = "Servicio detenido." });
        }

        private Dictionary<string, Action> BuildStepMap()
        {
            return new Dictionary<string, Action>
            {
                { "Reloj",                                 () => _legacy.SincronizarReloj() },
                { "TipoReloj",                             () => _legacy.SincronizarTipoReloj() },
                { "Persona",                               () => _legacy.SincronizarPersonas() },
                // Bootstrap: upload-only (upsert — no DELETE needed).
                // Download set-diff runs in SyncPesado (background loop) to avoid blocking Bootstrap/heartbeat.
                { "ImagenHuella",                          () => _legacy.SubirHuellasLocalesANube() },
                { "ConfigFuncionarioReloj",                () => _legacy.SincronizarConfigFuncionarios() },
                { "FuncionarioHasReloj",                   () => _legacy.SincronizarFuncionarioHasReloj() },
                { "TipoMarca",                             () => _legacy.SincronizarTipoMarca() },
                { "ResolucionMarca",                       () => _legacy.SincronizarResolucionMarca() },
                { "PerfilCasino",                          () => _legacy.SincronizarPerfilesCasino() },
                { "DetallePerfilCasino",                   () => _legacy.SincronizarDetallePerfilCasino() },
                { "TipoComida",                            () => _legacy.SincronizarTipoComida() },
                { "SucursalTipoComida",                    () => _legacy.SincronizarSucursalTipoComida() },
                { "RacionFija",                            () => _legacy.SincronizarRacionesFijas() },
                { "TipoInhabilitacionMarca",               () => _legacy.SincronizarTipoInhabilitacionMarca() },
                { "TipoRechazo",                           () => _legacy.SincronizarTipoRechazo() },
                { "TipoDetalleMarcaComida",                () => _legacy.SincronizarTipoDetalleMarcaComida() },
                { "TipoTurno",                             () => _legacy.SincronizarTipoTurno() },
                { "TipoRolUsuario",                        () => _legacy.SincronizarTipoRolUsuario() },
                { "CategoriaAlerta",                       () => _legacy.SincronizarCategoriaAlerta() },
                { "TipoComidaHasTipoInhabilitacionMarca",  () => _legacy.SincronizarTipoComidaHasTipoInhabilitacion() },
                { "CorreoAlerta",                          () => _legacy.SincronizarCorreosAlertas() },
                { "AlertaError",                           () => _legacy.SincronizarAlertasError() },
                { "Empresa",                               () => _legacy.SincronizarEmpresas() },
                { "Sucursal",                              () => _legacy.SincronizarSucursales() },
                { "InhabilitacionMarca",                   () => _legacy.SincronizarInhabilitaciones() },
                { "Marca",                                 () => _legacy.SincronizarMarcas() },
                { "Rechazo",                               () => _legacy.SincronizarRechazos() },
                { "LogSincronizador",                      () => _legacy.SincronizarLogs() },
            };
        }

        private void EjecutarTablas(IReadOnlyList<string> tablas, FaseSync fase = FaseSync.Bootstrap)
        {
            var steps = BuildStepMap();
            int i = 0;
            foreach (var tabla in tablas)
            {
                i++;
                RaiseEstado(new EstadoSync
                {
                    Fase = fase,
                    Mensaje = $"Sincronizando {tabla}...",
                    PorcentajeFase = (int)((float)i / tablas.Count * 100)
                });
                TrySync(tabla, steps.TryGetValue(tabla, out var step) ? step : null);
                ActualizarWatermarkLocal(tabla);
            }
        }

        // Maps PascalCase table names that have snake_case names in the local DB.
        private static readonly Dictionary<string, string> _localTableNames =
            new Dictionary<string, string>
            {
                { "ConfigFuncionarioReloj",               "config_funcionario_reloj" },
                { "FuncionarioHasReloj",                  "funcionario_has_reloj" },
                { "SucursalTipoComida",                   "sucursal_tipocomida" },
                { "TipoComidaHasTipoInhabilitacionMarca", "tipocomida_has_tipoinhabilitacionmarca" },
            };

        // After each bootstrap/pesado table sync, stamp the watermark from local MAX(updated_at).
        // Persona is excluded — its watermark is set from the cloud response in SincronizadorLegacy
        // to preserve the real cloud timestamp. Using local MAX here would overwrite it.
        private void ActualizarWatermarkLocal(string tabla)
        {
            if (tabla == "Persona") return; // handled in SincronizadorLegacy with cloud timestamp
            try
            {
                string mysqlName = _localTableNames.TryGetValue(tabla, out var n)
                    ? n
                    : (string.IsNullOrEmpty(tabla) ? tabla : char.ToLower(tabla[0]) + tabla.Substring(1));
                if (!Regex.IsMatch(mysqlName ?? "", @"^[a-zA-Z][a-zA-Z0-9_]*$")) return;
                var dt = new EjecutoresSql().traerDatosDataTable(
                    $"SELECT MAX(updated_at) FROM relojControl.`{mysqlName}`");
                if (dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return;
                string maxVal = dt.Rows[0][0]?.ToString();
                if (string.IsNullOrWhiteSpace(maxVal)) return;
                new SyncWatermark().GuardarWatermark(tabla, maxVal);
            }
            catch { /* non-fatal — watermark is best-effort */ }
        }

        private void TrySync(string nombre, Action step)
        {
            if (step == null) return;
            try { step(); }
            catch (Exception ex) { LogError(nombre, ex); }
        }

        private void ChequearConfigRemota()
        {
            var dt = new Modelo.Reloj().traerIdentificacionRelojPorNombre();
            if (dt == null || dt.Rows.Count == 0) return; // sin conexión — ignorar, no actuar

            bool bloqueado = dt.Rows[0][4].ToString() == "1";
            if (bloqueado)
            {
                LogError("ConfigCheck", new InvalidOperationException(
                    $"Reloj '{Environment.MachineName}' fue bloqueado remotamente. Deteniendo servicio."));
                Detener();
                _blockKillAction(); // Exit(0) — SCM no reinicia en exit limpio
                return;
            }

            var nuevoTipo = IdentificadorReloj.DeterminarTipo(
                int.Parse(dt.Rows[0][2].ToString()),
                int.Parse(dt.Rows[0][3].ToString()));

            int nuevoTipoRelojId = int.Parse(dt.Rows[0][6].ToString());

            if (nuevoTipo != _device.Tipo || nuevoTipoRelojId != _device.TipoRelojId)
            {
                LogError("ConfigCheck", new InvalidOperationException(
                    $"Tipo cambió (Tipo: {_device.Tipo}→{nuevoTipo}, TipoRelojId: {_device.TipoRelojId}→{nuevoTipoRelojId}). Reiniciando para re-bootstrap."));
                _watchdogKillAction(); // Exit(1) — SCM reinicia con nuevo tipo
                return;
            }

            // Remote restart command check — runs every delta cycle (~90s)
            ChequearComandoReinicio();
        }

        /// <summary>
        /// Verifica si hay un comando de reinicio pendiente o un abort pendiente para este reloj.
        /// Llamado desde ChequearConfigRemota() en cada ciclo delta.
        /// En modo test (_legacy == null): usa overrides inyectados; no llama a la BD real
        /// ni ejecuta Process.Start real.
        /// </summary>
        public void ChequearComandoReinicio()
        {
            // A) Ejecutar reinicio pendiente
            int? pendienteId = _comandoPendienteOverride != null
                ? _comandoPendienteOverride(_device.IdReloj)
                : new Modelo.ComandoReloj().TraerComandoPendiente(_device.IdReloj);

            if (pendienteId.HasValue)
            {
                if (_shutdownOverride != null)
                    _shutdownOverride("/r /f /t 60");
                else
                    System.Diagnostics.Process.Start("shutdown", "/r /f /t 60");

                if (_legacy != null)
                    new Modelo.ComandoReloj().MarcarShutdownEnviado(pendienteId.Value);
                LogAdvertencia("ComandoReinicio", "Reinicio remoto iniciado.");
            }

            // B) Abortar reinicio ya enviado
            int? abortarId = _comandoAbortarOverride != null
                ? _comandoAbortarOverride(_device.IdReloj)
                : new Modelo.ComandoReloj().TraerComandoParaAbortar(_device.IdReloj);

            if (abortarId.HasValue)
            {
                if (_shutdownOverride != null)
                    _shutdownOverride("/a");
                else
                    System.Diagnostics.Process.Start("shutdown", "/a");

                if (_legacy != null)
                    new Modelo.ComandoReloj().MarcarAbortado(abortarId.Value);
                LogAdvertencia("ComandoReinicio", "Reinicio remoto abortado.");
            }
        }

        private void LogAdvertencia(string contexto, string mensaje)
        {
            try
            {
                var h = new Herramientas();
                h.generarLogSincronizador($"{contexto}: {mensaje}", "ADVERTENCIA");
            }
            catch { /* don't let logging failure crash the sync loop */ }
        }

        private void RaiseEstado(EstadoSync estado) => OnEstadoCambiado?.Invoke(estado);

        private void LogError(string contexto, Exception ex)
        {
            try
            {
                var h = new Herramientas();
                h.generarLogSincronizador($"Error en sync {contexto}: {ex.Message}", "ERROR");
            }
            catch { /* don't let logging failure crash the sync loop */ }
        }
    }
}
