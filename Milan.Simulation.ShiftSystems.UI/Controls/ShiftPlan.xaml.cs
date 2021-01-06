#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Milan.Simulation.ShiftSystems.UI.ViewModels;

namespace Milan.Simulation.ShiftSystems.UI.Controls
{
  /// <summary>
  ///   Interaction logic for ShiftPlan.xaml
  /// </summary>
  public partial class ShiftPlan : UserControl
  {
    public ShiftPlan()
    {
      InitializeComponent();
    }

    private void SelectShift(object sender, RoutedEventArgs e)
    {
      var selectedShiftVM = (ShiftViewModel) ((Button) sender).DataContext;
      ((ShiftManagementSectionViewModel) DataContext).SelectedShift = selectedShiftVM.Shift;
    }

    private void UnselectShift(object sender, MouseButtonEventArgs e)
    {
      ((ShiftManagementSectionViewModel)DataContext).SelectedShift = null;
    }
  }
}