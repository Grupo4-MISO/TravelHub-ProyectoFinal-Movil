using App.Models;

namespace App.Services;

public class MockDataService
{
    private static List<Amenity>? _cachedAmenities;

    public static List<Property> GetFeaturedProperties()
    {
        var amenities = GetAmenities();

        return
        [
            new Property
            {
                Id = 1,
                Name = "Hotel Las Americas Resort",
                City = "Cartagena",
                Address = "Anillo Vial, Sector Cielo Mar, Zona Norte",
                Description = "Espectacular resort frente al mar Caribe con piscinas, playa privada y gastronomia de primer nivel. Ideal para vacaciones en familia o escapadas romanticas en la heroica.",
                Rating = 4.8,
                ReviewCount = 412,
                PricePerNight = 380000,
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                    "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800",
                    "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio", "Playa Privada"]),
                PropertyType = "Resort",
                Latitude = 10.4103,
                Longitude = -75.5185,
                Rooms = GetRoomsForProperty(1),
                Reviews = GetReviewsForProperty(1)
            },
            new Property
            {
                Id = 2,
                Name = "Hotel Boutique Casa del Arzobispado",
                City = "Bogota",
                Address = "Calle 11 #2-37, La Candelaria",
                Description = "Encantador hotel boutique en el corazon historico de La Candelaria. Casa colonial restaurada con vista a los cerros orientales y todas las comodidades modernas.",
                Rating = 4.5,
                ReviewCount = 238,
                PricePerNight = 220000,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                    "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800",
                    "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Desayuno", "Bar", "Room Service", "Terraza"]),
                PropertyType = "Boutique",
                Latitude = 4.5964,
                Longitude = -74.0733,
                Rooms = GetRoomsForProperty(2),
                Reviews = GetReviewsForProperty(2)
            },
            new Property
            {
                Id = 3,
                Name = "Decameron All Inclusive",
                City = "San Andres",
                Address = "Km 9 Via San Luis, San Andres Isla",
                Description = "Resort todo incluido en la paradisiaca isla de San Andres. Mar de siete colores, deportes acuaticos y entretenimiento para toda la familia.",
                Rating = 4.6,
                ReviewCount = 623,
                PricePerNight = 450000,
                ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                    "https://images.unsplash.com/photo-1445019980597-93fa8acb246c?w=800",
                    "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Todo Incluido", "Piscina", "Kids Club", "Spa", "Playa", "Snorkel"]),
                PropertyType = "Resort",
                Latitude = 12.5567,
                Longitude = -81.7185,
                Rooms = GetRoomsForProperty(3),
                Reviews = GetReviewsForProperty(3)
            },
            new Property
            {
                Id = 4,
                Name = "Hotel Dann Carlton",
                City = "Medellin",
                Address = "Cra. 43A #7-50, El Poblado",
                Description = "Hotel de lujo en el exclusivo sector de El Poblado. Rodeado de restaurantes, centros comerciales y la vibrante vida nocturna de la ciudad de la eterna primavera.",
                Rating = 4.7,
                ReviewCount = 315,
                PricePerNight = 290000,
                ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                    "https://images.unsplash.com/photo-1596394516093-501ba68a0ba6?w=800",
                    "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Restaurante", "Gimnasio", "Business Center"]),
                PropertyType = "Hotel",
                Latitude = 6.2086,
                Longitude = -75.5709,
                Rooms = GetRoomsForProperty(4),
                Reviews = GetReviewsForProperty(4)
            },
            new Property
            {
                Id = 5,
                Name = "Ecohotel La Casona de Leiva",
                City = "Villa de Leyva",
                Address = "Calle 13 #7-20, Centro Historico",
                Description = "Hermoso ecohotel colonial en la plaza principal de Villa de Leyva. Arquitectura tipica boyacense con jardines, chimenea y un ambiente de descanso total.",
                Rating = 4.4,
                ReviewCount = 178,
                PricePerNight = 180000,
                ImageUrl = "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",
                    "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800",
                    "https://images.unsplash.com/photo-1595576508898-0ad5c879a061?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Desayuno", "Chimenea", "Jardin", "Estacionamiento", "Restaurante"]),
                PropertyType = "Ecohotel",
                Latitude = 5.6342,
                Longitude = -73.5243,
                Rooms = GetRoomsForProperty(5),
                Reviews = GetReviewsForProperty(5)
            },
            new Property
            {
                Id = 6,
                Name = "Hotel Intercontinental",
                City = "Cali",
                Address = "Av. Colombia #2-72, Centro",
                Description = "Hotel de negocios y turismo en el corazon de la sultana del Valle. Cerca del Rio Cali, museos y la mejor salsa de Colombia.",
                Rating = 4.3,
                ReviewCount = 267,
                PricePerNight = 240000,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                    "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800",
                    "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Gimnasio", "Restaurante", "Business Center", "Estacionamiento"]),
                PropertyType = "Hotel",
                Latitude = 3.4516,
                Longitude = -76.5320,
                Rooms = GetRoomsForProperty(6),
                Reviews = GetReviewsForProperty(6)
            },
            new Property
            {
                Id = 7,
                Name = "Hotel Mocawa Resort",
                City = "Eje Cafetero",
                Address = "Km 4 Via Cerritos, Pereira, Risaralda",
                Description = "Resort en el corazon del Eje Cafetero rodeado de naturaleza, cultivos de cafe y paisaje cultural cafetero declarado patrimonio de la humanidad.",
                Rating = 4.6,
                ReviewCount = 198,
                PricePerNight = 320000,
                ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                    "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                    "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Spa", "Tour Cafetero", "Restaurante", "Senderismo"]),
                PropertyType = "Resort",
                Latitude = 4.8087,
                Longitude = -75.7348,
                Rooms = GetRoomsForProperty(7),
                Reviews = GetReviewsForProperty(7)
            },
            new Property
            {
                Id = 8,
                Name = "Hotel Almirante",
                City = "Barranquilla",
                Address = "Cra. 54 #75B-75, Alto Prado",
                Description = "Hotel moderno en la puerta de oro de Colombia. Ubicacion privilegiada cerca del malecon del Rio Magdalena y la zona de entretenimiento mas exclusiva.",
                Rating = 4.2,
                ReviewCount = 156,
                PricePerNight = 195000,
                ImageUrl = "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",
                    "https://images.unsplash.com/photo-1596394516093-501ba68a0ba6?w=800",
                    "https://images.unsplash.com/photo-1595576508898-0ad5c879a061?w=800"
                ],
                Amenities = GetAmenitiesByNames(amenities, ["WiFi", "Piscina", "Gimnasio", "Restaurante", "Bar", "Estacionamiento"]),
                PropertyType = "Hotel",
                Latitude = 10.9878,
                Longitude = -74.7889,
                Rooms = GetRoomsForProperty(8),
                Reviews = GetReviewsForProperty(8)
            }
        ];
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

    public static List<string> GetPopularCities()
    {
        return ["Cartagena", "Bogota", "San Andres", "Medellin", "Villa de Leyva", "Cali", "Eje Cafetero", "Barranquilla", "Santa Marta", "Bucaramanga"];
    }

    private static List<Room> GetRoomsForProperty(int propertyId)
    {
        return
        [
            new Room
            {
                Id = propertyId * 100 + 1,
                Name = "Habitacion Estandar",
                Description = "Habitacion comoda con todas las amenidades basicas para una estancia placentera.",
                MaxGuests = 2,
                PricePerNight = 150000 + (propertyId * 15000),
                ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=600",
                Features = ["Cama King", "TV 55\"", "Aire Acondicionado", "Bano Privado"],
                BedType = "King",
                RoomCount = 5
            },
            new Room
            {
                Id = propertyId * 100 + 2,
                Name = "Habitacion Superior",
                Description = "Habitacion amplia con vista parcial y minibar incluido.",
                MaxGuests = 3,
                PricePerNight = 250000 + (propertyId * 15000),
                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=600",
                Features = ["Cama King", "Sofa Cama", "Minibar", "Vista Parcial", "Balcon"],
                BedType = "King + Sofa",
                RoomCount = 3
            },
            new Room
            {
                Id = propertyId * 100 + 3,
                Name = "Suite Premium",
                Description = "Suite de lujo con sala independiente y vistas panoramicas.",
                MaxGuests = 4,
                PricePerNight = 420000 + (propertyId * 15000),
                ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=600",
                Features = ["Cama King", "Sala", "Jacuzzi", "Vista Panoramica", "Minibar", "Room Service"],
                BedType = "King + Sala",
                RoomCount = 2
            }
        ];
    }

    private static List<Review> GetReviewsForProperty(int propertyId)
    {
        return
        [
            new Review
            {
                AuthorName = "Camila Gutierrez",
                Rating = 5,
                Comment = "Excelente experiencia! Las instalaciones son de primera y el servicio impecable. Muy recomendado.",
                Date = DateTime.Now.AddDays(-5)
            },
            new Review
            {
                AuthorName = "Andres Morales",
                Rating = 4,
                Comment = "Muy buen hotel, habitaciones limpias y comodas. La ubicacion es perfecta para recorrer la ciudad.",
                Date = DateTime.Now.AddDays(-12)
            },
            new Review
            {
                AuthorName = "Valentina Ospina",
                Rating = 4.5,
                Comment = "Nos encanto la estancia. El desayuno buffet es espectacular y el personal muy amable. Volveremos!",
                Date = DateTime.Now.AddDays(-20)
            }
        ];
    }

    public static List<Reservation> GetActiveReservations()
    {
        var properties = GetFeaturedProperties();
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
            },
            new Reservation
            {
                Id = 2,
                BookingCode = "TH-2026-0002",
                Property = properties[2],
                Room = properties[2].Rooms[2],
                Traveler = GetSampleTraveler(),
                CheckIn = DateTime.Today.AddDays(30),
                CheckOut = DateTime.Today.AddDays(35),
                Adults = 2,
                Children = 2,
                RoomCount = 1,
                TotalPrice = 2250000,
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
            new Amenity { Id = 18, Name = "Jardin", Icon = "IconJardin" },
            new Amenity { Id = 19, Name = "Tour Cafetero", Icon = "IconTourCafetero" },
            new Amenity { Id = 20, Name = "Senderismo", Icon = "IconSenderismo" }
        ];

        return _cachedAmenities;
    }

    /// <summary>
    /// Método auxiliar para obtener amenidades por nombre
    /// </summary>
    private static List<Amenity> GetAmenitiesByNames(List<Amenity> allAmenities, string[] names)
    {
        return allAmenities
            .Where(a => names.Contains(a.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}