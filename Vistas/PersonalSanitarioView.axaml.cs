using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class PersonalSanitarioView : UserControl
{
    private PersonalSanitarioRepository _personalSanitarioRepository = new();
    private PersonalRepository _personalRepository = new();
    private PersonalSanitario? _seleccionado = null;
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

            cmbCodigoEmpleado.ItemsSource = null;
            cmbCodigoEmpleado.ItemsSource = await _personalRepository.ObtenerPersonalAsync(nodoActual);
            
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private PersonalSanitario ObtenerPersonalSanitario()
    {
        string nodoActual = ConfigGlobal.NodoSeleccionado;

        int anios = 0;
        int.TryParse(txtAniosExperiencia.Text, out anios);

        return new PersonalSanitario
        {
            CodigoEmpleado = (cmbCodigoEmpleado.SelectedItem as Personal)?.CodigoEmpleado ?? 0,
            Funcion = txtFuncion.Text?.Trim(),
            AniosExperiencia = anios,
            OrigenDatos = nodoActual
        };
    }

    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var personal = ObtenerPersonalSanitario();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionado == null)
            {
                await _personalSanitarioRepository.InsertarPersonalSanitarioAsync(personal, origen);
            }
            else
            {
                personal.CodigoEmpleado = _seleccionado.CodigoEmpleado;

                await _personalSanitarioRepository.ActualizarPersonalSanitarioAsync(personal, origen);
                _seleccionado = null;
            }

            LimpiarFormulario();
            await CargarDatos();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void LimpiarFormulario()
    {
        txtFuncion.Text = "";
        txtAniosExperiencia.Text = "";
        cmbCodigoEmpleado.SelectedItem = null;

        _seleccionado = null;
    }

    private void BtnLimpiar_OnClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private void BtnEditar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridPersonalSanitario.SelectedItem is PersonalSanitario ps)
            {
                _seleccionado = ps;

                txtFuncion.Text = ps.Funcion;
                txtAniosExperiencia.Text = ps.AniosExperiencia.ToString();

                cmbCodigoEmpleado.SelectedItem =
                    cmbCodigoEmpleado.ItemsSource.Cast<Personal>()
                        .FirstOrDefault(e => e.CodigoEmpleado == ps.CodigoEmpleado);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridPersonalSanitario.SelectedItem is PersonalSanitario ps)
            {
                string nodoActual = ConfigGlobal.NodoSeleccionado;

                await _personalSanitarioRepository.EliminarPersonalSanitarioAsync(
                    ps.CodigoEmpleado,
                    nodoActual);

                await CargarDatos();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}