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
    private Centro? _seleccionado = null;
    
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
        string nodoActual = ConfigGlobal.NodoSeleccionado;
        return new Centro
        { 
            CodigoCentro = _seleccionado?.CodigoCentro ?? 0,
            NombreCentro = txtNombreCentro.Text?.Trim(),
            Telefono = long.Parse(txtTelefono.Text),
            Direccion = txtDireccion.Text?.Trim(),
            OrigenDatos = nodoActual // o el valor que uses
        };
    } 
    
    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var centro = ObtenerCentro();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionado == null)
            {
                // INSERTAR
                await _centroRepository.InsertarCentroAsync(centro, origen);
            }
            else
            {
                // ACTUALIZAR
                await _centroRepository.ActualizarCentroAsync(centro, origen);
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
        txtNombreCentro.Text = "";
        txtDireccion.Text = "";
        txtTelefono.Text = "";
        _seleccionado = null;
    }

    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridCentros.SelectedItem is Centro centro)
            {
                var origen = ConfigGlobal.NodoSeleccionado;

                await _centroRepository.EliminarCentroAsync(centro.CodigoCentro, origen);

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
            if (GridCentros.SelectedItem is Centro centro)
            {
                _seleccionado = centro;

                txtNombreCentro.Text = centro.NombreCentro;
                txtDireccion.Text = centro.Direccion;
                txtTelefono.Text = centro.Telefono.ToString();
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
}