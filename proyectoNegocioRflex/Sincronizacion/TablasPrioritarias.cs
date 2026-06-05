using System.Collections.Generic;

namespace proyectoNegocioRflex.Sincronizacion
{
    public static class TablasPrioritarias
    {
        private static readonly List<string> _baseF1 = new List<string>
        {
            "LogSincronizador",
            "Empresa", "Sucursal", "TipoRolUsuario",
            "TipoMarca", "ResolucionMarca", "TipoInhabilitacionMarca",
            "TipoRechazo", "TipoDetalleMarcaComida", "TipoTurno", "CategoriaAlerta",
            "Reloj", "TipoReloj",
            "Persona", "ConfigFuncionarioReloj"
            // FuncionarioHasReloj removido de bootstrap — 1M+ filas, va en SyncCompleto
        };

        public static IReadOnlyList<string> GetFase1(TipoDispositivo tipo)
        {
            var lista = new List<string>(_baseF1);
            switch (tipo)
            {
                case TipoDispositivo.Enrolador:
                    lista.Add("ImagenHuella");
                    break;
                case TipoDispositivo.Asistencia:
                    lista.Add("TipoMarca");
                    lista.Add("ResolucionMarca");
                    break;
                case TipoDispositivo.Casino:
                    lista.Add("TipoMarca");
                    lista.Add("ResolucionMarca");
                    lista.Add("PerfilCasino");
                    lista.Add("DetallePerfilCasino");
                    lista.Add("TipoComida");
                    lista.Add("SucursalTipoComida");
                    // RacionFija removida de Bootstrap: descarga masiva (cientos de miles de filas) bloquea
                    // el Bootstrap indefinidamente. Va en SyncCompleto (background) como FuncionarioHasReloj.
                    break;
            }
            return lista;
        }

        public static IReadOnlyList<string> GetFase2(TipoDispositivo tipo)
        {
            var todas = new List<string>(GetTodas());
            var fase1 = new HashSet<string>(GetFase1(tipo));
            todas.RemoveAll(t => fase1.Contains(t));
            return todas;
        }

        public static IReadOnlyList<string> GetTodas()
        {
            return new List<string>
            {
                // ImagenHuella excluida: se sincroniza sólo en Bootstrap (Fase1) vía GetFase1().
                // Tenerla en Fase2 (SyncCompleto) dispara un bidir completo que descarga el histórico
                // del cloud y crea duplicados masivos (bug detectado 2026-05-29).
                "Reloj", "TipoReloj", "Persona",
                "ConfigFuncionarioReloj", "FuncionarioHasReloj",
                "TipoMarca", "ResolucionMarca",
                "PerfilCasino", "DetallePerfilCasino",
                "TipoComida", "SucursalTipoComida", "RacionFija",
                "TipoInhabilitacionMarca", "TipoRechazo",
                "TipoDetalleMarcaComida", "TipoTurno", "TipoRolUsuario",
                "CategoriaAlerta", "TipoComidaHasTipoInhabilitacionMarca",
                "CorreoAlerta", "AlertaError", "Empresa", "Sucursal",
                "InhabilitacionMarca", "Marca", "Rechazo", "LogSincronizador"
            };
        }
    }
}
