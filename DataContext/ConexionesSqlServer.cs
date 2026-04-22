using System;
using Microsoft.Data.SqlClient;

namespace MedicasMultibase.DataContext;

public static class ConexionesSqlServer
{
    private static string _conexionAnthony ="Server=localhost;" +
                                          "Database=sociedades_medicas;" +
                                          "User Id=sa;" +
                                          "Password=Pandacabrensexcel8#;" +
                                          "TrustServerCertificate=True;";
    
    private static string _conexionCruz = "Server=10.10.23.251;" +
                                            "Database=sociedades_medicas;" +
                                            "User Id=usuarios_remotos;" +
                                            "Password=Contraseña8#;" +
                                            "TrustServerCertificate=True;";

    public static SqlConnection ObtenerConexion(string proveedor)
    {
        switch (proveedor)
        {
            case "Anthony":
                return new SqlConnection(_conexionAnthony);
            case "Cruz":
                return new SqlConnection(_conexionCruz);
            default:
                throw new ArgumentException("Proveedor no válido");
        }
    }
}