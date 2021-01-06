#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Caliburn.Micro;
using EcoFactory.Components.Factories;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public class TransformationRuleEditorViewModel : PropertyChangedBase
  {
    private readonly ObservableCollection<object> _availableProductTypesForInput;
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IModel _owningModel;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourceFactory;
    private readonly ITransformationRuleOutputFactory _transformationRuleOutputFactory;

    public TransformationRuleEditorViewModel(ITransformationRule rule,
                                             IModel owningModel,
                                             IProductTypeAmountFactory productTypeAmountFactory,
                                             ITransformationRuleOutputFactory transformationRuleOutputFactory,
                                             IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                             IResourcePoolResourceTypeAmountFactory resourceFactory,
                                             IChainedParameterCommandFactory chainedParameterCommandFactory,
                                             bool showProbability = true)
    {
      Model = rule;
      ShowProbability = showProbability;
      _owningModel = owningModel;
      _productTypeAmountFactory = productTypeAmountFactory;
      _transformationRuleOutputFactory = transformationRuleOutputFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _resourceFactory = resourceFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;

      RemoveCommand = new RelayCommand(Remove, o => true);
      Inputs = new ObservableCollection<ProductTypeAmountEditorViewModel>(rule.Inputs.Select(ptDist => new ProductTypeAmountEditorViewModel(ptDist)));
      _availableProductTypesForInput = new ObservableCollection<object>(GetAvailableProductTypes(Inputs.Select(ptsd => ptsd.ProductType.Model)));
      AddInputCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificAmountCommand("Add",
                                                                                                 _availableProductTypesForInput,
                                                                                                 AddProductTypeAsInput);

      Outputs = new ObservableCollection<TransformationRuleOutputViewModel>(Model.Outputs.Select(CreateAlternativeOutput));
      AddOutputAlternativeCommand = new RelayCommand(AddOutput);
      UpdateShortDisplayText();
    }

    public ChainedParameterCommandViewModel AddInputCommand { get; private set; }

    public ITransformationRule Model { get; }

    public bool ShowProbability { get; }

    public ICommand RemoveCommand { get; private set; }

    public ICommand AddOutputAlternativeCommand { get; private set; }

    public int Probability
    {
      get { return Model.Probability; }
      set
      {
        Model.Probability = value;
        UpdateShortDisplayText();
        NotifyOfPropertyChange(() => Probability);
      }
    }

    public ObservableCollection<ProductTypeAmountEditorViewModel> Inputs { get; private set; }

    public ObservableCollection<TransformationRuleOutputViewModel> Outputs { get; private set; }

    public string DisplayShortText { get; private set; }

    private TransformationRuleOutputViewModel CreateAlternativeOutput(ITransformationRuleOutput trOutput)
    {
      //todo: use factory
      return new TransformationRuleOutputViewModel(trOutput,
                                                   _owningModel,
                                                   _productTypeAmountFactory,
                                                   _distributionConfigurationViewModelFactory,
                                                   _resourceFactory,
                                                   _chainedParameterCommandFactory);
    }

    private void Remove(object obj)
    {
      if (obj is ProductTypeAmountEditorViewModel)
      {
        RemoveInput(obj);
      }
      else if (obj is TransformationRuleOutputViewModel)
      {
        RemoveOutput(obj);
      }
    }

    private IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _owningModel.Entities.OfType<IProductType>()
                         .Except(productTypesAlreadyInUse);
    }

    /*
     * input related
     */

    private void RemoveInput(object item)
    {
      var pta = (ProductTypeAmountEditorViewModel) item;
      Model.RemoveInput(pta.Model);
      _availableProductTypesForInput.Add(pta.Model);
      Inputs.Remove(pta);
      UpdateShortDisplayText();
    }

    private void AddProductTypeAsInput(IDictionary<string, object> parameters)
    {
      AddInput(parameters,
               vm =>
               {
                 Inputs.Add(vm);
                 // remove product type from selectable parameter values (so it can't be selected twice)
                 _availableProductTypesForInput.Remove(vm.ProductType.Model);

                 UpdateShortDisplayText();
               },
               Model.AddInput);
    }

    private void AddInput(IDictionary<string, object> parameters,
                          Action<ProductTypeAmountEditorViewModel> updateViewModel,
                          Action<IProductTypeAmount> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters["productType"];

      // use values to create new entry, add to model (workstation)
      var ptSpecific = _productTypeAmountFactory.Create();
      ptSpecific.ProductType = productType;
      addToModel(ptSpecific);
      //todo: use factory
      var vm = new ProductTypeAmountEditorViewModel(ptSpecific);

      updateViewModel(vm);
    }

    /*
     * output related
     */

    private void AddOutput(object obj)
    {
      var newOutput = _transformationRuleOutputFactory.Create();
      Model.AddOutput(newOutput);
      Outputs.Add(CreateAlternativeOutput(newOutput));
      UpdateShortDisplayText();
    }

    private void RemoveOutput(object item)
    {
      var output = (TransformationRuleOutputViewModel) item;
      Model.RemoveOutput(output.Model);
      Outputs.Remove(output);
      UpdateShortDisplayText();
    }

    /*
     * display texts related
     */

    private void UpdateShortDisplayText()
    {
      var sb = new StringBuilder();

      FormatProductTypeAmountsDetailed(sb, Model.Inputs.ToArray());

      sb.Append(" >>> ");

      if (Model.Outputs.Any())
      {
        var firstOutput = Model.Outputs.First();
        FormatProductTypeAmountsShort(sb, firstOutput.Outputs.ToArray());
        sb.AppendFormat(" ({0}%)", firstOutput.Probability);

        foreach (var nextOutput in Model.Outputs.Skip(1))
        {
          sb.AppendFormat(" or ");
          FormatProductTypeAmountsShort(sb, nextOutput.Outputs.ToArray());
          sb.AppendFormat(" ({0}%)", nextOutput.Probability);
        }
      }
      else
      {
        sb.Append(" ? ");
      }

      DisplayShortText = sb.ToString();
      NotifyOfPropertyChange(() => DisplayShortText);
    }

    private void FormatProductTypeAmountsDetailed(StringBuilder sb, IProductTypeAmount[] productTypeAmounts)
    {
      if (productTypeAmounts.Any())
      {
        var firstInput = productTypeAmounts.First();
        sb.AppendFormat("{0} {1}", firstInput.Amount, firstInput.ProductType);
        foreach (var nextInput in productTypeAmounts.Skip(1))
        {
          sb.AppendFormat(" and {0} {1}", nextInput.Amount, nextInput.ProductType);
        }
      }
      else
      {
        sb.Append("Nothing");
      }
    }

    private void FormatProductTypeAmountsShort(StringBuilder sb, IProductTypeAmount[] productTypeAmounts)
    {
      if (productTypeAmounts.Any())
      {
        var firstInput = productTypeAmounts.First();
        sb.AppendFormat("{0}x {1}", firstInput.Amount, firstInput.ProductType);
        foreach (var nextInput in productTypeAmounts.Skip(1))
        {
          sb.AppendFormat(" + {0}x {1}", nextInput.Amount, nextInput.ProductType);
        }
      }
      else
      {
        sb.Append("Nothing");
      }
    }
  }
}