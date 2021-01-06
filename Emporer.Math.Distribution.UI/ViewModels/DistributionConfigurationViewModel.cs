#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class DistributionConfigurationViewModel : PropertyChangedBase
  {
    private readonly IDistributionConfiguration _entity;

    public DistributionConfigurationViewModel(IDistributionConfiguration entity)
    {
      _entity = entity;
    }

    public IDistributionConfiguration Entity
    {
      get { return _entity; }
    }

    public string Name
    {
      get { return Entity.Name; }
    }
  }
}