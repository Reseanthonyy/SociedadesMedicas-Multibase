using MySqlConnector;

namespace MedicasMultibase.DataContext;

public static class ConexionMySql
{
    private static string _conexionAldana = 
        "Server=10.10.11.92;" +
        "Port=3306;" +
        "Database=sociedades_medicas;" +
        "Uid=usuarios_remotos;" +
        "Pwd=password;";

    public static MySqlConnection ObtenerConexion()
    {
        return new MySqlConnection(_conexionAldana);
    }
}