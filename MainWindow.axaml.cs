using Avalonia.Controls;
using MedicasMultibase.Modelos;

namespace MedicasMultibase;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender!;
        // Verificamos que haya un item seleccionado y extraemos su Tag
        if (comboBox.SelectedItem is ComboBoxItem itemSeleccionado && itemSeleccionado.Tag != null)
        {
            // Actualizamos la variable global
            ConfigGlobal.NodoSeleccionado = itemSeleccionado.Tag.ToString()!;
            ConfigGlobal.NodoCambioEvent?.Invoke();
        }
    }
}