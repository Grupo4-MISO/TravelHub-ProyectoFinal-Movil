using App.Models;

namespace App.Services;

public class MockDataService
{
    private static List<Amenity>? _cachedAmenities;
    private static List<Country>? _cachedCountries;
    private static List<Property>? _cachedAllProperties;

    public static List<Country> GetCountries()
    {
        if (_cachedCountries != null)
            return _cachedCountries;

        _cachedCountries =
        [
            new Country 
            { 
                Id = 1, 
                Name = "Colombia", 
                Code = "CO", 
                CurrencyCode = "COP", 
                CurrencySymbol = "$",
                FlagEmoji = "🇨🇴",
                PhoneCode = "+57"
            },
            new Country 
            { 
                Id = 2, 
                Name = "Perú", 
                Code = "PE", 
                CurrencyCode = "PEN", 
                CurrencySymbol = "S/",
                FlagEmoji = "🇵🇪",
                PhoneCode = "+51"
            },
            new Country 
            { 
                Id = 3, 
                Name = "Ecuador", 
                Code = "EC", 
                CurrencyCode = "USD", 
                CurrencySymbol = "$",
                FlagEmoji = "🇪🇨",
                PhoneCode = "+593"
            },
            new Country 
            { 
                Id = 4, 
                Name = "México", 
                Code = "MX", 
                CurrencyCode = "MXN", 
                CurrencySymbol = "$",
                FlagEmoji = "🇲🇽",
                PhoneCode = "+52"
            },
            new Country 
            { 
                Id = 5, 
                Name = "Chile", 
                Code = "CL", 
                CurrencyCode = "CLP", 
                CurrencySymbol = "$",
                FlagEmoji = "🇨🇱",
                PhoneCode = "+56"
            },
            new Country 
            { 
                Id = 6, 
                Name = "Argentina", 
                Code = "AR", 
                CurrencyCode = "ARS", 
                CurrencySymbol = "$",
                FlagEmoji = "🇦🇷",
                PhoneCode = "+54"
            }
        ];

        return _cachedCountries;
    }

