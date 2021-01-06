#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

namespace Emporer.WPF.ViewModels
{
  /// <summary>
  ///   Hilfsklasse für INotifyDataErrorInfo-Implementierung
  /// </summary>
  public class ValidationBase : PropertyChangedBase, INotifyDataErrorInfo
  {
    /// <summary>
    ///   ctor
    /// </summary>
    public ValidationBase()
    {
      Validators = new List<Validator>();
    }

    /// <summary>
    ///   Liste der Validator-Objekte
    /// </summary>
    public List<Validator> Validators { get; private set; }

    /// <summary>
    ///   Validierung durchführen
    /// </summary>
    public void Validate()
    {
      // Liste der Validator-Objekte durchlaufen
      foreach (var validator in Validators)
      {
        // liegt ein Fehler vor?
        var hasError = validator.CheckForError();

        // Wenn sich der Zustand geändert hat, ErrorsChanged feuern
        if (hasError != validator.LastStatus)
        {
          validator.LastStatus = hasError;
          if (ErrorsChanged != null)
          {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(validator.PropertyName));
          }
        }
      }
    }

    #region INotifyDataErrorInfo-Implementierung

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    /// <summary>
    ///   Liste der Fehlermeldungen für die angegebene Eigenschaft ermitteln
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public IEnumerable GetErrors(string propertyName)
    {
      var errors = Validators.Where(v => v.PropertyName == propertyName && v.LastStatus)
                             .Select(v => v.ErrorMessage);
      return errors;
    }

    /// <summary>
    ///   Prüfen, ob Fehler vorliegen
    /// </summary>
    public bool HasErrors
    {
      get { return !Validators.TrueForAll(v => !v.LastStatus); }
    }

    #endregion
  }

  /// <summary>
  ///   Hilfsklasse für Validierungen
  /// </summary>
  public class Validator
  {
    /// <summary>
    ///   Name der Eigenschaft
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    ///   Methode, die im Fehlerfall true zurück gibt
    /// </summary>
    public Func<bool> CheckForError { get; set; }

    /// <summary>
    ///   Die Fehlermeldung
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    ///   Letzter Zustand. True bei Fehler
    /// </summary>
    public bool LastStatus { get; set; }
  }
}