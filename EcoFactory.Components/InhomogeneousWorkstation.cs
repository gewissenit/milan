#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class InhomogeneousWorkstation : WorkstationBase, IInhomogeneousWorkstation
  {
    private bool _isInSetup;
    private bool _noShiftIsActive;

    public override bool IsAvailable(Product product)
    {
      if (!_initialized ||
          !_idle.Active ||
          !base.IsAvailable(product))
      {
        return false;
      }

      var batchSize = _currentBatchSize ?? double.MaxValue;
      return _arrivedProducts.Count == 0 || (_arrivedProducts.Count < batchSize);
    }


    public override void Initialize()
    {
      base.Initialize();

      _processing.OnFinish = OnExitProcessing;
      CheckBatchSizes();

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

    public override void Reset()
    {
      _isInSetup = false;
      _noShiftIsActive = false;
      base.Reset();
    }

    public override void Receive(Product product)
    {
      base.Receive(product);
      _arrivedProducts.Enqueue(product);
      var currentBatchSize = GetBatchSize(product);
      if (_arrivedProducts.Count == currentBatchSize)
      {
        _idle.Exit();
        if (NeedSetup(product))
        {
          _setup.OnFinish = () =>
                            {
                              _lastProduct = product;
                              _isInSetup = false;
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
          _lastProduct = product;
          _processingResourceManager.HandleResourcesAndEnterProductionProcess();
        }
      }
    }

    protected override bool NeedSetup(Product product)
    {
      return HasSetup;
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
          ((!CanFail || !_failure.Active)))
      {
        _blocked.Exit();
        _idle.Enter();
      }
    }

    private void OnEnterSetup()
    {
      _isInSetup = true;
    }

    private void OnEnterFailure()
    {
      if (_idle.Active)
      {
        _idle.Exit();
      }
      else if (_off.Active)
      {
        _off.Exit();
      }
      else if (_blocked.Active)
      {
        _blocked.Exit();
      }
      else if (_arrivedProducts.Any())
      {
        if (_isInSetup)
        {
          _setup.Cancel();
        }
        else
        {
          _processing.Cancel();
          _arrivedProducts.Clear();
          _currentBatchSize = null;
          _processingResourceManager.ReturnResources(this, _lastProcessingResourceReceivedEvent);
        }
      }
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
      while (_arrivedProducts.Any())
      {
        var product = _arrivedProducts.Dequeue();
        if (!_productSender.CanTransmit(product))
        {
          _blockedProducts.Enqueue(product);
        }
        else
        {
          Send(product);
        }
      }

      _currentBatchSize = null;
      if (_noShiftIsActive)
      {
        _off.Enter();
      }
      else if (!_blockedProducts.Any())
      {
        _idle.Enter();
      }
      else
      {
        _blocked.Enter();
      }
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
  }
}