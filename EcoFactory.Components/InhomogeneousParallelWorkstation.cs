#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.States;
using Milan.Simulation;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class InhomogeneousParallelWorkstation : WorkstationBase, IInhomogeneousParallelWorkstation
  {
    private bool _isInSetup;
    private bool _noShiftIsActive;
    private List<ParallelProcess> _processes;

    public override bool IsAvailable(Product product)
    {
      if (!_initialized ||
          !base.IsAvailable(product) ||
          _isInSetup)
      {
        return false;
      }
      var batchSize = _currentBatchSize ?? double.MaxValue;
      if (_processes.Any(p => p.Active))
      {
        return true;
      }
      else if (_idle.Active)
      {
        if (_arrivedProducts.Count == 0 ||
            _arrivedProducts.Count < batchSize)
        {
          return true;
        }
      }
      return false;
    }

    public override void Initialize()
    {
      base.Initialize();
      _processes = new List<ParallelProcess>();
      CheckBatchSizes();

      _processingResourceManager = new ResourceManager(_awaitingProcessingResources, CreateAndEnterParallelProductionProcess)
                                   {
                                     RequestedResources = ProcessingResourcesDictionary
                                   };

      if (HasSetup)
      {
        _setup.OnFinish = OnExitSetup;
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
      _processes = null;
      _noShiftIsActive = false;
      _isInSetup = false;
      base.Reset();
    }

    public override void Receive(Product product)
    {
      base.Receive(product);
      _arrivedProducts.Enqueue(product);
      var currentBatchSize = GetBatchSize(product);
      if (_arrivedProducts.Count == currentBatchSize)
      {
        if (NeedSetup(product))
        {
          if (_idle.Active)
          {
            _idle.Exit();
          }
          _setupResourceManager.HandleResourcesAndEnterProductionProcess();
          if (IsDemandingProcessingResourcesInSetup)
          {
            _processingResourceManager.HandleResources();
          }
        }
        else
        {
          if (!_processes.Any(p => p.Active))
          {
            _idle.Exit();
          }
          _processingResourceManager.HandleResourcesAndEnterProductionProcess();
        }
      }
    }

    protected override bool NeedSetup(Product product)
    {
      return HasSetup;
    }

    protected override void StartUp()
    {
      _noShiftIsActive = false;

      if (CanFail && _failure.Active)
      {
        return;
      }

      if (_processes.Any(p => p.Active))
      {
        return;
      }

      _off.Exit();

      if (_blockedProducts.Any())
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

        if (_blockedProducts.Any())
        {
          _blocked.Enter();
          return;
        }
      }

      if (_processes.Any())
      {
        foreach (var process in _processes)
        {
          process.Resume();
        }
        if (!_isInSetup)
        {
          RaiseGotAvailable();
        }
      }
      else if (!_isInSetup)
      {
        _idle.Enter();
      }

      if (_isInSetup && (!HasSetup || !_setup.Active))
      {
        _setup.Start();
      }
    }

    protected override void ShutDown()
    {
      _noShiftIsActive = true;
      if (CanFail && _failure.Active)
      {
        return;
      }
      if (_isInSetup)
      {
        //TODO: use suspend
        _setup.Cancel();
      }
      if (_processes.Any(p => p.Active))
      {
        return;
      }

      if (_idle.Active)
      {
        _idle.Exit();
      }
      else if (_blocked.Active)
      {
        _blocked.Exit();
      }
      _off.Enter();
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

      if (!_blockedProducts.Any())
      {
        if (!CanFail ||
            !_failure.Active)
        {
          _blocked.Exit();
          if (_processes.Any())
          {
            foreach (var process in _processes)
            {
              process.Resume();
            }
            if (!_isInSetup)
            {
              RaiseGotAvailable();
            }
          }
          else if (!_isInSetup)
          {
            _idle.Enter();
          }

          if (_isInSetup && (!HasSetup || !_setup.Active))
          {
            _setup.Start();
          }
        }
      }
    }

    private void OnEnterSetup()
    {
      _isInSetup = true;
    }

    private void OnExitSetup()
    {
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
      RaiseGotAvailable();
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

      foreach (var process in _processes)
      {
        process.Cancel();
      }
      _processes.Clear();
      _processingResourceManager.ReturnResources(this, _lastProcessingResourceReceivedEvent);

      if (_isInSetup)
      {
        _setup.Cancel();
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

    private void OnExitParallelProcess(ParallelProcess process)
    {
      _processingResourceManager.ReturnResources(this, _lastProcessingResourceReceivedEvent);
      _processes.Remove(process);
      while (process.Products.Any())
      {
        var product = process.Products.Dequeue();
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
        foreach (var productionProcess in _processes)
        {
          productionProcess.Suspend();
        }
        if (_isInSetup)
        {
          _setup.Cancel();
        }
        _off.Enter();
      }
      else if (_blockedProducts.Any())
      {
        foreach (var productionProcess in _processes)
        {
          productionProcess.Suspend();
        }
        if (_isInSetup)
        {
          _setup.Cancel();
        }
        _blocked.Enter();
      }
      else if (!_processes.Any() &&
               !_isInSetup)
      {
        _idle.Enter();
      }
    }

    private void CreateAndEnterParallelProductionProcess()
    {
      var process = new ParallelProcess("Processing", this, _arrivedProducts.ToArray());
      _arrivedProducts.Clear();
      _currentBatchSize = null;
      process.DurationDistribution = ProcessingDurationDistribution;
      process.ProductTypeDistributions = ProcessingsDictionary;
      process.OnFinish = OnExitParallelProcess;
      _processes.Add(process);
      process.Start();
    }
  }
}