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
        public string CountryCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
