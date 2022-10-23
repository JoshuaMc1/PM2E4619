using PM2E4619.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM2E4619.Controller
{
    public class DatosController
    {
        readonly SQLiteAsyncConnection _conexion;

        public DatosController(String dbPath)
        {
            _conexion = new SQLiteAsyncConnection(dbPath);
            _conexion.CreateTableAsync<Datos>().Wait();
        }

        public Task<List<Datos>> ObtenerDatos()
        {
            return _conexion.Table<Datos>().ToListAsync();
        }

        public Task<int> GuardarDatos(Datos datos)
        {
            if (datos.Id != 0)
            {
                return _conexion.UpdateAsync(datos);
            }
            else
            {
                return _conexion.InsertAsync(datos);
            }
        }

        public Task<Datos> ObtenerDatosId(int id)
        {
            return _conexion.Table<Datos>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> BorrarDatos(Datos datos)
        {
            return _conexion.DeleteAsync(datos);
        }
    }
}
