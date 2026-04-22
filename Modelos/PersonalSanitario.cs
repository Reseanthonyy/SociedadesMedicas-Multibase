namespace MedicasMultibase.Modelos;

public class PersonalSanitario
{
    public string OrigenDatos { get; set; } = string.Empty;
    public int CodigoEmpleado { get; set; }
    public string Funcion { get; set; } = string.Empty;
    public int AniosExperiencia { get; set; }
    
    //Extra
    public string  NombreEmpleado { get; set; } = string.Empty;
}