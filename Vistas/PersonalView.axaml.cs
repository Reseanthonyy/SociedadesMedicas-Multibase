using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class PersonalView : UserControl
{
    private PersonalRepository _personalRepository = new();
    public PersonalView()
    {
        InitializeComponent();
        ConfigGlobal.NodoCambioEvent +=  async () =>
            await CargarDatos();
    }
    private async Task CargarDatos()
    {
        try
        {
            string nodoActual = ConfigGlobal.NodoSeleccionado;
            GridPersonal.ItemsSource = null;
            GridPersonal.ItemsSource = await _personalRepository.ObtenerPersonalAsync(nodoActual);
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}