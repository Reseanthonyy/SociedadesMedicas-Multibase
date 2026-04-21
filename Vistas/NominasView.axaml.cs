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

public partial class NominasView : UserControl
{
    private NominaRepository _nominaRepository = new();
    private PersonalRepository _personalRepository = new();
    private EspecialidadRepository _especialidadRepository = new();
    private CentroRepository  _centroRepository = new();
    
    private Nomina? _seleccionada = null;
    
    public NominasView()
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
            GridNominas.ItemsSource = null;
            GridNominas.ItemsSource = await _nominaRepository.ObtenerNominasAsync(nodoActual);
            
            cmbCentros.ItemsSource = await _centroRepository.ObtenerCentrosAsync(nodoActual);
            cmbEmpleados.ItemsSource = await _personalRepository.ObtenerPersonalAsync(nodoActual);
            cmbEspecialidades.ItemsSource = await _especialidadRepository.ObtenerEspecialidadesAsync(nodoActual);
            
            Console.WriteLine("Se muestran");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private Nomina ObtenerNomina()
    {
        string nodoActual = ConfigGlobal.NodoSeleccionado;

        return new Nomina
        {
            CodigoNomina = _seleccionada?.CodigoNomina ?? 0,

            CodigoCentro = (cmbCentros.SelectedItem as Centro)?.CodigoCentro ?? 0,
            CodigoEmpleado = (cmbEmpleados.SelectedItem as Personal)?.CodigoEmpleado ?? 0,
            CodigoEspecialidad = (cmbEspecialidades.SelectedItem as Especialidad)?.CodigoEspecialidad ?? 0,

            Salario = decimal.TryParse(txtSalario.Text, out var salario) ? salario : 0,

            FechaConsulta = dpFechaConsulta.SelectedDate?.DateTime ?? DateTime.Now,
            Hora = tpHora.SelectedTime ?? TimeSpan.Zero,

            OrigenDatos = nodoActual
        };
    }

    private async void BtnEliminar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (GridNominas.SelectedItem is Nomina nomina)
            {
                var origen = ConfigGlobal.NodoSeleccionado;

                await _nominaRepository.EliminarNominaAsync(nomina.CodigoNomina, origen);

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
            if (GridNominas.SelectedItem is Nomina nomina)
            {
                _seleccionada = nomina;

                cmbCentros.SelectedItem = cmbCentros.Items
                    .OfType<Centro>()
                    .FirstOrDefault(c => c.CodigoCentro == nomina.CodigoCentro);

                cmbEmpleados.SelectedItem = cmbEmpleados.Items
                    .OfType<Personal>()
                    .FirstOrDefault(p => p.CodigoEmpleado == nomina.CodigoEmpleado);

                cmbEspecialidades.SelectedItem = cmbEspecialidades.Items
                    .OfType<Especialidad>()
                    .FirstOrDefault(e => e.CodigoEspecialidad == nomina.CodigoEspecialidad);

                txtSalario.Text = nomina.Salario.ToString();

                if (nomina.FechaConsulta != null)
                {
                    dpFechaConsulta.SelectedDate =
                        DateTime.SpecifyKind(nomina.FechaConsulta.Date, DateTimeKind.Local);
                }

                tpHora.SelectedTime = nomina.Hora;
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

    private void LimpiarFormulario()
    {
        cmbCentros.SelectedItem = null;
        cmbEmpleados.SelectedItem = null;
        cmbEspecialidades.SelectedItem = null;

        txtSalario.Text = string.Empty;

        dpFechaConsulta.SelectedDate = null;
        tpHora.SelectedTime = null;

        _seleccionada = null;
    }

    private async void BtnGuardar_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var nomina = ObtenerNomina();
            var origen = ConfigGlobal.NodoSeleccionado;

            if (_seleccionada == null)
            {
                await _nominaRepository.InsertarNominaAsync(nomina, origen);
            }
            else
            {
                await _nominaRepository.ActualizarNominaAsync(nomina, origen);
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
}