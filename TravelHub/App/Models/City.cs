using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Models
{
    public class City
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CountryCode { get; set; } // CO, PE, EC, MX, CL, AR
        public string Name { get; set; } // Bogotá, Lima, Quito, Ciudad de México, Santiago, Buenos Aires
    }
}
