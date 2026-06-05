namespace proyectoNegocioRflex.Sincronizacion
{
    public class DeviceIdentity
    {
        public int IdReloj      { get; set; }
        public int SucursalId   { get; set; }
        public TipoDispositivo Tipo { get; set; }
        public bool Bloqueado   { get; set; }
        public bool AutoInicia  { get; set; }
        public int TipoRelojId  { get; set; } // FK tipoReloj_idtipoReloj — complementa soloEnrolador/relojCasino
    }
}
