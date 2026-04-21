using System;

namespace MedicasMultibase.Modelos;

public class ConfigGlobal
{
    public static string NodoSeleccionado { get; set; } = "Consolidado";

    public static Action NodoCambioEvent;
    

}