using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class CentroRepository
{
    public async Task<List<Centro>> ObtenerCentrosAsync(string origen)
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

    private async Task<List<Centro>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<Centro>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_centro,
                            nombre_centro,
                            direccion,
                            telefono
                         FROM centros";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Centro
            {
                OrigenDatos = proveedor,
                CodigoCentro = reader.GetInt32(0),
                NombreCentro = reader.GetString(1),
                Direccion = reader.GetString(2),
                Telefono = reader.GetInt64(3)
            });
        }

        return lista;
    }

    private async Task<List<Centro>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<Centro>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_centro,
                            nombre_centro,
                            direccion,
                            telefono
                         FROM centros";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Centro
            {
                OrigenDatos = "Aldana",
                CodigoCentro = reader.GetInt32(0),
                NombreCentro = reader.GetString(1),
                Direccion = reader.GetString(2),
                Telefono = reader.GetInt64(3)
            });
        }

        return lista;
    }

    private async Task<List<Centro>> ObtenerConsolidadoAsync()
    {
        var lista = new List<Centro>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR CENTRO
       ===============================*/

    public async Task InsertarCentroAsync(Centro centro, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await InsertarSqlServerAsync(centro, origen);
        }
        else if (origen == "Aldana")
        {
            await InsertarMySqlAsync(centro);
        }
    }

    private async Task InsertarSqlServerAsync(Centro centro, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO centros
                        (nombre_centro, direccion, telefono)
                         VALUES
                        (@nombre, @direccion, @telefono)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", centro.NombreCentro);
        cmd.Parameters.AddWithValue("@direccion", centro.Direccion);
        cmd.Parameters.AddWithValue("@telefono", centro.Telefono);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(Centro centro)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO centros
                        (nombre_centro, direccion, telefono)
                         VALUES
                        (@nombre, @direccion, @telefono)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", centro.NombreCentro);
        cmd.Parameters.AddWithValue("@direccion", centro.Direccion);
        cmd.Parameters.AddWithValue("@telefono", centro.Telefono);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR CENTRO
       ===============================*/

    public async Task ActualizarCentroAsync(Centro centro, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await ActualizarSqlServerAsync(centro, origen);
        }
        else if (origen == "Aldana")
        {
            await ActualizarMySqlAsync(centro);
        }
    }

    private async Task ActualizarSqlServerAsync(Centro centro, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE centros
                         SET nombre_centro = @nombre,
                             direccion = @direccion,
                             telefono = @telefono
                         WHERE codigo_centro = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", centro.CodigoCentro);
        cmd.Parameters.AddWithValue("@nombre", centro.NombreCentro);
        cmd.Parameters.AddWithValue("@direccion", centro.Direccion);
        cmd.Parameters.AddWithValue("@telefono", centro.Telefono);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(Centro centro)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE centros
                         SET nombre_centro = @nombre,
                             direccion = @direccion,
                             telefono = @telefono
                         WHERE codigo_centro = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", centro.CodigoCentro);
        cmd.Parameters.AddWithValue("@nombre", centro.NombreCentro);
        cmd.Parameters.AddWithValue("@direccion", centro.Direccion);
        cmd.Parameters.AddWithValue("@telefono", centro.Telefono);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR CENTRO
       ===============================*/

    public async Task EliminarCentroAsync(int codigo, string origen)
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

        string query = @"DELETE FROM centros
                         WHERE codigo_centro = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM centros
                         WHERE codigo_centro = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}