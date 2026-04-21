using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class EspecialidadRepository
{
    /* ===============================
       OBTENER ESPECIALIDADES
       ===============================*/

    public async Task<List<Especialidad>> ObtenerEspecialidadesAsync(string origen)
    {
        switch (origen)
        {
            case "Anthony":
            case "Cruz":
                return await ObtenerDesdeSqlServerAsync(origen);

            case "Aldana":
                return await ObtenerDesdeMySqlAsync();

            case "Consolidado":
                return await ObtenerConsolidadoAsync();

            default:
                throw new ArgumentException("Origen no válido");
        }
    }

    private async Task<List<Especialidad>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<Especialidad>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_especialidad,
                            nombre_especialidad,
                            descripcion
                         FROM especialidades";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Especialidad
            {
                OrigenDatos = proveedor,
                CodigoEspecialidad = reader.GetInt32(0),
                NombreEspecialidad = reader.GetString(1),
                Descripcion = reader.GetString(2)
            });
        }

        return lista;
    }

    private async Task<List<Especialidad>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<Especialidad>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_especialidad,
                            nombre_especialidad,
                            descripcion
                         FROM especialidades";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Especialidad
            {
                OrigenDatos = "Aldana",
                CodigoEspecialidad = reader.GetInt32(0),
                NombreEspecialidad = reader.GetString(1),
                Descripcion = reader.GetString(2)
            });
        }

        return lista;
    }

    private async Task<List<Especialidad>> ObtenerConsolidadoAsync()
    {
        var lista = new List<Especialidad>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR ESPECIALIDAD
       ===============================*/

    public async Task InsertarEspecialidadAsync(Especialidad especialidad, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await InsertarSqlServerAsync(especialidad, origen);
        }
        else if (origen == "Aldana")
        {
            await InsertarMySqlAsync(especialidad);
        }
    }

    private async Task InsertarSqlServerAsync(Especialidad especialidad, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO especialidades
                        (nombre_especialidad, descripcion)
                         VALUES
                        (@nombre, @descripcion)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", especialidad.NombreEspecialidad);
        cmd.Parameters.AddWithValue("@descripcion", especialidad.Descripcion);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(Especialidad especialidad)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO especialidades
                        (nombre_especialidad, descripcion)
                         VALUES
                        (@nombre, @descripcion)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", especialidad.NombreEspecialidad);
        cmd.Parameters.AddWithValue("@descripcion", especialidad.Descripcion);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR ESPECIALIDAD
       ===============================*/

    public async Task ActualizarEspecialidadAsync(Especialidad especialidad, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await ActualizarSqlServerAsync(especialidad, origen);
        }
        else if (origen == "Aldana")
        {
            await ActualizarMySqlAsync(especialidad);
        }
    }

    private async Task ActualizarSqlServerAsync(Especialidad especialidad, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE especialidades
                         SET nombre_especialidad = @nombre,
                             descripcion = @descripcion
                         WHERE codigo_especialidad = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", especialidad.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@nombre", especialidad.NombreEspecialidad);
        cmd.Parameters.AddWithValue("@descripcion", especialidad.Descripcion);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(Especialidad especialidad)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE especialidades
                         SET nombre_especialidad = @nombre,
                             descripcion = @descripcion
                         WHERE codigo_especialidad = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", especialidad.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@nombre", especialidad.NombreEspecialidad);
        cmd.Parameters.AddWithValue("@descripcion", especialidad.Descripcion);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR ESPECIALIDAD
       ===============================*/

    public async Task EliminarEspecialidadAsync(int codigo, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await EliminarSqlServerAsync(codigo, origen);
        }
        else if (origen == "Aldana")
        {
            await EliminarMySqlAsync(codigo);
        }
    }

    private async Task EliminarSqlServerAsync(int codigo, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"DELETE FROM especialidades
                         WHERE codigo_especialidad = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM especialidades
                         WHERE codigo_especialidad = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}