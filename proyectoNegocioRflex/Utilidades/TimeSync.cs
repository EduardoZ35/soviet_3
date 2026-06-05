using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace proyectoNegocioRflex.Utilidades
{
    /// <summary>
    /// Synchronises the local Windows clock against the cloud DB server's UTC timestamp.
    /// Intended to run on service startup so DST transitions are corrected automatically
    /// without requiring VNC/TeamViewer access to client machines.
    /// </summary>
    public static class TimeSync
    {
        // ── Win32 structs ───────────────────────────────────────────────────────

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort Year, Month, DayOfWeek, Day, Hour, Minute, Second, Milliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int  HighPart;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        // ── Win32 imports ───────────────────────────────────────────────────────

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SYSTEMTIME st);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(
            IntPtr tokenHandle, bool disableAll,
            ref TOKEN_PRIVILEGES newState, uint bufLen,
            IntPtr prevState, IntPtr returnLen);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint TOKEN_QUERY             = 0x0008;
        private const uint SE_PRIVILEGE_ENABLED    = 0x0002;
        private const string SE_SYSTEMTIME_NAME    = "SeSystemtimePrivilege";

        // ── public API ─────────────────────────────────────────────────────────

        private const string ChileTimeZoneId      = "Pacific SA Standard Time";
        private const double DriftThresholdMinutes = 2.0;

        private static readonly string[] CloudConnectionFiles =
        {
            @"C:\rflexapps\config\conexionRelojServidorLectura.txt",
            @"C:\rflexapps\config\conexionRelojServidor.txt",
        };

        /// <summary>
        /// Checks timezone and clock drift. Corrects both if needed.
        /// Returns a human-readable result string suitable for logging.
        /// Never throws.
        /// </summary>
        public static string SyncTime()
        {
            try
            {
                string tzResult = EnsureTimezone();

                string connFile = FindConnectionFile();
                if (connFile == null)
                    return $"{tzResult} | TimeSync: sin archivo conexión cloud — omitido";

                var ejecutor = new EjecutoresSql();
                var dt = ejecutor.traerDatosConexionDinamica("SELECT UTC_TIMESTAMP()", connFile);
                if (dt == null || dt.Rows.Count == 0)
                    return $"{tzResult} | TimeSync: cloud no respondió UTC";

                if (!DateTime.TryParse(dt.Rows[0][0]?.ToString(), out DateTime cloudUtc))
                    return $"{tzResult} | TimeSync: UTC cloud no parseable ({dt.Rows[0][0]})";

                DateTime localUtc = DateTime.UtcNow;
                TimeSpan drift    = cloudUtc - localUtc;

                if (Math.Abs(drift.TotalMinutes) <= DriftThresholdMinutes)
                    return $"{tzResult} | TimeSync: OK (drift {drift.TotalSeconds:F0}s)";

                bool ok = ApplySystemTime(cloudUtc);
                if (!ok)
                {
                    int err = Marshal.GetLastWin32Error();
                    string errDesc = err == 1314
                        ? "SeSystemtimePrivilege no disponible — el servicio no tiene privilegio para cambiar el reloj"
                        : $"Win32 error {err}";
                    return $"{tzResult} | TimeSync: SetSystemTime falló — drift {drift.TotalMinutes:F1} min — {errDesc}";
                }

                return $"{tzResult} | TimeSync: reloj corregido {drift.TotalMinutes:+0.0;-0.0} min";
            }
            catch (Exception ex)
            {
                return "TimeSync: excepción — " + ex.Message;
            }
        }

        // ── helpers ─────────────────────────────────────────────────────────────

        private static string EnsureTimezone()
        {
            try
            {
                if (TimeZoneInfo.Local.Id == ChileTimeZoneId)
                    return $"TZ: ya es '{ChileTimeZoneId}'";

                string oldId = TimeZoneInfo.Local.Id;
                var p = Process.Start(new ProcessStartInfo("tzutil.exe", $"/s \"{ChileTimeZoneId}\"")
                {
                    CreateNoWindow  = true,
                    UseShellExecute = false
                });
                // Capture WaitForExit return value — false means timeout (process still running).
                // p?.X ?? false handles the p-null case (process never launched).
                // On .NET 8 + UseShellExecute=false, Start() throws rather than returning null,
                // so p is always non-null here, but the null-guard is kept for safety.
                bool exited = p?.WaitForExit(5000) ?? false;
                // Invalidate CLR's cached timezone so subsequent TimeZoneInfo.Local.Id calls
                // reflect the change made by tzutil — otherwise the process-level cache holds
                // the old ID for its entire lifetime and tzutil re-fires on every SyncTime() call.
                TimeZoneInfo.ClearCachedData();
                // ExitCode is only valid when the process has exited; use -1 on timeout/null.
                int exitCode = exited ? p.ExitCode : -1;
                if (exitCode != 0)
                    return $"TZ: tzutil falló (exit {exitCode}) — timezone puede no haberse cambiado";
                return $"TZ: cambiada '{oldId}' → '{ChileTimeZoneId}'";
            }
            catch (Exception ex)
            {
                return $"TZ: error — {ex.Message}";
            }
        }

        private static string FindConnectionFile()
        {
            foreach (var path in CloudConnectionFiles)
                if (System.IO.File.Exists(path)) return path;
            return null;
        }

        private static bool ApplySystemTime(DateTime utc)
        {
            // SeSystemtimePrivilege must be explicitly enabled even for SYSTEM account.
            EnablePrivilege(SE_SYSTEMTIME_NAME);

            var st = new SYSTEMTIME
            {
                Year         = (ushort)utc.Year,
                Month        = (ushort)utc.Month,
                Day          = (ushort)utc.Day,
                Hour         = (ushort)utc.Hour,
                Minute       = (ushort)utc.Minute,
                Second       = (ushort)utc.Second,
                Milliseconds = (ushort)utc.Millisecond
            };
            return SetSystemTime(ref st);
        }

        private static void EnablePrivilege(string privilegeName)
        {
            if (!OpenProcessToken(GetCurrentProcess(),
                TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr token))
                return;
            try
            {
                if (!LookupPrivilegeValue(null, privilegeName, out LUID luid))
                    return;

                var tp = new TOKEN_PRIVILEGES
                {
                    PrivilegeCount = 1,
                    Luid           = luid,
                    Attributes     = SE_PRIVILEGE_ENABLED
                };
                AdjustTokenPrivileges(token, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            }
            finally
            {
                CloseHandle(token);
            }
        }
    }
}
