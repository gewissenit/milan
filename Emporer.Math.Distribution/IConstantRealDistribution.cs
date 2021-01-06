#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public interface IConstantRealDistribution : IRealDistribution
  {
    /// <summary>
    ///   Get/Set the ConstantValue of this distribution, which will be always returned.
    /// </summary>
    /// <value>constant value</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("ConstantValue")]
    [Category("Parameters")]
    [Description("The ConstantValue of this distribution, which will be always returned.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double ConstantValue { get; set; }
  }
}