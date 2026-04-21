using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class PersonalSanitarioView : UserControl
{
    private PersonalSanitarioRepository _personalSanitarioRepository = new();
    public PersonalSanitarioView()
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
            GridPersonalSanitario.ItemsSource = null;
            GridPersonalSanitario.ItemsSource = await _personalSanitarioRepository.ObtenerPersonalSanitarioAsync(nodoActual);
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}