using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class PersonalRepository
{
    /* ===============================
       OBTENER PERSONAL
       ===============================*/

    public async Task<List<Personal>> ObtenerPersonalAsync(string origen)
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

    private async Task<List<Personal>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<Personal>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_empleado,
                            nombre,
                            direccion,
                            salario,
                            irpf,
                            fecha_contrato,
                            codigo_centro
                         FROM personales";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Personal
            {
                OrigenDatos = proveedor,
                CodigoEmpleado = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Direccion = reader.GetString(2),
                Salario = reader.GetDecimal(3),
                Irpf = reader.GetDecimal(4),
                FechaContrato = reader.GetDateTime(5),
                CodigoCentro = reader.GetInt32(6)
            });
        }

        return lista;
    }

    private async Task<List<Personal>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<Personal>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_empleado,
                            nombre,
                            direccion,
                            salario,
                            irpf,
                            fecha_contrato,
                            codigo_centro
                         FROM personales";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Personal
            {
                OrigenDatos = "Aldana",
                CodigoEmpleado = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Direccion = reader.GetString(2),
                Salario = reader.GetDecimal(3),
                Irpf = reader.GetDecimal(4),
                FechaContrato = reader.GetDateTime(5),
                CodigoCentro = reader.GetInt32(6)
            });
        }

        return lista;
    }

    private async Task<List<Personal>> ObtenerConsolidadoAsync()
    {
        var lista = new List<Personal>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR PERSONAL
       ===============================*/

    public async Task InsertarPersonalAsync(Personal personal, string origen)
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

    private async Task InsertarSqlServerAsync(Personal personal, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO personales
                        (nombre,
                         direccion,
                         salario,
                         irpf,
                         fecha_contrato,
                         codigo_centro)
                         VALUES
                        (@nombre,
                         @direccion,
                         @salario,
                         @irpf,
                         @fecha,
                         @centro)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", personal.Nombre);
        cmd.Parameters.AddWithValue("@direccion", personal.Direccion);
        cmd.Parameters.AddWithValue("@salario", personal.Salario);
        cmd.Parameters.AddWithValue("@irpf", personal.Irpf);
        cmd.Parameters.AddWithValue("@fecha", personal.FechaContrato);
        cmd.Parameters.AddWithValue("@centro", personal.CodigoCentro);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(Personal personal)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO personales
                        (nombre,
                         direccion,
                         salario,
                         irpf,
                         fecha_contrato,
                         codigo_centro)
                         VALUES
                        (@nombre,
                         @direccion,
                         @salario,
                         @irpf,
                         @fecha,
                         @centro)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@nombre", personal.Nombre);
        cmd.Parameters.AddWithValue("@direccion", personal.Direccion);
        cmd.Parameters.AddWithValue("@salario", personal.Salario);
        cmd.Parameters.AddWithValue("@irpf", personal.Irpf);
        cmd.Parameters.AddWithValue("@fecha", personal.FechaContrato);
        cmd.Parameters.AddWithValue("@centro", personal.CodigoCentro);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR PERSONAL
       ===============================*/

    public async Task ActualizarPersonalAsync(Personal personal, string origen)
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

    private async Task ActualizarSqlServerAsync(Personal personal, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE personales
                         SET nombre = @nombre,
                             direccion = @direccion,
                             salario = @salario,
                             irpf = @irpf,
                             fecha_contrato = @fecha,
                             codigo_centro = @centro
                         WHERE codigo_empleado = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@nombre", personal.Nombre);
        cmd.Parameters.AddWithValue("@direccion", personal.Direccion);
        cmd.Parameters.AddWithValue("@salario", personal.Salario);
        cmd.Parameters.AddWithValue("@irpf", personal.Irpf);
        cmd.Parameters.AddWithValue("@fecha", personal.FechaContrato);
        cmd.Parameters.AddWithValue("@centro", personal.CodigoCentro);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(Personal personal)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE personales
                         SET nombre = @nombre,
                             direccion = @direccion,
                             salario = @salario,
                             irpf = @irpf,
                             fecha_contrato = @fecha,
                             codigo_centro = @centro
                         WHERE codigo_empleado = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", personal.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@nombre", personal.Nombre);
        cmd.Parameters.AddWithValue("@direccion", personal.Direccion);
        cmd.Parameters.AddWithValue("@salario", personal.Salario);
        cmd.Parameters.AddWithValue("@irpf", personal.Irpf);
        cmd.Parameters.AddWithValue("@fecha", personal.FechaContrato);
        cmd.Parameters.AddWithValue("@centro", personal.CodigoCentro);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR PERSONAL
       ===============================*/

    public async Task EliminarPersonalAsync(int codigo, string origen)
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

        string query = @"DELETE FROM personales
                         WHERE codigo_empleado = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM personales
                         WHERE codigo_empleado = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}