using System;

namespace MedicasMultibase.Modelos;

public class Personal
{
    public string OrigenDatos { get; set; } = string.Empty;
    public int CodigoEmpleado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public decimal Irpf { get; set; }
    public DateTime FechaContrato { get; set; }
    public int CodigoCentro { get; set; }
}