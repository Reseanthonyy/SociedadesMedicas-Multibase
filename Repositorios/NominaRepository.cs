using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class NominaRepository
{
    /* ===============================
       OBTENER NOMINAS
       ===============================*/

    public async Task<List<Nomina>> ObtenerNominasAsync(string origen)
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

    private async Task<List<Nomina>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<Nomina>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_nomina,
                            codigo_centro,
                            codigo_empleado,
                            codigo_especialidad,
                            salario,
                            fecha_consulta,
                            hora
                         FROM nominas";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Nomina
            {
                OrigenDatos = proveedor,
                CodigoNomina = reader.GetInt32(0),
                CodigoCentro = reader.GetInt32(1),
                CodigoEmpleado = reader.GetInt32(2),
                CodigoEspecialidad = reader.GetInt32(3),
                Salario = reader.GetDecimal(4),
                FechaConsulta = reader.GetDateTime(5),
                Hora = reader.GetTimeSpan(6)
            });
        }

        return lista;
    }

    private async Task<List<Nomina>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<Nomina>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_nomina,
                            codigo_centro,
                            codigo_empleado,
                            codigo_especialidad,
                            salario,
                            fecha_consulta,
                            hora
                         FROM nominas";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Nomina
            {
                OrigenDatos = "Aldana",
                CodigoNomina = reader.GetInt32(0),
                CodigoCentro = reader.GetInt32(1),
                CodigoEmpleado = reader.GetInt32(2),
                CodigoEspecialidad = reader.GetInt32(3),
                Salario = reader.GetDecimal(4),
                FechaConsulta = reader.GetDateTime(5),
                Hora = reader.IsDBNull(6)
                    ? TimeSpan.Zero
                    : reader.GetTimeSpan(6)
            });
        }

        return lista;
    }

    private async Task<List<Nomina>> ObtenerConsolidadoAsync()
    {
        var lista = new List<Nomina>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR NOMINA
       ===============================*/

    public async Task InsertarNominaAsync(Nomina nomina, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await InsertarSqlServerAsync(nomina, origen);
        }
        else if (origen == "Aldana")
        {
            await InsertarMySqlAsync(nomina);
        }
    }

    private async Task InsertarSqlServerAsync(Nomina nomina, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO nominas
                        (codigo_centro,
                         codigo_empleado,
                         codigo_especialidad,
                         salario,
                         fecha_consulta,
                         hora)
                         VALUES
                        (@centro,
                         @empleado,
                         @especialidad,
                         @salario,
                         @fecha,
                         @hora)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@centro", nomina.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", nomina.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", nomina.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@salario", nomina.Salario);
        cmd.Parameters.AddWithValue("@fecha", nomina.FechaConsulta);
        cmd.Parameters.AddWithValue("@hora", nomina.Hora);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(Nomina nomina)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO nominas
                        (codigo_centro,
                         codigo_empleado,
                         codigo_especialidad,
                         salario,
                         fecha_consulta,
                         hora)
                         VALUES
                        (@centro,
                         @empleado,
                         @especialidad,
                         @salario,
                         @fecha,
                         @hora)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@centro", nomina.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", nomina.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", nomina.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@salario", nomina.Salario);
        cmd.Parameters.AddWithValue("@fecha", nomina.FechaConsulta);
        cmd.Parameters.AddWithValue("@hora", nomina.Hora);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR NOMINA
       ===============================*/

    public async Task ActualizarNominaAsync(Nomina nomina, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await ActualizarSqlServerAsync(nomina, origen);
        }
        else if (origen == "Aldana")
        {
            await ActualizarMySqlAsync(nomina);
        }
    }

    private async Task ActualizarSqlServerAsync(Nomina nomina, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE nominas
                         SET codigo_centro = @centro,
                             codigo_empleado = @empleado,
                             codigo_especialidad = @especialidad,
                             salario = @salario,
                             fecha_consulta = @fecha,
                             hora = @hora
                         WHERE codigo_nomina = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", nomina.CodigoNomina);
        cmd.Parameters.AddWithValue("@centro", nomina.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", nomina.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", nomina.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@salario", nomina.Salario);
        cmd.Parameters.AddWithValue("@fecha", nomina.FechaConsulta);
        cmd.Parameters.AddWithValue("@hora", nomina.Hora);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(Nomina nomina)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE nominas
                         SET codigo_centro = @centro,
                             codigo_empleado = @empleado,
                             codigo_especialidad = @especialidad,
                             salario = @salario,
                             fecha_consulta = @fecha,
                             hora = @hora
                         WHERE codigo_nomina = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", nomina.CodigoNomina);
        cmd.Parameters.AddWithValue("@centro", nomina.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", nomina.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", nomina.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@salario", nomina.Salario);
        cmd.Parameters.AddWithValue("@fecha", nomina.FechaConsulta);
        cmd.Parameters.AddWithValue("@hora", nomina.Hora);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR NOMINA
       ===============================*/

    public async Task EliminarNominaAsync(int codigo, string origen)
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

        string query = @"DELETE FROM nominas
                         WHERE codigo_nomina = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM nominas
                         WHERE codigo_nomina = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}