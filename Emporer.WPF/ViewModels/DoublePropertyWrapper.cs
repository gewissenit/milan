#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Emporer.WPF.ViewModels
{
  public class DoublePropertyWrapper : PropertyWrapper<double>
  {
    public DoublePropertyWrapper(Func<double> getter, Action<double> setter)
      : base(getter, setter)
    {
    }
  }
}