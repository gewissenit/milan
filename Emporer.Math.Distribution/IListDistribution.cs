#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public interface IListDistribution : IDistribution
  {
    [Browsable(true)]
    [ReadOnly(true)]
    [DisplayName("EntryCount")]
    [Category("Parameters")]
    [Description("This counter tells you how many entries was add into the list.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    int EntryCount
    {
      get;
    }

    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("IsPeriodic")]
    [Category("Parameters")]
    [Description("<c>true</c> if this distribution is periodic; otherwise, <c>false</c>.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    bool IsPeriodic
    {
      get;
      set;
    }

    [Browsable(true)]
    [ReadOnly(true)]
    [DisplayName("IsInitialized")]
    [Category("Parameters")]
    [Description(
      "Determines whether this empirical distribution has been properly initialized. /n <c>true</c> if this instance is initialized; otherwise, <c>false</c>."
      )]
    [EditorBrowsable(EditorBrowsableState.Always)]
    bool IsInitialized
    {
      get;
    }

    void AddEntry(double value);
  }
}