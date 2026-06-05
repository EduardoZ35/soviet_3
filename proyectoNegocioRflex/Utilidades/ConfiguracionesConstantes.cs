namespace proyectoNegocioRflex.Utilidades
{
    public static class ConfiguracionesConstantes
    {

        //Variable que sirve para indicar si en este momento se está ejecutando el programa
        //en modo de prueba o desarrollo (con esto se muestra que la versión actual es de prueba y no 
        //se refleja un numero de versión).
        //Tambien inhabilita la comprobación de funcionamiento de los otros programas.
        public const bool EN_DESARROLLO = false;

        //Variable que indica a que url se debe conectar el reloj para comprobar conexión.
        public const string URL_COMPROBAR_PING = "https://relojes.rflex.io/ping";


        //Nombres de proyecto
        public const string PROYECTO_RELOJ_CONTROL = "relojControl";
        public const string PROYECTO_SINCRONIZADOR = "sincronizadorReloj";
        public const string PROYECTO_SINCRONIZADOR_CASINO = "sincronizadorCasino";

        public const string IP_LOCAL = "127.0.0.1";

        //Titulos pantallas de marca
        public const string TIPO_MARCA_ASISTENCIA = "Registro de asistencia";
        public const string TIPO_MARCA_CASINO = "Registro de casino";

        //Categorias de notificaciones
        public const int CATEGORIA_NOTIFICACION_ENERGIA = 1;
        public const int CATEGORIA_NOTIFICACION_IMPRESORA = 2;
        public const int CATEGORIA_NOTIFICACION_HUELLERO = 3;
        public const int CATEGORIA_NOTIFICACION_ERROR_BIOMETRICO = 4;
        public const int CATEGORIA_NOTIFICACION_CONEXION = 5;
        public const int CATEGORIA_NOTIFICACION_ENROLAMIENTO = 6;
        public const int CATEGORIA_NOTIFICACION_FALLA = 7;
        public const int CATEGORIA_NOTIFICACION_CONFIGURACION = 8;
        public const int CATEGORIA_NOTIFICACION_ERROR_NOTIFICACIONES = 9;
        public const int CATEGORIA_NOTIFICACION_OTROS = 10;

        //Tipos de marcas
        public const int MARCA_ENTRADA = 1;
        public const int MARCA_SALIDA = 2;
        public const int MARCA_INICIO_COMIDA = 3;
        public const int MARCA_TERMINO_COMIDA = 4;
        public const int MARCA_CASINO = 5;

        //tiposInhabilitacion de marca
        public const int INHABILITACION_JORNADA = 1;
        public const int INHABILITACION_MARCA_ENTRADA = 6;
        public const int INHABILITACION_MARCA_SALIDA = 7;

        //tipoRechazo 
        public const int RECHAZO_HUELLA_NO_RECONOCIDA = 1;
        public const int RECHAZO_PIN_MARCA_INCORRECTO = 2;
        public const int RECHAZO_MARCA_DESAYUNO_YA_REGISTRADA = 3;
        public const int RECHAZO_MARCA_ALMUERZO_YA_REGISTRADA = 4;
        public const int RECHAZO_MARCA_ONCE_YA_REGISTRADA = 5;
        public const int RECHAZO_MARCA_CENA_YA_REGISTRADA = 6;
        public const int RECHAZO_MARCA_INICIO_JORNADA_YA_REGISTRADA = 7;
        public const int RECHAZO_MARCA_TERMINO_JORNADA_YA_REGISTRADA = 8;
        public const int RECHAZO_MARCA_TERMINO_COMIDA_YA_REGISTRADA = 9;
        public const int RECHAZO_MARCA_COMIDA_FUERA_HORARIO = 10;
        public const int RECHAZO_MARCA_TERMINO_DESAYUNO_YA_REGISTRADA = 11;
        public const int RECHAZO_MARCA_TERMINO_ALMUERZO_YA_REGISTRADA = 12;
        public const int RECHAZO_MARCA_TERMINO_ONCE_YA_REGISTRADA = 13;
        public const int RECHAZO_MARCA_TERMINO_CENA_YA_REGISTRADA = 14;
        public const int RECHAZO_MARCA_CASINO_CORRESPONDIENTE_YA_REGISTRADA = 15;
        public const int RECHAZO_MARCA_INICIO_COMIDA_YA_REGISTRADA = 16;

        //Vencimiento de ticket por defecto
        public const int DIAS_VENCIMIENTO_TICKET = 30;
    }
}
