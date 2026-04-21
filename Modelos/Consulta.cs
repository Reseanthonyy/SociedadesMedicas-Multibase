using System;

namespace MedicasMultibase.Modelos;

public class Consulta
{
    public int CodigoConsulta { get; set; }
    public string OrigenDatos { get; set; } = string.Empty;
    public int CodigoCentro { get; set; }
    public int CodigoEmpleado { get; set; }
    public int CodigoEspecialidad { get; set; }
    public DateTime DiaSemana { get; set; }
    public TimeSpan Hora { get; set; }
}