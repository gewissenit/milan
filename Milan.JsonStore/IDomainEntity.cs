#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;

namespace Milan.JsonStore
{
  public interface IDomainEntity : INotifyPropertyChanged, IComparable
  {
    bool IsReadonly { get; set; }
    Guid Id { get; set; }
  }
}