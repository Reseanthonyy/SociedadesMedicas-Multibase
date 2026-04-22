using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class PersonalSanitarioRepository
{

    /* ===============================
       OBTENER PERSONAL SANITARIO
       ===============================*/

    public async Task<List<PersonalSanitario>> ObtenerPersonalSanitarioAsync(string origen)
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

    private async Task<List<PersonalSanitario>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<PersonalSanitario>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            ps.codigo_empleado,
                            p.nombre,
                            ps.funcion,
                            ps.anios_experiencia
                         FROM personales_sanitarios ps
                         INNER JOIN personales p
                             ON ps.codigo_empleado = p.codigo_empleado";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new PersonalSanitario
            {
                OrigenDatos = proveedor,
                CodigoEmpleado = reader.GetInt32(0),
                NombreEmpleado = reader.GetString(1),
                Funcion = reader.GetString(2),
                AniosExperiencia = reader.GetInt32(3)
            });
        }

        return lista;
    }

    private async Task<List<PersonalSanitario>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<PersonalSanitario>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            ps.codigo_empleado,
                            p.nombre,
                            ps.funcion,
                            ps.anios_experiencia
                         FROM personales_sanitarios ps
                         INNER JOIN personales p
                             ON ps.codigo_empleado = p.codigo_empleado";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new PersonalSanitario
            {
                OrigenDatos = "Aldana",
                CodigoEmpleado = reader.GetInt32(0),
                NombreEmpleado = reader.GetString(1),
                Funcion = reader.GetString(2),
                AniosExperiencia = reader.GetInt32(3)
            });
        }

        return lista;
    }

    private async Task<List<PersonalSanitario>> ObtenerConsolidadoAsync()
    {
        var lista = new List<PersonalSanitario>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR PERSONAL SANITARIO
       ===============================*/

    public async Task InsertarPersonalSanitarioAsync(PersonalSanitario personal, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await InsertarSqlServerAsync(personal, origen);
        }
        else if (origen == "Aldana")
        {
            await InsertarMySqlAsync(personal);
        }
    }

    private async Task InsertarSqlServerAsync(PersonalSanitario personal, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO personales_sanitarios
                        (codigo_empleado,
                         funcion,
                         anios_experiencia)
                         VALUES
                        (@codigo,
                         @funcion,
                         @anios)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@funcion", personal.Funcion);
        cmd.Parameters.AddWithValue("@anios", personal.AniosExperiencia);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(PersonalSanitario personal)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO personales_sanitarios
                        (codigo_empleado,
                         funcion,
                         anios_experiencia)
                         VALUES
                        (@codigo,
                         @funcion,
                         @anios)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@funcion", personal.Funcion);
        cmd.Parameters.AddWithValue("@anios", personal.AniosExperiencia);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR PERSONAL SANITARIO
       ===============================*/

    public async Task ActualizarPersonalSanitarioAsync(PersonalSanitario personal, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await ActualizarSqlServerAsync(personal, origen);
        }
        else if (origen == "Aldana")
        {
            await ActualizarMySqlAsync(personal);
        }
    }

    private async Task ActualizarSqlServerAsync(PersonalSanitario personal, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE personales_sanitarios
                         SET funcion = @funcion,
                             anios_experiencia = @anios
                         WHERE codigo_empleado = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@funcion", personal.Funcion);
        cmd.Parameters.AddWithValue("@anios", personal.AniosExperiencia);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(PersonalSanitario personal)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE personales_sanitarios
                         SET funcion = @funcion,
                             anios_experiencia = @anios
                         WHERE codigo_empleado = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@funcion", personal.Funcion);
        cmd.Parameters.AddWithValue("@anios", personal.AniosExperiencia);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR PERSONAL SANITARIO
       ===============================*/

    public async Task EliminarPersonalSanitarioAsync(int codigo, string origen)
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

        string query = @"DELETE FROM personales_sanitarios
                         WHERE codigo_empleado = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM personales_sanitarios
                         WHERE codigo_empleado = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}