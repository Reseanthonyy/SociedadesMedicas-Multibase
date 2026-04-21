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

public partial class ConsultasView : UserControl
{
    private ConsultaRepository _consultaRepository = new();
    private PersonalRepository _personalRepository = new();
    private CentroRepository _centroRepository = new();
    private EspecialidadRepository _especialidadRepository = new();
    
    private Consulta? _seleccionada = null;
    
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
            
            cmbCentros.ItemsSource = await _centroRepository.ObtenerCentrosAsync(nodoActual);
            cmbEmpleados.ItemsSource = await _personalRepository.ObtenerPersonalAsync(nodoActual);

            cmbEspecialidades.ItemsSource = null;
            cmbEspecialidades.ItemsSource = await _especialidadRepository.ObtenerEspecialidadesAsync(nodoActual);
            
            Console.WriteLine("Se muestran");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
    private Consulta ObtenerConsulta()
    {
        string nodoActual = ConfigGlobal.NodoSeleccionado;

        var fecha = dpFecha.SelectedDate.Value.DateTime.Date;
        var hora = tpHora.SelectedTime.Value;

        var fechaHora = fecha.Add(hora);

        return new Consulta
        {
            CodigoConsulta = _seleccionada?.CodigoConsulta ?? 0,
            CodigoCentro = (cmbCentros.SelectedItem as Centro)?.CodigoCentro ?? 0,
            CodigoEmpleado = (cmbEmpleados.SelectedItem as Personal)?.CodigoEmpleado ?? 0,
            CodigoEspecialidad = (cmbEspecialidades.SelectedItem as Especialidad)?.CodigoEspecialidad ?? 0,

            DiaSemana = fechaHora,
            Hora = hora, // IMPORTANTE
            OrigenDatos = nodoActual
        };
    }

    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridConsultas.SelectedItem is Consulta consulta)
            {
                var origen = ConfigGlobal.NodoSeleccionado;

                await _consultaRepository.EliminarConsultaAsync(consulta.CodigoConsulta, origen);

                await CargarDatos();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var consulta = ObtenerConsulta();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionada == null)
            {
                await _consultaRepository.InsertarConsultaAsync(consulta, origen);
            }
            else
            {
                await _consultaRepository.ActualizarConsultaAsync(consulta, origen);
                _seleccionada = null;
            }

            LimpiarFormulario();
            await CargarDatos();
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
            if (GridConsultas.SelectedItem is Consulta consulta)
            {
                _seleccionada = consulta;

                // Selección en combos (requiere que coincidan objetos)
                cmbCentros.SelectedItem = cmbCentros.Items
                    .OfType<Centro>()
                    .FirstOrDefault(c => c.CodigoCentro == consulta.CodigoCentro);
                
                cmbEmpleados.SelectedItem = cmbEmpleados.Items
                    .OfType<Personal>()
                    .FirstOrDefault(p => p.CodigoEmpleado == consulta.CodigoEmpleado);
                
                cmbEspecialidades.SelectedItem = cmbEspecialidades.Items
                    .OfType<Especialidad>()
                    .FirstOrDefault(e => e.CodigoEspecialidad == consulta.CodigoEspecialidad);

                if (consulta.DiaSemana != null)
                {
                    var fecha = consulta.DiaSemana;

                    dpFecha.SelectedDate = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Local);
                    tpHora.SelectedTime = consulta.Hora;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    private void LimpiarFormulario()
    {
        cmbCentros.SelectedItem = null;
        cmbEmpleados.SelectedItem = null;
        cmbEspecialidades.SelectedItem = null;

        dpFecha.SelectedDate = null;
        tpHora.SelectedTime = null;

        _seleccionada = null;
    }

    private void BtnLimpiar_OnClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }
}