using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MedicasMultibase.Modelos;
using MedicasMultibase.Repositorios;

namespace MedicasMultibase.Vistas;

public partial class EspecialidadesView : UserControl
{
    private EspecialidadRepository _especialidadRepository = new();
    private Especialidad? _seleccionado = null;
    public EspecialidadesView()
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
            GridEspecialidades.ItemsSource = null;
            GridEspecialidades.ItemsSource = await _especialidadRepository.ObtenerEspecialidadesAsync(nodoActual);
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private Especialidad ObtenerEspecialidad()
    {
        string nodoActual = ConfigGlobal.NodoSeleccionado;

        return new Especialidad
        {
            CodigoEspecialidad = _seleccionado?.CodigoEspecialidad ?? 0,
            NombreEspecialidad = txtNombreEspecialidad.Text?.Trim(),
            Descripcion = txtDescripcion.Text?.Trim(),
            OrigenDatos = nodoActual
        };
    }


    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var especialidad = ObtenerEspecialidad();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionado == null)
            {
                // INSERTAR
                await _especialidadRepository.InsertarEspecialidadAsync(especialidad, origen);
            }
            else
            {
                // ACTUALIZAR
                await _especialidadRepository.ActualizarEspecialidadAsync(especialidad, origen);
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
        txtNombreEspecialidad.Text = "";
        txtDescripcion.Text = "";
        _seleccionado = null;
    }

    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridEspecialidades.SelectedItem is Especialidad esp)
            {
                string nodoActual = ConfigGlobal.NodoSeleccionado;

                await _especialidadRepository.EliminarEspecialidadAsync(
                    esp.CodigoEspecialidad,
                    nodoActual);

                await CargarDatos();
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

    private void BtnEditar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridEspecialidades.SelectedItem is Especialidad esp)
            {
                _seleccionado = esp;

                txtNombreEspecialidad.Text = esp.NombreEspecialidad;
                txtDescripcion.Text = esp.Descripcion;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}