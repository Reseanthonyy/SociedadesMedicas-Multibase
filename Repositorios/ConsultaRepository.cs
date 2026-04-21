using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicasMultibase.DataContext;
using MedicasMultibase.Modelos;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MedicasMultibase.Repositorios;

public class ConsultaRepository
{
    /* ===============================
       OBTENER CONSULTAS
       ===============================*/

    public async Task<List<Consulta>> ObtenerConsultasAsync(string origen)
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

    private async Task<List<Consulta>> ObtenerDesdeSqlServerAsync(string proveedor)
    {
        var lista = new List<Consulta>();

        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_consulta,
                            codigo_centro,
                            codigo_empleado,
                            codigo_especialidad,
                            dia_semana,
                            hora
                         FROM consultas";

        using var cmd = new SqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Consulta
            {
                OrigenDatos = proveedor,
                CodigoConsulta = reader.GetInt32(0),
                CodigoCentro = reader.GetInt32(1),
                CodigoEmpleado = reader.GetInt32(2),
                CodigoEspecialidad = reader.GetInt32(3),
                DiaSemana = reader.GetDateTime(4),
                Hora = reader.GetTimeSpan(5)
            });
        }

        return lista;
    }

    private async Task<List<Consulta>> ObtenerDesdeMySqlAsync()
    {
        var lista = new List<Consulta>();

        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"SELECT 
                            codigo_consulta,
                            codigo_centro,
                            codigo_empleado,
                            codigo_especialidad,
                            dia_semana,
                            hora
                         FROM consultas";

        using var cmd = new MySqlCommand(query, conexion);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Consulta
            {
                OrigenDatos = "Aldana",
                CodigoConsulta = reader.GetInt32(0),
                CodigoCentro = reader.GetInt32(1),
                CodigoEmpleado = reader.GetInt32(2),
                CodigoEspecialidad = reader.GetInt32(3),
                DiaSemana = reader.GetDateTime(4),
                Hora = reader.IsDBNull(5)
                    ? TimeSpan.Zero
                    : reader.GetTimeSpan(5)
            });
        }

        return lista;
    }

    private async Task<List<Consulta>> ObtenerConsolidadoAsync()
    {
        var lista = new List<Consulta>();

        lista.AddRange(await ObtenerDesdeSqlServerAsync("Anthony"));
        lista.AddRange(await ObtenerDesdeSqlServerAsync("Cruz"));
        lista.AddRange(await ObtenerDesdeMySqlAsync());

        return lista;
    }



    /* ===============================
       INSERTAR CONSULTA
       ===============================*/

    public async Task InsertarConsultaAsync(Consulta consulta, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await InsertarSqlServerAsync(consulta, origen);
        }
        else if (origen == "Aldana")
        {
            await InsertarMySqlAsync(consulta);
        }
    }

    private async Task InsertarSqlServerAsync(Consulta consulta, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"INSERT INTO consultas
                        (codigo_centro,
                         codigo_empleado,
                         codigo_especialidad,
                         dia_semana,
                         hora)
                         VALUES
                        (@centro,
                         @empleado,
                         @especialidad,
                         @dia,
                         @hora)";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@centro", consulta.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", consulta.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", consulta.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@dia", consulta.DiaSemana);
        cmd.Parameters.AddWithValue("@hora", consulta.Hora);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertarMySqlAsync(Consulta consulta)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"INSERT INTO consultas
                        (codigo_centro,
                         codigo_empleado,
                         codigo_especialidad,
                         dia_semana,
                         hora)
                         VALUES
                        (@centro,
                         @empleado,
                         @especialidad,
                         @dia,
                         @hora)";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@centro", consulta.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", consulta.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", consulta.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@dia", consulta.DiaSemana);
        cmd.Parameters.AddWithValue("@hora", consulta.Hora);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ACTUALIZAR CONSULTA
       ===============================*/

    public async Task ActualizarConsultaAsync(Consulta consulta, string origen)
    {
        if (origen == "Anthony" || origen == "Cruz")
        {
            await ActualizarSqlServerAsync(consulta, origen);
        }
        else if (origen == "Aldana")
        {
            await ActualizarMySqlAsync(consulta);
        }
    }

    private async Task ActualizarSqlServerAsync(Consulta consulta, string proveedor)
    {
        using var conexion = ConexionesSqlServer.ObtenerConexion(proveedor);

        await conexion.OpenAsync();

        string query = @"UPDATE consultas
                         SET codigo_centro = @centro,
                             codigo_empleado = @empleado,
                             codigo_especialidad = @especialidad,
                             dia_semana = @dia,
                             hora = @hora
                         WHERE codigo_consulta = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", consulta.CodigoConsulta);
        cmd.Parameters.AddWithValue("@centro", consulta.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", consulta.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", consulta.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@dia", consulta.DiaSemana);
        cmd.Parameters.AddWithValue("@hora", consulta.Hora);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ActualizarMySqlAsync(Consulta consulta)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"UPDATE consultas
                         SET codigo_centro = @centro,
                             codigo_empleado = @empleado,
                             codigo_especialidad = @especialidad,
                             dia_semana = @dia,
                             hora = @hora
                         WHERE codigo_consulta = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", consulta.CodigoConsulta);
        cmd.Parameters.AddWithValue("@centro", consulta.CodigoCentro);
        cmd.Parameters.AddWithValue("@empleado", consulta.CodigoEmpleado);
        cmd.Parameters.AddWithValue("@especialidad", consulta.CodigoEspecialidad);
        cmd.Parameters.AddWithValue("@dia", consulta.DiaSemana);
        cmd.Parameters.AddWithValue("@hora", consulta.Hora);

        await cmd.ExecuteNonQueryAsync();
    }



    /* ===============================
       ELIMINAR CONSULTA
       ===============================*/

    public async Task EliminarConsultaAsync(int codigo, string origen)
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

        string query = @"DELETE FROM consultas
                         WHERE codigo_consulta = @codigo";

        using var cmd = new SqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EliminarMySqlAsync(int codigo)
    {
        using var conexion = ConexionMySql.ObtenerConexion();

        await conexion.OpenAsync();

        string query = @"DELETE FROM consultas
                         WHERE codigo_consulta = @codigo";

        using var cmd = new MySqlCommand(query, conexion);

        cmd.Parameters.AddWithValue("@codigo", codigo);

        await cmd.ExecuteNonQueryAsync();
    }
}