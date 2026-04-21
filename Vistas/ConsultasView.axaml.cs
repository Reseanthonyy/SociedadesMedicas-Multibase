using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class ConsultasView : UserControl
{
    private ConsultaRepository _consultaRepository = new();
    public ConsultasView()
    {
        InitializeComponent();
        _ = CargarDatos();
        ConfigGlobal.NodoCambioEvent +=  async () =>
            await CargarDatos();
    }

    private async Task CargarDatos()
    {
        try
        {
            string nodoActual = ConfigGlobal.NodoSeleccionado;
            GridConsultas.ItemsSource = null;
            GridConsultas.ItemsSource = await _consultaRepository.ObtenerConsultasAsync(nodoActual);
            Console.WriteLine("Se muestran");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}