    public static Country GetCountryByCode(string code)
    {
        return GetCountries().FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) 
               ?? GetCountries().First();
    }

    public static Country GetCountryById(int id)
    {
        return GetCountries().FirstOrDefault(c => c.Id == id) 
               ?? GetCountries().First();
    }

    public static List<string> GetPopularCitiesByCountry(string countryCode)
    {
        return countryCode.ToUpper() switch
        {
            "CO" => ["Cartagena", "Bogotá", "Medellín", "Cali", "Santa Marta", "San Andrés", "Eje Cafetero", "Villa de Leyva"],
            "PE" => ["Lima", "Cusco", "Arequipa", "Puno", "Trujillo", "Máncora", "Paracas", "Iquitos"],
            "EC" => ["Quito", "Guayaquil", "Cuenca", "Galápagos", "Montañita", "Baños", "Manta", "Otavalo"],
            "MX" => ["Ciudad de México", "Cancún", "Playa del Carmen", "Guadalajara", "Puerto Vallarta", "Oaxaca", "Tulum", "Los Cabos"],
            "CL" => ["Santiago", "Valparaíso", "Viña del Mar", "Puerto Varas", "San Pedro de Atacama", "Punta Arenas", "La Serena", "Concepción"],
            "AR" => ["Buenos Aires", "Mendoza", "Bariloche", "Córdoba", "Salta", "Puerto Madryn", "Ushuaia", "Mar del Plata"],
            _ => []
        };
    }

    public static List<Property> GetAllProperties()
    {
        if (_cachedAllProperties != null)
            return _cachedAllProperties;

        var amenities = GetAmenities();
        var properties = new List<Property>();

        // COLOMBIA
        var colombia = GetCountryByCode("CO");
        properties.AddRange([
            new Property
            {
                Id = 1,
                Name = "Hotel Las Américas Resort",
                City = "Cartagena",
                Address = "Anillo Vial, Sector Cielo Mar, Zona Norte",
                Description = "Espectacular resort frente al mar Caribe con piscinas, playa privada y gastronomía de primer nivel.",
                Rating = 4.8,
                ReviewCount = 412,
                PricePerNight = 380000,
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800", "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio", "Playa Privada"]),
                PropertyType = "Resort",
                Latitude = 10.4103,
                Longitude = -75.5185,
                Country = colombia,
                Rooms = GetRoomsForProperty(1, 380000),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 2,
                Name = "Hotel Boutique Casa del Arzobispado",
                City = "Bogotá",
                Address = "Calle 11 #2-37, La Candelaria",
                Description = "Encantador hotel boutique en el corazón histórico de La Candelaria.",
                Rating = 4.5,
                ReviewCount = 238,
                PricePerNight = 220000,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Desayuno", "Bar", "Room Service"]),
                PropertyType = "Boutique",
                Latitude = 4.5964,
                Longitude = -74.0733,
                Country = colombia,
                Rooms = GetRoomsForProperty(2, 220000),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 3,
                Name = "Hotel Dann Carlton Medellín",
                City = "Medellín",
                Address = "Cra. 43A #7-50, El Poblado",
                Description = "Hotel de lujo en el exclusivo sector de El Poblado.",
                Rating = 4.7,
                ReviewCount = 315,
                PricePerNight = 290000,
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio"]),
                PropertyType = "Hotel",
                Latitude = 6.2086,
                Longitude = -75.5709,
                Country = colombia,
                Rooms = GetRoomsForProperty(3, 290000),
                Reviews = GetReviewsForProperty()
            }
        ]);

        // PERÚ
        var peru = GetCountryByCode("PE");
        properties.AddRange([
            new Property
            {
                Id = 10,
                Name = "Belmond Hotel Monasterio",
                City = "Cusco",
                Address = "Calle Palacio 140, Plazoleta Nazarenas",
                Description = "Hotel histórico en un monasterio del siglo XVI con vistas a los Andes.",
                Rating = 4.9,
                ReviewCount = 856,
                PricePerNight = 450,
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Spa", "Restaurante", "Bar"]),
                PropertyType = "Boutique",
                Latitude = -13.5167,
                Longitude = -71.9789,
                Country = peru,
                Rooms = GetRoomsForProperty(10, 450),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 11,
                Name = "JW Marriott Lima",
                City = "Lima",
                Address = "Malecón de la Reserva 615, Miraflores",
                Description = "Hotel de lujo frente al Océano Pacífico con casino y spa de clase mundial.",
                Rating = 4.7,
                ReviewCount = 623,
                PricePerNight = 320,
                ImageUrl = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio"]),
                PropertyType = "Hotel",
                Latitude = -12.1308,
                Longitude = -77.0261,
                Country = peru,
                Rooms = GetRoomsForProperty(11, 320),
                Reviews = GetReviewsForProperty()
            }
        ]);

        // ECUADOR
        var ecuador = GetCountryByCode("EC");
        properties.AddRange([
            new Property
            {
                Id = 20,
                Name = "Casa Gangotena",
                City = "Quito",
                Address = "Bolivar Oe6-41 y Cuenca, Centro Histórico",
                Description = "Mansión restaurada en el corazón del Centro Histórico de Quito.",
                Rating = 4.8,
                ReviewCount = 412,
                PricePerNight = 180,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Restaurante", "Bar", "Terraza"]),
                PropertyType = "Boutique",
                Latitude = -0.2186,
                Longitude = -78.5097,
                Country = ecuador,
                Rooms = GetRoomsForProperty(20, 180),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 21,
                Name = "Finch Bay Galápagos Hotel",
                City = "Galápagos",
                Address = "Punta Estrada, Puerto Ayora",
                Description = "Único hotel frente a la playa en Puerto Ayora, puerta de entrada a las Islas Galápagos.",
                Rating = 4.9,
                ReviewCount = 567,
                PricePerNight = 280,
                ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Restaurante", "Playa"]),
                PropertyType = "Resort",
                Latitude = -0.7431,
                Longitude = -90.3114,
                Country = ecuador,
                Rooms = GetRoomsForProperty(21, 280),
                Reviews = GetReviewsForProperty()
            }
        ]);

        // MÉXICO
        var mexico = GetCountryByCode("MX");
        properties.AddRange([
            new Property
            {
                Id = 30,
                Name = "Grand Fiesta Americana Coral Beach",
                City = "Cancún",
                Address = "Blvd. Kukulcan Km 9.5, Zona Hotelera",
                Description = "Resort todo incluido con playa privada en el corazón de la Zona Hotelera de Cancún.",
                Rating = 4.8,
                ReviewCount = 1245,
                PricePerNight = 350,
                ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Todo Incluido", "Piscina", "Spa", "Playa Privada"]),
                PropertyType = "Resort",
                Latitude = 21.1355,
                Longitude = -86.7465,
                Country = mexico,
                Rooms = GetRoomsForProperty(30, 350),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 31,
                Name = "Four Seasons México DF",
                City = "Ciudad de México",
                Address = "Paseo de la Reforma 500, Colonia Juárez",
                Description = "Elegante hotel de lujo en el corazón de la Ciudad de México.",
                Rating = 4.9,
                ReviewCount = 892,
                PricePerNight = 280,
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio"]),
                PropertyType = "Hotel",
                Latitude = 19.4266,
                Longitude = -99.1718,
                Country = mexico,
                Rooms = GetRoomsForProperty(31, 280),
                Reviews = GetReviewsForProperty()
            }
        ]);

        // CHILE
        var chile = GetCountryByCode("CL");
        properties.AddRange([
            new Property
            {
                Id = 40,
                Name = "The Singular Santiago",
                City = "Santiago",
                Address = "Merced 294, Lastarria",
                Description = "Hotel boutique de lujo en el histórico barrio Lastarria.",
                Rating = 4.8,
                ReviewCount = 534,
                PricePerNight = 180,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Restaurante", "Bar", "Gimnasio"]),
                PropertyType = "Boutique",
                Latitude = -33.4372,
                Longitude = -70.6506,
                Country = chile,
                Rooms = GetRoomsForProperty(40, 180),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 41,
                Name = "Hotel AWA Puerto Varas",
                City = "Puerto Varas",
                Address = "San Francisco 200",
                Description = "Hotel contemporáneo con vistas al Lago Llanquihue y volcanes.",
                Rating = 4.9,
                ReviewCount = 423,
                PricePerNight = 220,
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Spa", "Restaurante", "Terraza"]),
                PropertyType = "Hotel",
                Latitude = -41.3196,
                Longitude = -72.9833,
                Country = chile,
                Rooms = GetRoomsForProperty(41, 220),
                Reviews = GetReviewsForProperty()
            }
        ]);

        // ARGENTINA
        var argentina = GetCountryByCode("AR");
        properties.AddRange([
            new Property
            {
                Id = 50,
                Name = "Alvear Palace Hotel",
                City = "Buenos Aires",
                Address = "Av. Alvear 1891, Recoleta",
                Description = "Icónico hotel de lujo en el elegante barrio de Recoleta.",
                Rating = 4.9,
                ReviewCount = 1234,
                PricePerNight = 320,
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio"]),
                PropertyType = "Hotel",
                Latitude = -34.5887,
                Longitude = -58.3896,
                Country = argentina,
                Rooms = GetRoomsForProperty(50, 320),
                Reviews = GetReviewsForProperty()
            },
            new Property
            {
                Id = 51,
                Name = "Llao Llao Hotel & Resort",
                City = "Bariloche",
                Address = "Av. Ezequiel Bustillo Km 25",
                Description = "Resort de montaña con vistas espectaculares a los lagos y montañas patagónicas.",
                Rating = 4.8,
                ReviewCount = 876,
                PricePerNight = 380,
                ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                ImageUrls = ["https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800"],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Senderismo"]),
                PropertyType = "Resort",
                Latitude = -41.0483,
                Longitude = -71.5376,
                Country = argentina,
                Rooms = GetRoomsForProperty(51, 380),
                Reviews = GetReviewsForProperty()
            }
        ]);

        _cachedAllProperties = properties;
        return _cachedAllProperties;
    }

    public static List<Property> GetFeaturedProperties(string? countryCode = null)
    {
        var allProperties = GetAllProperties();
        
        if (string.IsNullOrEmpty(countryCode))
            return allProperties.Take(8).ToList();

        return allProperties
            .Where(p => p.Country.Code.Equals(countryCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<string> GetPromotionalImages()
    {
        return
        [
            "https://www.centraldevacaciones.com/blog/wp-content/uploads/2015/07/EspecialWamos2FB.jpg",
            "https://www.viajesexito.com/wp-content/uploads/2025/07/banner-principal-mobile-2.webp",
            "https://www.latamairlines.com/content/dam/latamxp/sites/sh-ofertas/paquetes-promov3.png"
        ];
    }

    private static List<Room> GetRoomsForProperty(int propertyId, decimal basePrice)
    {
        return
        [
            new Room
            {
                Id = propertyId * 100 + 1,
                Name = "Habitación Estándar",
                Description = "Habitación cómoda con todas las amenidades básicas.",
                MaxGuests = 2,
                PricePerNight = basePrice * 0.6m,
                ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=600",
                Features = ["Cama King", "TV 55\"", "Aire Acondicionado", "Baño Privado"],
                BedType = "King",
                RoomCount = 5
            },
            new Room
            {
                Id = propertyId * 100 + 2,
                Name = "Habitación Superior",
                Description = "Habitación amplia con vista parcial.",
                MaxGuests = 3,
                PricePerNight = basePrice,
                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=600",
                Features = ["Cama King", "Sofá Cama", "Minibar", "Vista Parcial"],
                BedType = "King + Sofá",
                RoomCount = 3
            },
            new Room
            {
                Id = propertyId * 100 + 3,
                Name = "Suite Premium",
                Description = "Suite de lujo con vistas panorámicas.",
                MaxGuests = 4,
                PricePerNight = basePrice * 1.5m,
                ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=600",
                Features = ["Cama King", "Sala", "Jacuzzi", "Vista Panorámica"],
                BedType = "King + Sala",
                RoomCount = 2
            }
        ];
    }

    private static List<Review> GetReviewsForProperty()
    {
        return
        [
            new Review
            {
                AuthorName = "María González",
                Rating = 5,
                Comment = "Excelente experiencia! Instalaciones de primera.",
                Date = DateTime.Now.AddDays(-5)
            },
            new Review
            {
                AuthorName = "Carlos Rodríguez",
                Rating = 4,
                Comment = "Muy buen hotel, ubicación perfecta.",
                Date = DateTime.Now.AddDays(-12)
            }
        ];
    }

    public static List<Reservation> GetActiveReservations()
    {
        var properties = GetFeaturedProperties("CO");
        return
        [
            new Reservation
            {
                Id = 1,
                BookingCode = "TH-2026-0001",
                Property = properties[0],
                Room = properties[0].Rooms[1],
                Traveler = GetSampleTraveler(),
                CheckIn = DateTime.Today.AddDays(15),
                CheckOut = DateTime.Today.AddDays(19),
                Adults = 2,
                Children = 1,
                RoomCount = 1,
                TotalPrice = 1000000,
                Status = "Confirmada"
            }
        ];
    }

    public static Traveler GetSampleTraveler()
    {
        return new Traveler
        {
            Id = 1,
            Photo = "https://images.imagenmia.com/model_version/bbfea91410ef7994cfefde4a33e032f3aebf7b90dda683f7fa32ea2685d2e7bb/1723819204347-output.jpg",
            FirstName = "Juan",
            LastName = "Rodriguez",
            Email = "j.rodriguez@email.com",
            Phone = "+57 321557899",
            DocumentType = "CC",
            DocumentNumber = "1090388345"
        };
    }

    public static List<Amenity> GetAmenities()
    {
        if (_cachedAmenities != null)
            return _cachedAmenities;

        _cachedAmenities =
        [
            new Amenity { Id = 1, Name = "WiFi", Icon = "IconWiFi" },
            new Amenity { Id = 2, Name = "Desayuno", Icon = "IconDesayuno" },
            new Amenity { Id = 3, Name = "Estacionamiento", Icon = "IconEstacionamiento" },
            new Amenity { Id = 4, Name = "Piscina", Icon = "IconPiscina" },
            new Amenity { Id = 5, Name = "Gimnasio", Icon = "IconGimnasio" },
            new Amenity { Id = 6, Name = "Room Service", Icon = "IconRoomService" },
            new Amenity { Id = 7, Name = "Spa", Icon = "IconSpa" },
            new Amenity { Id = 8, Name = "Restaurante", Icon = "IconRestaurante" },
            new Amenity { Id = 9, Name = "Playa Privada", Icon = "IconPlayaPrivada" },
            new Amenity { Id = 10, Name = "Bar", Icon = "IconBar" },
            new Amenity { Id = 11, Name = "Terraza", Icon = "IconTerraza" },
            new Amenity { Id = 12, Name = "Todo Incluido", Icon = "IconTodoIncluido" },
            new Amenity { Id = 13, Name = "Kids Club", Icon = "IconKidsClub" },
            new Amenity { Id = 14, Name = "Playa", Icon = "IconPlaya" },
            new Amenity { Id = 15, Name = "Snorkel", Icon = "IconSnorkel" },
            new Amenity { Id = 16, Name = "Business Center", Icon = "IconBusinessCenter" },
            new Amenity { Id = 17, Name = "Chimenea", Icon = "IconChimenea" },
            new Amenity { Id = 18, Name = "Jardín", Icon = "IconJardin" },
            new Amenity { Id = 19, Name = "Tour Cafetero", Icon = "IconTourCafetero" },
            new Amenity { Id = 20, Name = "Senderismo", Icon = "IconSenderismo" }
        ];

        return _cachedAmenities;
    }

    private static List<Amenity> GetAmenitiesByNames(List<Amenity> allAmenities, string[] names)
    {
        return allAmenities
            .Where(a => names.Contains(a.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}