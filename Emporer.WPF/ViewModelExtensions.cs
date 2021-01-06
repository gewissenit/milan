using System;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using GeWISSEN.Utils;

namespace Emporer.WPF
{
  public static class ViewModelExtensions
  {
    public static void SetAndRaiseIfChanged<T, TProperty>(this T propertyChangedBase,
                                                          ref TProperty field,
                                                          TProperty newValue,
                                                          [CallerMemberName] string propertyName = null) where T : PropertyChangedBase
    {
      Throw.IfNull(propertyChangedBase, nameof(propertyChangedBase));
      Throw.IfNull(newValue, nameof(newValue));

      if (field != null &&
          field.Equals(newValue))
      {
        return;
      }
      field = newValue;
      propertyChangedBase.NotifyOfPropertyChange(propertyName);
    }
  }
}