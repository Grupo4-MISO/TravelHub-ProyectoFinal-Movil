namespace App.Resources.Styles;

/// <summary>
/// Clase estática que contiene todos los iconos de Material Symbols como constantes.
/// Usar con x:Static en XAML o directamente en C#.
/// </summary>
public static class Icons
{
    // Navegación y Acciones Generales
    public const string Buscar = "\ue8b6";
    public const string Reservas = "\ue7ef";
    public const string Usuario = "\ue7fd";
    public const string Cuenta = "\ue853";
    public const string Home = "\ue88a";
    public const string Atras = "\ue5c4";
    public const string Cerrar = "\ue5cd";
    public const string Menu = "\ue5d2";
    public const string Mas = "\ue145";
    public const string Menos = "\ue15b";
    
    // Alertas y Estados
    public const string Alerta = "\ue002";
    public const string Exito = "\ue86c";
    public const string Error = "\ue000";
    public const string Info = "\ue88e";
    public const string Advertencia = "\ue002";
    
    // Acciones CRUD
    public const string Guardar = "\ue161";
    public const string Eliminar = "\ue872";
    public const string Editar = "\ue3c9";
    public const string Ver = "\ue417";
    public const string Descargar = "\uf090";
    public const string Compartir = "\ue80d";
    
    // Navegación y Ubicación
    public const string Mapa = "\ue55b";
    public const string Ubicacion = "\ue0c8";
    public const string Direcciones = "\ue1e5";
    
    // Fechas y Tiempo
    public const string Calendario = "\ue935";
    public const string Reloj = "\ue192";
    public const string Fecha = "\ue916";
    
    // Contacto y Comunicación
    public const string Telefono = "\ue0cd";
    public const string Email = "\ue0be";
    public const string Mensaje = "\ue0c9";
    public const string Llamar = "\ue0b0";
    
    // Hospedaje y Hotel
    public const string Hotel = "\ue549";
    public const string Habitacion = "\ue8a4";
    public const string Cama = "\uef48";
    public const string Llave = "\ue73c";
    
    // Amenidades Generales
    public const string WiFi = "\ue63e";
    public const string Desayuno = "\uea64";
    public const string Estacionamiento = "\ue54f";
    public const string Piscina = "\ueb48";
    public const string Gimnasio = "\ueb43";
    public const string RoomService = "\ueb49";
    public const string Spa = "\ueb4c";
    public const string Restaurante = "\ue56c";
    public const string Bar = "\ue540";
    public const string Terraza = "\ue94d";
    public const string Chimenea = "\ue1ca";
    public const string Jardin = "\uea3e";
    
    // Amenidades Especiales
    public const string Playa = "\ueb3e";
    public const string PlayaPrivada = "\ueb3e";
    public const string TodoIncluido = "\ue8f9";
    public const string KidsClub = "\uf1ae";
    public const string Snorkel = "\ue30a";
    public const string BusinessCenter = "\ue0af";
    public const string TourCafetero = "\ue541";
    public const string Senderismo = "\ue52f";
    
    // Personas y Grupos
    public const string Persona = "\ue7fd";
    public const string Personas = "\ue7fb";
    public const string Familia = "\ue53a";
    public const string Nino = "\ue50b";
    public const string Adulto = "\ue7fd";
    
    // Valoraciones
    public const string Estrella = "\ue838";
    public const string EstrellaVacia = "\ue83a";
    public const string Corazon = "\ue87d";
    public const string CorazonVacio = "\ue87e";
    public const string Favorito = "\ue87d";
    
    // Pago y Dinero
    public const string Pago = "\ue8f7";
    public const string Tarjeta = "\ue870";
    public const string Dinero = "\ue263";
    public const string Precio = "\ue227";
    public const string Factura = "\ue8b0";
    
    // Documentos
    public const string Documento = "\ue873";
    public const string PDF = "\ue415";
    public const string Voucher = "\ue8b0";
    public const string Confirmacion = "\ue86c";
    
    // Configuración
    public const string Configuracion = "\ue8b8";
    public const string Notificaciones = "\ue7f4";
    public const string Idioma = "\ue8e2";
    public const string Ayuda = "\ue887";
    
    // Filtros y Ordenamiento
    public const string Filtro = "\ue152";
    public const string Ordenar = "\ue164";
    public const string Lista = "\ue8ef";
    public const string Cuadricula = "\ue3ec";
    
    // Check-in/Check-out
    public const string CheckIn = "\ue1e5";
    public const string CheckOut = "\ue1e6";
    public const string QR = "\uef6b";
    public const string Camara = "\ue3af";
    
    // Otros
    public const string Imagenes = "\ue3f4";
    public const string Galeria = "\ue413";
    public const string Flecha = "\ue5c8";
    public const string Check = "\ue5ca";
    public const string Cancelar = "\ue14c";

    /// <summary>
    /// Obtiene el glyph del icono por nombre de clave (para compatibilidad con el sistema anterior).
    /// Usado principalmente por el converter.
    /// </summary>
    public static string GetGlyphByKey(string key)
    {
        return key switch
        {
            "IconWiFi" => WiFi,
            "IconDesayuno" => Desayuno,
            "IconEstacionamiento" => Estacionamiento,
            "IconPiscina" => Piscina,
            "IconGimnasio" => Gimnasio,
            "IconRoomService" => RoomService,
            "IconSpa" => Spa,
            "IconRestaurante" => Restaurante,
            "IconPlayaPrivada" => PlayaPrivada,
            "IconBar" => Bar,
            "IconTerraza" => Terraza,
            "IconTodoIncluido" => TodoIncluido,
            "IconKidsClub" => KidsClub,
            "IconPlaya" => Playa,
            "IconSnorkel" => Snorkel,
            "IconBusinessCenter" => BusinessCenter,
            "IconChimenea" => Chimenea,
            "IconJardin" => Jardin,
            "IconTourCafetero" => TourCafetero,
            "IconSenderismo" => Senderismo,
            _ => Info // Icono por defecto
        };
    }
}