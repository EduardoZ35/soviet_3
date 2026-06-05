using System;

namespace proyectoNegocioRflex.Sincronizacion
{
    public enum FaseSync { Identificando, Bootstrap, SyncCompleto, Delta, Detenido, Error }

    public class EstadoSync
    {
        public FaseSync Fase      { get; set; }
        public string Mensaje     { get; set; }
        public int PorcentajeFase { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
