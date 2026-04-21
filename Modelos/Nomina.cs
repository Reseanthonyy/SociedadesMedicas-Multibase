using System;

namespace MedicasMultibase.Modelos;

public class Nomina
{
    public string OrigenDatos { get; set; } = string.Empty;
    public int CodigoNomina { get; set; }
    public int CodigoCentro { get; set; }
    public int CodigoEmpleado { get; set; }
    public int CodigoEspecialidad { get; set; }
    public decimal Salario { get; set; }
    public DateTime FechaConsulta { get; set; }
    public TimeSpan Hora { get; set; }
}