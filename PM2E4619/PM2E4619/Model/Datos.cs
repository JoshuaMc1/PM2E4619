using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E4619.Model
{
    public class Datos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string latitud { get; set; }
        public string longitud { get; set; }
        public string descripcion { get; set; }
        public string image { get; set; }
    }
}
