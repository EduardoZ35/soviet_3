using proyectoNegocioRflex.Utilidades;
using System;
using System.Data;

namespace proyectoNegocioRflex.Modelo
{
    public class ActualizacionesTablasBDLocal
    {

        /// <summary>
        /// Función principal de la clase
        /// esta función entrega la verificación si es que existe una columna en la base de datos local.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="tabla"></param>
        /// <param name="columna"></param>
        /// <returns></returns>
        private DataTable comprobarColumnaExistente(string tabla, string columna)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "Show columns from relojControl." + tabla + " like '" + columna + "'";
            DataTable dt = ejecutor.traerDatosDataTable(sql);
            return dt;
        }

        private DataTable comprobarTablaExistente(string tabla)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT * FROM information_schema.tables WHERE table_schema = 'relojControl' AND table_name = '" + tabla + "' LIMIT 1;";
            DataTable dt = ejecutor.traerDatosDataTable(sql);
            return dt;
        }

        /// <summary>
        /// Función principal de la clase... 
        /// en esta función debemos ir guardando las actualizaciones de las tablas de la base de datos.
        /// por cada inicio del sincronizador se verificará esta función
        /// </summary>
        public void actualizarTablasYColumnasModeloLocal()
        {
            //Agrega la columna codigoMarca en la tabla marca

            //Corrige nombre de columna descripción en tabla tipoTurno.
            generarActualizacionColumnas("tipoTurno", "descripci�n", "ALTER TABLE `relojControl`.`tipoTurno` CHANGE COLUMN `descripci�n` `descripcion` VARCHAR(300) CHARACTER SET 'utf8' COLLATE 'utf8_unicode_ci' NOT NULL; ", false, true);

            //Corrige nombre de columna descripción en tabla tipoTurno.
            generarActualizacionColumnas("tipoTurno", "descripción", "ALTER TABLE `relojControl`.`tipoTurno` CHANGE COLUMN `descripción` `descripcion` VARCHAR(300) CHARACTER SET 'utf8' COLLATE 'utf8_unicode_ci' NOT NULL; ", false, true);

            //Corrige nombre de columna descripción en tabla tipoTurno.
            generarActualizacionColumnas("tipoTurno", "descripci?n", "ALTER TABLE `relojControl`.`tipoTurno` CHANGE COLUMN `descripci?n` `descripcion` VARCHAR(300) CHARACTER SET 'utf8' COLLATE 'utf8_unicode_ci' NOT NULL; ", false, true);


            //Agrega columna codigoMarca en tabla marca.
            generarActualizacionColumnas("marca", "codigoMarca", "ALTER TABLE `relojControl`.`marca`ADD COLUMN `codigoMarca` INT NULL DEFAULT NULL COMMENT 'Se genera concatenando el id de la marca con el id del reloj' AFTER `sucursal_idsucursal`;", true);

            //Elimina Tabla columna codigoMarca de la tabla marca
            //generarActualizacionTablas("marca", "codigoMarca", "ALTER TABLE `relojControl`.`marca` DROP COLUMN `codigoMarca`;", false);


            //Agrega columna autoIniciaPrograma en la tabla reloj
            //generarActualizacionTablas("reloj", "autoIniciaPrograma", "ALTER TABLE `relojControl`.`reloj`ADD COLUMN `autoIniciaPrograma` BIT NULL DEFAULT 1 AFTER `resolucionMarca_idresolucionMarca`;", true);

            //Agrega columna empresa_idempresa en tabla persona.
            generarActualizacionColumnas("persona", "empresa_idempresa", "ALTER TABLE `relojControl`.`persona` ADD COLUMN `empresa_idempresa` INT(10) NULL DEFAULT 1 AFTER `habilitada`;", true);

            //Elimina Tabla columna empresa_idempresa
            //generarActualizacionTablas("persona", "empresa_idempresa", "ALTER TABLE `relojControl`.`persona` DROP COLUMN `empresa_idempresa`;", false);



            //Crea la tabla puntoCanje
            generarActualizacionTablas("puntoCanje", " CREATE TABLE  `relojControl`.`puntoCanje` (" +
                                          "`idpuntoCanje` INT NOT NULL AUTO_INCREMENT," +
                                          "`rutPuntoCanje` VARCHAR(45) NULL DEFAULT ''," +
                                          "`nombrePuntoCanje` VARCHAR(100) NULL DEFAULT ''," +
                                          "`puntoCanjeRS` VARCHAR(100) NULL DEFAULT ''," +
                                          "`direccionCM` VARCHAR(250) NULL DEFAULT ''," +
                                          "`direccionPuntoCanje` VARCHAR(250) NULL DEFAULT ''," +
                                          "`empresa_idempresa` INT NULL DEFAULT 1," +
                                          "`cobroLiberado` BIT NULL DEFAULT 0," +
                                          "`habilitado` BIT NULL DEFAULT 1," +
                                          "`created_by` VARCHAR(45) NULL DEFAULT ''," +
                                          "`updated_by` VARCHAR(45) NULL DEFAULT ''," +
                                          "`created_at` TIMESTAMP NULL DEFAULT current_timestamp()," +
                                          "`updated_at` TIMESTAMP NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()," +
                                          " PRIMARY KEY(`idpuntoCanje`))" +
                                          " ENGINE = InnoDB " +
                                          "DEFAULT CHARSET = utf8 COLLATE = utf8_unicode_ci;", true);

            //Elimina Tabla puntoCanje
            //generarActualizacionTablas("puntoCanje", "DROP TABLE `relojControl`.`puntoCanje`;", false);




            //crea la tabla puntoCanje_has_admin
            generarActualizacionTablas("puntoCanje_has_admin", "CREATE TABLE `puntoCanje_has_admin` ( " +
                                      "`idpuntoCanje_has_admin` INT NOT NULL AUTO_INCREMENT, " +
                                      "`puntoCanje_idpuntoCanje` INT UNSIGNED NULL, " +
                                      "`admin_idadmin` INT UNSIGNED NULL, " +
                                      "`activo` INT NULL DEFAULT 0," +
                                      "`created_by` VARCHAR(45) NULL," +
                                      "`updated_by` VARCHAR(45) NULL," +
                                      "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
                                      "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                      "PRIMARY KEY(`idpuntoCanje_has_admin`)" +
                                      ") ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_unicode_ci; ", true);

            //Elimina Tabla puntoCanje
            //generarActualizacionTablas("puntoCanje_has_admin", "DROP TABLE `relojControl`.`puntoCanje_has_admin`;", false);



            //Crea la tabla ticketCasino
            generarActualizacionTablas("ticketCasino", "CREATE TABLE `relojControl`.`ticketCasino` (" +
                                                      "`idticketCasino` INT NOT NULL AUTO_INCREMENT," +
                                                      "`codigoCobroTicket` VARCHAR(45) NULL," +
                                                      "`persona_rut` VARCHAR(200) NULL," +
                                                      "`reloj_idreloj` INT NULL," +
                                                      "`marca_codigoMarca` INT NULL," +
                                                      "`huella_idhuella` INT NULL DEFAULT NULL," +
                                                      "`marcaPin` BIT NULL DEFAULT 0," +
                                                      "`tipoComida_idtipoComida` INT NULL," +
                                                      "`cobrado` BIT NULL DEFAULT 0," +
                                                      "`fechaHoraCobro` DATETIME NULL DEFAULT NULL," +
                                                      "`puntoCanje_idpuntoCanje` int(10) unsigned DEFAULT NULL," +
                                                      "`validadoPor` VARCHAR(45) NULL," +
                                                      "`fechaHoraVencimiento` DATETIME NULL," +
                                                      "`generadoPorReloj` BIT NULL DEFAULT 1," +
                                                      "`checksum` VARCHAR(200) NULL," +
                                                      "`notificado` BIT NULL DEFAULT 0," +
                                                      "`respaldado` BIT NULL DEFAULT 0," +
                                                      "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP," +
                                                      "`updated_at` TIMESTAMP NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP," +
                                                      "PRIMARY KEY(`idticketCasino`)) " +
                                                      "ENGINE = InnoDB " +
                                                      "DEFAULT CHARACTER SET = utf8 COLLATE = utf8_unicode_ci;", true);
            //Elimina Tabla ticketCasino
            //generarActualizacionTablas("ticketCasino", "DROP TABLE `relojControl`.`ticketCasino`;", false);

            //Agrega columna sinRestriccionTicket tabla config_funcionario_reloj
            generarActualizacionColumnas("config_funcionario_reloj", "sinRestriccionTicket", "ALTER TABLE `relojControl`.`config_funcionario_reloj` ADD COLUMN `sinRestriccionTicket` BIT NULL DEFAULT 0 AFTER `tipoRolUsuario_idtipoRolUsuario`;", true);
            //Elimina Tabla columna sinRestriccionTicket
            //generarActualizacionTablas("config_funcionario_reloj", "sinRestriccionTicket", "ALTER TABLE `relojControl`.`config_funcionario_reloj` DROP COLUMN `sinRestriccionTicket`;", false);


            //Agrega columna habilitada tabla tipoComida
            generarActualizacionColumnas("tipoComida", "habilitada", "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `habilitada` BIT NULL DEFAULT 1 AFTER `horaTermino`;", true);
            //Elimina columna habilitada
            //generarActualizacionTablas("tipoComida", "habilitada", "ALTER TABLE `relojControl`.`tipoComida` DROP COLUMN `habilitada`;", false);


            //Crea la tabla perfilCasino
            generarActualizacionTablas("perfilCasino", "CREATE TABLE `relojControl`.`perfilCasino` (" +
                                                        "`idperfilCasino` INT NOT NULL AUTO_INCREMENT," +
                                                        "`nombrePerfilCasino` VARCHAR(60) NULL," +
                                                        "`habilitado` BIT NULL DEFAULT 1," +
                                                        "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP," +
                                                        "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                        "PRIMARY KEY(`idperfilCasino`)) " +
                                                        "ENGINE = InnoDB " +
                                                        "DEFAULT CHARACTER SET = utf8 COLLATE=utf8_unicode_ci;", true);
            //Elimina Tabla perfilCasino
            //generarActualizacionTablas("perfilCasino", "DROP TABLE `relojControl`.`perfilCasino`;", false);


            //Crea la tabla detallePerfilCasino
            generarActualizacionTablas("detallePerfilCasino", "CREATE TABLE `relojControl`.`detallePerfilCasino` (" +
                                                            "`iddetallePerfilCasino` INT NOT NULL AUTO_INCREMENT," +
                                                            "`perfilCasino_idperfilCasino` INT NULL," +
                                                            "`tipoComida_idtipoComida` INT NULL," +
                                                            "`numeroDia` INT NULL," +
                                                            "`habilitado` BIT NULL DEFAULT 0," +
                                                            "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP," +
                                                            "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                            "PRIMARY KEY(`iddetallePerfilCasino`))" +
                                                            "COMMENT = 'numeroDia = 1 Lunes 2 Martes 3 Miercoles 4 Jueves 5 Viernes 6 Sabado 7 Domingo' " +
                                                            "DEFAULT CHARACTER SET = utf8 COLLATE = utf8_unicode_ci;", true);
            //Elimina Tabla detallePerfilCasino
            //generarActualizacionTablas("perfilCasino", "DROP TABLE `relojControl`.`detallePerfilCasino`;", false);


            //Crea la tabla tipoComida_has_tipoInhabilitacionMarca
            generarActualizacionTablas("tipoComida_has_tipoInhabilitacionMarca", "CREATE TABLE `relojControl`.`tipoComida_has_tipoInhabilitacionMarca` ( " +
                                                          "`idtipoComida_has_tipoInhabilitacionMarca` INT NOT NULL AUTO_INCREMENT," +
                                                          "`tipoComida_idtipoComida` INT UNSIGNED NULL," +
                                                          "`tipoInhabilitacionMarca_idtipoInhabilitacionMarca` INT UNSIGNED NULL," +
                                                          "`tipoMarca_idtipoMarca` INT UNSIGNED NULL," +
                                                          "`habilitado` BIT NULL DEFAULT 1," +
                                                          "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP," +
                                                          "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                          "PRIMARY KEY(`idtipoComida_has_tipoInhabilitacionMarca`)" +
                                                          ") ENGINE = InnoDB DEFAULT CHARACTER SET = utf8 COLLATE = utf8_unicode_ci;", true);
            //Elimina Tabla detallePerfilCasino
            //generarActualizacionTablas("tipoComida_has_tipoInhabilitacionMarca", "DROP TABLE `relojControl`.`tipoComida_has_tipoInhabilitacionMarca`;", false);

            //Crea la tabla puntoCanje_has_tipoComida
            generarActualizacionTablas("puntoCanje_has_tipoComida", "CREATE TABLE `relojControl`.`puntoCanje_has_tipoComida` (" +
              "`idpuntoCanje_has_tipoComida` INT NOT NULL AUTO_INCREMENT, " +
              "`puntoCanje_idpuntoCanje` INT NULL," +
              "`tipoComida_idtipoComida` INT NULL," +
              "`activo` BIT NULL DEFAULT 1," +
              "`created_by` VARCHAR(45) NULL DEFAULT ''," +
              "`updated_by` VARCHAR(45) NULL DEFAULT ''," +
              "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
              "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, " +
              "PRIMARY KEY(`idpuntoCanje_has_tipoComida`)) " +
            "ENGINE = InnoDB DEFAULT CHARACTER SET = utf8  COLLATE = utf8_unicode_ci", true);

            //Elimina Tabla puntoCanje_has_tipoComida
            //generarActualizacionTablas("puntoCanje_has_tipoComida", "DROP TABLE `relojControl`.`puntoCanje_has_tipoComida`;", false);




            //Crea la tabla tipoComida_has_valor
            generarActualizacionTablas("tipoComida_has_valor", "CREATE TABLE `relojControl`.`tipoComida_has_valor` (" +
              "`idtipoComida_has_valor` INT NOT NULL AUTO_INCREMENT," +
              "`tipoComida_idtipoComida` INT NULL," +
              "`fechaInicio` DATETIME NULL," +
              "`fechaTermino` DATETIME NULL," +
              "`valorReal` INT NULL," +
              "`valorNominal` INT NULL," +
              "`activo` BIT NULL DEFAULT 1," +
              "`created_by` VARCHAR(45) NULL," +
              "`updated_by` VARCHAR(45) NULL," +
              "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
              "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, " +
              "PRIMARY KEY(`idtipoComida_has_valor`)) " +
             "ENGINE = InnoDB DEFAULT CHARACTER SET = utf8  COLLATE = utf8_unicode_ci", true);

            //Elimina Tabla tipoComida_has_valor
            //generarActualizacionTablas("tipoComida_has_valor", "DROP TABLE `relojControl`.`tipoComida_has_valor`;", false);

            //Elimna la columna valor en tabla tipoComida.
            //ALTER TABLE `relojControl`.`tipoComida` DROP COLUMN `valor`;
            generarActualizacionColumnas("tipoComida", "valor", "ALTER TABLE `relojControl`.`tipoComida` DROP COLUMN `valor`;", false, false);

            //Cambia el nombre de la columna horaInicio de tipoComida
            generarActualizacionColumnas("tipoComida", "horaInicio", "ALTER TABLE `relojControl`.`tipoComida` CHANGE COLUMN `horaInicio` `horaInicioEmision` TIME NOT NULL;", false, true);

            //Cambia el nombre de la columna horaTermino de tipoComida
            generarActualizacionColumnas("tipoComida", "horaTermino", "ALTER TABLE `relojControl`.`tipoComida` CHANGE COLUMN `horaTermino` `horaTerminoEmision` TIME NOT NULL;", false, true);

            //Agrega la columna horaInicioCobro de tipoComida
            generarActualizacionColumnas("tipoComida", "horaInicioCobro", "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `horaInicioCobro` TIME NULL AFTER `horaTerminoEmision`;", true, false);

            //Agrega la columna horaTerminoCobro de tipoComida
            generarActualizacionColumnas("tipoComida", "horaTerminoCobro", "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `horaTerminoCobro` TIME NULL AFTER `horaInicioCobro`;", true, false);

            //Agrega la columna updated_by de tipoComida
            generarActualizacionColumnas("tipoComida", "updated_by", "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `updated_by` VARCHAR(45) NULL DEFAULT '' AFTER `habilitada`;", true, false);

            //Agrega la columna created_by de tipoComida
            generarActualizacionColumnas("tipoComida", "created_by", "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `created_by` VARCHAR(45) NULL DEFAULT '' AFTER `updated_by`;", true, false);

            //Agrega la columna empresa a la tabla correo alerta
            generarActualizacionColumnas("correoAlerta", "empresa_idempresa", "ALTER TABLE `relojControl`.`correoAlerta` ADD COLUMN `empresa_idempresa` INT NULL DEFAULT 1 AFTER `correo`;", true, false);

            //Agrega la tabla categoriaAlerta
            generarActualizacionTablas("categoriaAlerta", "CREATE TABLE `relojControl`.`categoriaAlerta` (" +
                                         " `idcategoriaAlerta` INT NOT NULL AUTO_INCREMENT, " +
                                         " `nombre` VARCHAR(100) NOT NULL DEFAULT ''," +
                                         " `activo` BIT NULL DEFAULT 1," +
                                         " `created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP," +
                                         " `updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, " +
                                         " PRIMARY KEY(`idcategoriaAlerta`));", true);

            //Agrega la tabla tipoReloj
            generarActualizacionTablas("tipoReloj", "CREATE TABLE `relojControl`.`tipoReloj`( " +
                                         "`idtipoReloj` INT NOT NULL AUTO_INCREMENT, " +
                                         "`nombre` VARCHAR(45) NULL, " +
                                         "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
                                         "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, " +
                                         "PRIMARY KEY(`idtipoReloj`)); ", true);


            //Agrega la columna categoriaAlerta_idcategoriaAlerta a la tabla alertaError
            generarActualizacionColumnas("alertaError", "categoriaAlerta_idcategoriaAlerta", "ALTER TABLE `relojControl`.`alertaError` " +
                                        "ADD COLUMN `categoriaAlerta_idcategoriaAlerta` INT NULL DEFAULT NULL AFTER `sucursal_idsucursal`, " +
                                        "ADD INDEX `fk_alertaError_1_idx` (`categoriaAlerta_idcategoriaAlerta` ASC); " +
                                        "ALTER TABLE `relojControl`.`alertaError` ADD CONSTRAINT `fk_categoriaAlerta` " +
                                        "FOREIGN KEY(`categoriaAlerta_idcategoriaAlerta`) " +
                                        "REFERENCES `relojControl`.`categoriaAlerta` (`idcategoriaAlerta`) " +
                                        "ON DELETE NO ACTION ON UPDATE NO ACTION; ", true, false);


            //Agrega la tabla correoAlerta_categoriaAlerta
            generarActualizacionTablas("correoAlerta_categoriaAlerta", "CREATE TABLE `relojControl`.`correoAlerta_categoriaAlerta` ( " +
                                       "`idcorreoAlerta_categoriaAlerta` INT(11) NOT NULL AUTO_INCREMENT, " +
                                       "`idcorreoAlerta` INT(11) DEFAULT NULL, " +
                                       "`idcategoriaAlerta` INT(11) DEFAULT NULL, " +
                                       "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP(), " +
                                       "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP(), " +
                                       "PRIMARY KEY(`idcorreoAlerta_categoriaAlerta`));", true);

            //Agrega la columna tipoReloj_idtipoReloj a la tabla reloj 
            generarActualizacionColumnas("reloj", "tipoReloj_idtipoReloj", "ALTER TABLE `relojControl`.`reloj` ADD COLUMN `tipoReloj_idtipoReloj` INT NULL DEFAULT 1 AFTER `sucursal_idsucursal`; ", true, false);


            //Agrega la columna marcajeLibre a la tabla config_funcionario_reloj
            generarActualizacionColumnas("config_funcionario_reloj", "marcajeLibre", " ALTER TABLE `relojControl`.`config_funcionario_reloj` ADD COLUMN `marcajeLibre` BIT NULL DEFAULT 0 COMMENT 'Se le habilita el marcaje en todos los relojes de asistencia.' AFTER `sinRestriccionTicket`;", true, false);



            //Agrega la tabla version
            generarActualizacionTablas("version", " CREATE TABLE `relojControl`.`version` ( " +
                                        " `idversion` INT NOT NULL AUTO_INCREMENT, " +
                                        " `reloj_idreloj` INT NULL, " +
                                        " `relojControl` VARCHAR(40) NULL, " +
                                        " `sincronizador` VARCHAR(40) NULL, " +
                                        " `sincronizadorCasino` VARCHAR(40) NULL, " +
                                        " `respaldado` BIT NULL DEFAULT 0, " +
                                        " `created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
                                        " `updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, " +
                                        " PRIMARY KEY(`idversion`), " +
                                        " UNIQUE INDEX `idreloj_UNIQUE` (`reloj_idreloj` ASC)); ", true);

            //Agrega la tabla sucursal_tipoComida
            generarActualizacionTablas("sucursal_tipoComida", "CREATE TABLE `relojControl`.`sucursal_tipoComida` ( " +
                                        "`idsucursal_tipoComida` INT NOT NULL AUTO_INCREMENT, " +
                                        "`sucursal_idsucursal` INT NOT NULL, " +
                                        "`tipoComida_idtipoComida` INT NOT NULL, " +
                                        "`horaInicioEmision` TIME NOT NULL, " +
                                        "`horaTerminoEmision` TIME NOT NULL, " +
                                        "`horaInicioCobro` TIME NOT NULL, " +
                                        "`horaTerminoCobro` TIME NOT NULL, " +
                                        "`habilitada` BIT NULL DEFAULT 1, " +
                                        "`created_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP(), " +
                                        "`updated_at` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP(), " +
                                        " PRIMARY KEY(`idsucursal_tipoComida`)); ", true);

            //Agrega la tabla categoriaNotificacion
            generarActualizacionTablas("categoriaNotificacion", "CREATE TABLE `relojControl`.`categoriaNotificacion` (" +
                                       " `idcategoriaNotificacion` int(11) NOT NULL AUTO_INCREMENT, " +
                                       " `categoria` varchar(100) DEFAULT NULL, " +
                                       " `created_at` timestamp NULL DEFAULT current_timestamp(), " +
                                       " `updated_at` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(), " +
                                       " PRIMARY KEY(`idcategoriaNotificacion`)) ", true);

            //Agrega la columna compruebaConexion a la tabla reloj
            generarActualizacionColumnas("reloj", "compruebaConexion",
                "ALTER TABLE `relojControl`.`reloj` ADD COLUMN `compruebaConexion` BIT NULL DEFAULT 1 AFTER `horaTerminoRangoActualizacion`;"
                , true, false);


            //Agrega la columna diasExpiracionTicket a la tabla tipoComida
            generarActualizacionColumnas("tipoComida", "diasExpiracionTicket",
                "ALTER TABLE `relojControl`.`tipoComida` ADD COLUMN `diasExpiracionTicket` INT NULL DEFAULT 30 AFTER `horaTerminoCobro`;"
                , true, false);

            //Agrega la columna diasExpiracionTicket a la tabla sucursal_tipoComida
            generarActualizacionColumnas("sucursal_tipoComida", "diasExpiracionTicket",
                "ALTER TABLE `relojControl`.`sucursal_tipoComida` ADD COLUMN `diasExpiracionTicket` INT NULL DEFAULT 30 AFTER `horaTerminoCobro`;"
                , true, false);
        }

        /// <summary>
        /// Función que verifica y actualiza las columnas de las bases de datos del reloj LOCAL.
        /// </summary>
        /// <param name="tabla"> nombre de la tabla a trabajar</param>
        /// <param name="columna"> columna a procesar</param>
        /// <param name="sqlCreacion">sql a ejecutar para agregar o eliminar alguna columna</param>I
        /// <param name="agregaColumna">Indica si es una creación o eliminación de columnas.</param>
        /// <param name="actualizaColumna">Indica si es una edición de columnas...</param>
        private void generarActualizacionColumnas(string tabla, string columna, string sqlModificacion, bool agregaColumna, bool actualizaColumna = false)
        {
            try
            {
                DataTable dt = comprobarColumnaExistente(tabla, columna);
                if (agregaColumna)
                {
                    //Si agrega y el conteo de columnas es cero ejecutamos la instrucción sql
                    if (dt.Rows.Count == 0)
                    {
                        EjecutoresSql eje = new EjecutoresSql();
                        Herramientas h = new Herramientas();
                        if (eje.EjecutaSQLModificacionTabla(sqlModificacion))
                        {
                            h.generarLogSincronizador("Agregada columna: " + columna + "en tabla: " + tabla + ". Equipo: " + Environment.MachineName, "AVISO");
                        }
                    }
                }
                else
                {
                    //Si el conteo de tablas es mayor que cero ejecutamos la instrucción sql
                    if (dt.Rows.Count > 0)
                    {
                        //Si no agrega columna y actualiza...
                        if (actualizaColumna)
                        {
                            EjecutoresSql eje = new EjecutoresSql();
                            Herramientas h = new Herramientas();
                            if (eje.EjecutaSQLModificacionTabla(sqlModificacion))
                            {
                                h.generarLogSincronizador("Actualizada columna " + columna + " de tabla: " + tabla + ". Equipo: " + Environment.MachineName, "AVISO");
                            }
                        }
                        else //Si no agrega columna y no actualiza.. quiere decir que elimina.
                        {
                            EjecutoresSql eje = new EjecutoresSql();
                            Herramientas h = new Herramientas();
                            if (eje.EjecutaSQLModificacionTabla(sqlModificacion))
                            {
                                h.generarLogSincronizador("Eliminada columna " + columna + " de tabla: " + tabla + ". Equipo: " + Environment.MachineName, "AVISO");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Herramientas h = new Herramientas();
                h.generarLogSincronizador("generarActualizacionColumnas() " + Environment.MachineName + " Tabla:  " + tabla + " columna : " + columna + " " + ex.ToString(), "ERROR");
            }

        }


        private void generarActualizacionTablas(string tabla, string sqlModificacion, bool agregaTabla)
        {
            try
            {
                DataTable dt = comprobarTablaExistente(tabla);
                if (agregaTabla)
                {
                    //Si agrega y el conteo de tablas es cero ejecutamos la instrucción sql
                    if (dt.Rows.Count == 0)
                    {
                        EjecutoresSql eje = new EjecutoresSql();
                        Herramientas h = new Herramientas();
                        if (eje.EjecutaSQLModificacionTabla(sqlModificacion))
                        {
                            h.generarLogSincronizador("Agregada tabla: " + tabla + ". Equipo: " + Environment.MachineName, "AVISO");
                        }
                    }
                }
                else
                {
                    //Si el conteo de tablas es mayor que cero ejecutamos la instrucción sql
                    if (dt.Rows.Count > 0)
                    {
                        EjecutoresSql eje = new EjecutoresSql();
                        Herramientas h = new Herramientas();
                        if (eje.EjecutaSQLModificacionTabla(sqlModificacion))
                        {
                            h.generarLogSincronizador("Eliminada tabla " + tabla + ". Equipo: " + Environment.MachineName, "AVISO");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Herramientas h = new Herramientas();
                h.generarLogSincronizador("generarActualizacionTablas() " + Environment.MachineName + " Tabla:  " + tabla + " " + ex.ToString(), "ERROR");
            }


        }
    }
}
