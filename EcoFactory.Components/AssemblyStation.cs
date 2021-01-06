#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using EcoFactory.Components.States;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Resources;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class AssemblyStation : WorkstationBase, IAssemblyStation
  {
    [JsonProperty]
    private readonly IList<ITransformationRule> _TransformationRules = new List<ITransformationRule>();

    private bool _isInSetup;
    private Queue<Product> _lastInput;
    private Queue<Product> _lastOutput;
    private ITransformationRule _lastTransformationRule;

    private bool _noShiftIsActive;
    private new Process _processing;
    private IDictionary<ITransformationRuleOutput, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> _transformationRuleSpecificProcessingResourcesDictionary;

    public IDictionary<ITransformationRuleOutput, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> TransformationRuleSpecificProcessingResourcesDictionary
    {
      get { return _transformationRuleSpecificProcessingResourcesDictionary ?? (_transformationRuleSpecificProcessingResourcesDictionary = new Dictionary<ITransformationRuleOutput, IDictionary<IResourcePool, IDictionary<IResourceType, int>>>()); }
    }

    public IEnumerable<ITransformationRule> TransformationRules
    {
      get { return _TransformationRules; }
    }

    public void AddTransformationRule(ITransformationRule transformationRule)
    {
      if (_TransformationRules.Contains(transformationRule))
      {
        throw new InvalidOperationException("The given transformationRule does already exist.");
      }
      _TransformationRules.Add(transformationRule);
    }

    public override void Initialize()
    {
      base.Initialize();

      CheckTransformationRules();

      _lastInput = new Queue<Product>();
      _lastOutput = new Queue<Product>();

      _processing = new Process("Processing", this, CreateProcessingStart, CreateProcessingEnd, CreateProcessingCancel)
                    {
                      OnFinish = OnExitProcessing
                    };

      _processingResourceManager = new ResourceManager(_awaitingProcessingResources, _processing.Start)
                                   {
                                     RequestedResources = ProcessingResourcesDictionary
                                   };

      if (HasSetup)
      {
        _setup.OnStart = OnEnterSetup;
      }
      if (CanFail)
      {
        _failure.OnStart = OnEnterFailure;
        _failure.OnFinish = OnExitFailure;
      }

      if (IsWorkingTimeDependent)
      {
        _noShiftIsActive = true;
        _off.Enter();
      }
      else
      {
        _idle.Enter();
      }
    }

    public override bool IsAvailable(Product product)
    {
      var amount = 0;
      amount = TransformationRules.Select(transformationRule => transformationRule.Inputs.Where(i => i.ProductType == product.ProductType)
                                                                                  .Sum(productTypeAmount => productTypeAmount.Amount))
                                  .Concat(new[]
                                          {
                                            amount
                                          })
                                  .Max();
      return _initialized && _idle.Active && amount > _arrivedProducts.Count(p => p.ProductType == product.ProductType);
    }

    public override void Receive(Product product)
    {
      base.Receive(product);

      _arrivedProducts.Enqueue(product);
      ITransformationRule transformationRule;

      if (TryGetFittingTransformationRule(out transformationRule))
      {
        _idle.Exit();
        if (transformationRule != _lastTransformationRule && HasSetup)
        {
          _setup.OnFinish = () =>
                            {
                              _isInSetup = false;
                              _lastTransformationRule = transformationRule;
                              TransformProducts();
                              _setupResourceManager.ReturnResources(this, _lastSetupResourceReceivedEvent);
                              if (IsDemandingProcessingResourcesInSetup && _processingResourceManager.ResourcesAvailable)
                              {
                                _processingResourceManager.EnterProductionProcess();
                              }
                              else
                              {
                                _processingResourceManager.HandleResourcesAndEnterProductionProcess();
                              }
                            };
          _setupResourceManager.HandleResourcesAndEnterProductionProcess();
          if (IsDemandingProcessingResourcesInSetup)
          {
            _processingResourceManager.HandleResources();
          }
        }
        else
        {
          _lastTransformationRule = transformationRule;
          TransformProducts();
          _processingResourceManager.HandleResourcesAndEnterProductionProcess();
        }
      }
    }

    public void RemoveTransformationRule(ITransformationRule transformationRule)
    {
      if (!_TransformationRules.Contains(transformationRule))
      {
        throw new InvalidOperationException("The given transformationRule does not exist.");
      }
      _TransformationRules.Remove(transformationRule);
    }

    public override void Reset()
    {
      _lastTransformationRule = null;
      _lastOutput = null;
      _lastInput = null;
      _processing = null;
      _isInSetup = false;
      _noShiftIsActive = false;
      foreach (var transformationRule in TransformationRules)
      {
        transformationRule.Distribution = null;
        foreach (var output in transformationRule.Outputs)
        {
          output.Distribution = null;
        }
      }

      base.Reset();
    }

    protected override void OnGotAvailable(object sender, EventArgs e)
    {
      if (!_initialized ||
          _off.Active ||
          !_blockedProducts.Any())
      {
        return;
      }

      while (_blockedProducts.Any() &&
             _productSender.CanTransmit(_blockedProducts.Peek()))
      {
        var product = _blockedProducts.Dequeue();
        Send(product);
      }
      if (!_blockedProducts.Any() &&
          (!CanFail || !_failure.Active))
      {
        _blocked.Exit();
        _idle.Enter();
      }
    }

    protected override void PrepareSetup()
    {
      _setup.DurationDistribution = SetupDurationDistribution;
    }

    protected override void ShutDown()
    {
      _noShiftIsActive = true;
      if (CanFail && _failure.Active)
      {
        return;
      }
      if (_processing.Active)
      {
        return;
      }

      if (HasSetup && _setup.Active)
      {
        _setup.Cancel();
      }
      else if (_idle.Active)
      {
        _idle.Exit();
      }
      else if (_blocked.Active)
      {
        _blocked.Exit();
      }
      _off.Enter();
    }

    protected override void StartUp()
    {
      _noShiftIsActive = false;
      if (CanFail && _failure.Active)
      {
        return;
      }
      if (_processing.Active)
      {
        return;
      }

      _off.Exit();
      if (_isInSetup)
      {
        _setup.Start();
      }
      else if (_blockedProducts.Any())
      {
        var products = new Queue<Product>(_blockedProducts);
        _blockedProducts.Clear();
        while (products.Any())
        {
          var product = products.Dequeue();
          if (!_productSender.CanTransmit(product))
          {
            _blockedProducts.Enqueue(product);
          }
          else
          {
            Send(product);
          }
        }
        if (!_blockedProducts.Any())
        {
          _idle.Enter();
        }
        else
        {
          _blocked.Enter();
        }
      }
      else
      {
        _idle.Enter();
      }
    }

    private void OnEnterFailure()
    {
      if (_idle.Active)
      {
        _idle.Exit();
      }
      if (_off.Active)
      {
        _off.Exit();
      }
      if (_blocked.Active)
      {
        _blocked.Exit();
      }
      _lastInput.Clear();
      if (_isInSetup)
      {
        _setup.Cancel();
      }
      else if (_lastOutput.Any())
      {
        _processing.Cancel();
        _processingResourceManager.ReturnResources(this, _lastProcessingResourceReceivedEvent);
        _lastOutput.Clear();
      }
    }

    private void OnEnterSetup()
    {
      _isInSetup = true;
    }

    private void OnExitFailure()
    {
      if (_noShiftIsActive)
      {
        _off.Enter();
      }
      else if (_isInSetup)
      {
        _setup.Start();
      }
      else if (_blockedProducts.Any())
      {
        _blocked.Enter();
      }
      else
      {
        _idle.Enter();
      }
    }

    private void OnExitProcessing()
    {
      _processingResourceManager.ReturnResources(this, _lastProcessingResourceReceivedEvent);
      _lastInput.Clear();
      while (_lastOutput.Count > 0)
      {
        var product = _lastOutput.Dequeue();

        /*HACK: product creation happened at a point in time where its real life time in the model wasn't determined.
         * So we have to reset the timestamp here. */
        product.TimeStamp = CurrentExperiment.CurrentTime;
        product.EntryPoint = this;

        if (!_productSender.CanTransmit(product))
        {
          _blockedProducts.Enqueue(product);
        }
        else
        {
          Send(product);
        }
      }

      if (_noShiftIsActive)
      {
        _off.Enter();
      }
      else if (_blockedProducts.Any())
      {
        _blocked.Enter();
      }
      else
      {
        _idle.Enter();
      }
    }

    private void TransformProducts()
    {
      if (_lastInput.Any())
      {
        throw new InvalidOperationException("This should not occur!");
      }

      foreach (var productTypeAmount in _lastTransformationRule.Inputs)
      {
        var productsOfType = new Queue<Product>(_arrivedProducts.Where(p => p.ProductType == productTypeAmount.ProductType));
        for (var i = 0; i < productTypeAmount.Amount; i++)
        {
          var product = productsOfType.Dequeue();
          _lastInput.Enqueue(product);
          var temp = new Queue<Product>();
          while (_arrivedProducts.Count != 0)
          {
            var currentProduct = _arrivedProducts.Dequeue();
            if (currentProduct != product)
            {
              temp.Enqueue(currentProduct);
            }
          }
          _arrivedProducts = temp;
        }
      }
      var output = _lastTransformationRule.GetSampleOutput();
      SetProcessingResources(output);
      foreach (var productTypeAmount in output.Outputs)
      {
        for (var i = 0; i < productTypeAmount.Amount; i++)
        {
          _lastOutput.Enqueue(new Product(Model, productTypeAmount.ProductType, CurrentExperiment.CurrentTime, _lastInput.ToArray())
                              {
                                EntryPoint = this
                              });
        }
      }
      _processing.DurationDistribution = output.Distribution;
    }

    private void CheckTransformationRules()
    {
      if (!TransformationRules.Any())
      {
        throw new ModelConfigurationException(Model, this, $"Transformation rules in workstation {Name} are not well defined. Please provide at least one rule.", "TransformationRules");
      }
    }

    private ISimulationEvent CreateProcessingCancel()
    {
      return new ProductsDestroyedEvent(this, _lastOutput.ToArray());
    }

    private IRelatedEvent CreateProcessingEnd(ISimulationEvent arg)
    {
      return new TransformationEndEvent(this, _lastOutput.ToArray(), arg, _lastTransformationRule);
    }

    private ISimulationEvent CreateProcessingStart()
    {
      return new TransformationStartEvent(this, _lastInput.ToArray(), _lastTransformationRule);
    }

    private void SetProcessingResources(ITransformationRuleOutput output)
    {
      _processingResourceManager.RequestedResources = TransformationRuleSpecificProcessingResourcesDictionary.ContainsKey(output)
                                                        ? TransformationRuleSpecificProcessingResourcesDictionary[output]
                                                        : ProcessingResourcesDictionary;
    }

    private bool TryGetFittingTransformationRule(out ITransformationRule transformationRule)
    {
      var possibleRules = TransformationRules.Where(IsRulePossible)
                                             .ToArray();

      if (!possibleRules.Any())
      {
        transformationRule = null;
        return false;
      }

      transformationRule = possibleRules.OrderBy(r => r.Probability)
                                        .Last();

      return true;
    }


    private bool IsRulePossible(ITransformationRule rule)
    {
      foreach (var input in rule.Inputs)
      {
        var products = _arrivedProducts.Where(p => p.ProductType == input.ProductType);
        if (products.Count() < input.Amount)
        {
          return false;
        }
      }
      return true;
    }
  }
}