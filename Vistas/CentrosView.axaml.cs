using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class CentrosView : UserControl
{
    private CentroRepository _centroRepository = new();
    public CentrosView()
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
            GridCentros.ItemsSource = null;
            GridCentros.ItemsSource = await _centroRepository.ObtenerCentrosAsync(nodoActual);
            Console.WriteLine("Se muestran");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private Centro ObtenerCentro()
    {
        return new Centro
        { 
            NombreCentro = txtNombreCentro.Text?.Trim(),
            Telefono = long.Parse(txtTelefono.Text),
            Direccion = txtDireccion.Text?.Trim(),
            OrigenDatos = "UI" // o el valor que uses
        };
    } 
    
    private void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var centro = ObtenerCentro();
            Console.WriteLine(centro);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}