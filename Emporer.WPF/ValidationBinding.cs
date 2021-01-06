#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows.Data;

namespace Emporer.WPF
{
  /// <summary>
  ///   Alternative zur {Binding}-MarkupExtension. NotifyOnValidationError wird hier auf true gesetzt, sonst bleibt alles
  ///   gleich
  /// </summary>
  public class ValidationBinding : Binding
  {
    public ValidationBinding()
    {
      NotifyOnValidationError = true;
      ValidatesOnNotifyDataErrors = true;
      ValidatesOnDataErrors = true;
      ValidatesOnExceptions = true;
      UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
    }

    public ValidationBinding(string path)
      : base(path)
    {
      NotifyOnValidationError = true;
      ValidatesOnNotifyDataErrors = true;
      ValidatesOnDataErrors = true;
      ValidatesOnExceptions = true;
      UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
    }
  }
}