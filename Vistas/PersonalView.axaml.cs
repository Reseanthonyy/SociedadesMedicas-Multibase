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

public partial class PersonalView : UserControl
{
    private PersonalRepository _personalRepository = new();
    private CentroRepository _centroRepository = new();
    
    
    private Personal? _seleccionado = null;
    
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
            
            cmbCentros.ItemsSource = null;
            cmbCentros.ItemsSource = await _centroRepository.ObtenerCentrosAsync(nodoActual);
            
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private Personal ObtenerPersonal()
    {
        string nodoActual = ConfigGlobal.NodoSeleccionado;

        decimal salario = 0;
        decimal irpf = 0;

        decimal.TryParse(txtSalario.Text, out salario);
        decimal.TryParse(txtIrpf.Text, out irpf);

        return new Personal
        {
            CodigoEmpleado = _seleccionado?.CodigoEmpleado ?? 0,
            Nombre = txtNombre.Text?.Trim(),
            Direccion = txtDireccion.Text?.Trim(),
            Salario = salario,
            Irpf = irpf,
            FechaContrato = dpFechaContrato.SelectedDate?.DateTime ?? DateTime.Now,
            CodigoCentro = (cmbCentros.SelectedItem as Centro)?.CodigoCentro ?? 0,
            OrigenDatos = nodoActual
        };
    }
    
    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridPersonal.SelectedItem is Personal per)
            {
                string nodoActual = ConfigGlobal.NodoSeleccionado;

                await _personalRepository.EliminarPersonalAsync(
                    per.CodigoEmpleado,
                    nodoActual);

                await CargarDatos();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void BtnEditar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridPersonal.SelectedItem is Personal per)
            {
                _seleccionado = per;

                txtNombre.Text = per.Nombre;
                txtDireccion.Text = per.Direccion;
                txtSalario.Text = per.Salario.ToString();
                txtIrpf.Text = per.Irpf.ToString();
                dpFechaContrato.SelectedDate = per.FechaContrato;

                // ComboBox (ajustable según tu modelo real)
                cmbCentros.SelectedItem = 
                    cmbCentros.ItemsSource.Cast<Centro>()
                        .FirstOrDefault(c => c.CodigoCentro == per.CodigoCentro);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void BtnLimpiar_OnClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var personal = ObtenerPersonal();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionado == null)
            {
                // INSERTAR
                await _personalRepository.InsertarPersonalAsync(personal, origen);
            }
            else
            {
                // ACTUALIZAR
                await _personalRepository.ActualizarPersonalAsync(personal, origen);
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
        txtNombre.Text = "";
        txtDireccion.Text = "";
        txtSalario.Text = "";
        txtIrpf.Text = "";
        dpFechaContrato.SelectedDate = null;
        cmbCentros.SelectedItem = null;

        _seleccionado = null;
    }
}