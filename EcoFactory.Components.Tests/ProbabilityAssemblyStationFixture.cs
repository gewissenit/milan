#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Events;
using Milan.Simulation.Tests;
using Moq;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class ProbabilityAssemblyStationFixture : WorkstationBaseFixture<ProbabilityAssemblyStation>
  {
    protected override ProbabilityAssemblyStation CreateSUT()
    {
      return new ProbabilityAssemblyStation();
    }

    protected override void Default_Ctor(ProbabilityAssemblyStation sut)
    {
      Assert.IsEmpty(sut.TransformationRules.ToArray());
      base.Default_Ctor(sut);
    }

    [Test]
    public void Add_TransformationRule()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      sut.AddTransformationRule(transformationRuleMock);
      Assert.Contains(transformationRuleMock, sut.TransformationRules.ToArray());
      Assert.AreEqual(1, sut.TransformationRules.Count());
    }

    [Test]
    public void Cancel_Setup_When_On_Failure_And_Then_Setups_Again_After_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();

      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(20);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(3);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.HasSetup = true;
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.CanFail = true;

      sut.Initialize();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<SetupStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<SetupEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<SetupCancelEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<SetupStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Cancel_Setup_When_ShutDown_And_Then_Setups_Again_After_ReStartUp()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(5);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.HasSetup = true;
      sut.IsWorkingTimeDependent = true;

      sut.Initialize();
      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();

      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<SetupStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<SetupEndEvent>()
                                  .Count());
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<SetupCancelEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<SetupStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Destroy_Processing_Products_On_Failure()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productA2 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);

      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();

      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);
      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.CanFail = true;

      sut.Initialize();

      Assert.IsTrue(sut.IsAvailable(productA1));
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(4, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsDestroyedEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsDestroyedEvent>(productTypeD));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Do_Not_Destroy_Blocked_Products_On_Failure()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productA2 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);

      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();

      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();


      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);


      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();


      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(5);


      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);


      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(10);


      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(productB1))
                   .IgnoreArguments()
                   .Return(false)
                   .Repeat.Twice();
      successorMock.Expect(se => se.IsAvailable(productA1))
                   .IgnoreArguments()
                   .Return(true)
                   .Repeat.Times(4);

      successorMock.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);
      _entities.Add(successorMock);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.Add(connectionMock);
      sut.CanFail = true;
      Assert.IsFalse(sut.IsAvailable(productA1));

      sut.Initialize();

      Assert.IsTrue(sut.IsAvailable(productA1));
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(4, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      Assert.AreEqual(2, scheduler.Count());

      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<BlockedEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(15, scheduler.Clock.CurrentTime);
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      eventRaiser.Raise(sut.Connections.First(), EventArgs.Empty);
      Assert.IsTrue(sut.IsAvailable(productA2));
      sut.IsInState("Idle");
      Assert.AreEqual(5, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductTransmitEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductTransmitEvent>(productTypeD));
      Assert.AreEqual(1, scheduler.OfType<BlockedEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Fail_On_Add_TransformationRule_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      sut.AddTransformationRule(transformationRuleMock);
      Assert.Throws<InvalidOperationException>(() => sut.AddTransformationRule(transformationRuleMock));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_TransformationRule()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveTransformationRule(transformationRuleMock));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_TransformationRule()
    {
      var sut = CreateMinimalConfiguredSUT();
      ITransformationRule transformationRuleMock = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveTransformationRule(transformationRuleMock));
    }

    [Test]
    public void Is_Available_After_Shift_Is_Started()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.IsWorkingTimeDependent = true;
      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.IsTrue(sut.IsAvailable(productA1));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Blocked_After_ReStartUp_When_Was_In_Blocked_State_Before_ShutDown()
    {
      var scheduler = new SpyScheduler();
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.IsWorkingTimeDependent = true;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;


      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      sut.IsInState("Processing");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<BlockedEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Idle_After_ReStartUp_When_Was_Idle_Before_Shutdown()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.IsWorkingTimeDependent = true;


      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Not_Available_When_Shift_Is_Not_Started()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.IsWorkingTimeDependent = true;

      sut.Initialize();

      Assert.IsFalse(sut.IsAvailable(productA1));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Process_One_TranformationRule()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();

      var productA1 = MockProduct(modelMock, productTypeA);
      var productA2 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);
      var productB2 = MockProduct(modelMock, productTypeB);

      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();


      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);

      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(5);
      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(productB1))
                   .IgnoreArguments()
                   .Return(false)
                   .Repeat.Twice();
      successorMock.Expect(se => se.IsAvailable(productA1))
                   .IgnoreArguments()
                   .Return(true)
                   .Repeat.Times(4);

      successorMock.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      _entities.Add(successorMock);
      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;

      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.Add(connectionMock);
      sut.AddTransformationRule(transformationRuleMock);
      Assert.IsFalse(sut.IsAvailable(productA1));
      sut.Initialize();

      Assert.IsTrue(sut.IsAvailable(productA1));
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();

      sut.Receive(productA1);
      Assert.IsFalse(sut.IsAvailable(productA2));
      Assert.IsTrue(sut.IsAvailable(productB1));
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));

      scheduler.ProcessNextSchedulable();

      sut.Receive(productB1);
      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));

      scheduler.ProcessNextSchedulable();
      Assert.IsFalse(sut.IsAvailable(productA2));
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();

      eventRaiser.Raise(sut.Connections.First(), EventArgs.Empty);
      Assert.IsTrue(sut.IsAvailable(productA2));
      sut.IsInState("Idle");
      Assert.AreEqual(4, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductTransmitEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductTransmitEvent>(productTypeD));
      Assert.AreEqual(1, scheduler.OfType<BlockedEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.Receive(productB2);
      Assert.IsTrue(sut.IsAvailable(productA2));
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB2));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Remove_TransformationRule()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      sut.AddTransformationRule(transformationRuleMock);
      sut.RemoveTransformationRule(transformationRuleMock);
      Assert.IsEmpty(sut.TransformationRules.ToArray());
    }

    [Test]
    public void Stay_Failed_When_ShutDown_And_Switches_To_Idle_After_ReStartUp_And_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(20);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.IsWorkingTimeDependent = true;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.CanFail = true;
      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      sut.OnWorkingTimeEnded();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      sut.OnWorkingTimeStarted();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_Failed_When_ShutDown_And_Switches_To_Off_After_Failed_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);
      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(20);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.IsWorkingTimeDependent = true;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.CanFail = true;

      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      sut.OnWorkingTimeEnded();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<FailureStartEvent>()
                                  .Count());
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_When_ShutDown_And_Switches_To_Blocked_After_ReStartUp_And_Processing_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);
      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.IsWorkingTimeDependent = true;
      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      sut.OnWorkingTimeStarted();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_When_ShutDown_And_Switches_To_Off_After_Processing_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var inputAMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var inputBMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputCMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputDMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productTypeC = _mockRepo.DynamicMock<IProductType>();
      var productTypeD = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputAMock, inputBMock
                                    });
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputCMock, outputDMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);

      inputAMock.Expect(m => m.ProductType)
                .Return(productTypeA);
      inputAMock.Expect(m => m.Amount)
                .Return(1);
      inputBMock.Expect(m => m.ProductType)
                .Return(productTypeB);
      inputBMock.Expect(m => m.Amount)
                .Return(1);

      outputCMock.Expect(m => m.ProductType)
                 .Return(productTypeC);
      outputCMock.Expect(m => m.Amount)
                 .Return(1);
      outputDMock.Expect(m => m.ProductType)
                 .Return(productTypeD);
      outputDMock.Expect(m => m.Amount)
                 .Return(1);
      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);
      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      var empiricalIntDistribution2Mock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      empiricalIntDistribution2Mock.Expect(d => d.TryAddEntry(0, 0))
                                   .IgnoreArguments()
                                   .Return(true);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      _mockRepo.ReplayAll();

      _entities.Add(productTypeA);
      _entities.Add(productTypeB);
      _entities.Add(productTypeC);
      _entities.Add(productTypeD);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.TransformationSelectionDistribution = empiricalIntDistribution2Mock;
      sut.IsWorkingTimeDependent = true;
      sut.Initialize();

      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<IdleStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productA1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(productB1));
      Assert.AreEqual(1, scheduler.OfType<IdleEndEvent>()
                                  .Count());

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeC));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeD));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.OfType<OffEndEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.OfType<BlockedStartEvent>()
                                  .Count());
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Uses_TransformationRule_Specific_Resources()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var inputMock = new Mock<IProductTypeAmount>();
      var outputMock = new Mock<IProductTypeAmount>();
      var transformationRuleMock = new Mock<ITransformationRule>();
      var transformationRuleOutputMock = new Mock<ITransformationRuleOutput>();
      var productTypeMock = new Mock<IProductType>();
      var transformationSelectionDistributionMock = new Mock<IEmpiricalIntDistribution>();

      transformationSelectionDistributionMock.Setup(tsd => tsd.TryAddEntry(0, 1))
                                             .Returns(true);

      transformationRuleMock.Setup(trm => trm.Probability)
                            .Returns(100);

      transformationRuleMock.Setup(trm => trm.Inputs)
                            .Returns(new[]
                                     {
                                       inputMock.Object
                                     });
      transformationRuleMock.Setup(trm => trm.GetSampleOutput())
                            .Returns(transformationRuleOutputMock.Object);
      transformationRuleOutputMock.Setup(trm => trm.Outputs)
                                  .Returns(new[]
                                           {
                                             outputMock.Object
                                           });
      transformationRuleOutputMock.Setup(trm => trm.Distribution)
                                  .Returns(processingDurationDistributionMock.Object);
      inputMock.Setup(m => m.ProductType)
               .Returns(productTypeMock.Object);
      inputMock.Setup(m => m.Amount)
               .Returns(1);
      outputMock.Setup(m => m.ProductType)
                .Returns(productTypeMock.Object);
      outputMock.Setup(m => m.Amount)
                .Returns(1);
      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      var product = new Mock<Product>(modelMock.Object, productTypeMock.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);

      product.Setup(p => p.ProductType)
             .Returns(productTypeMock.Object);

      resourceTypeAmount.Setup(rta => rta.Amount)
                        .Returns(5);
      resourceTypeAmount.Setup(rta => rta.ResourceType)
                        .Returns(resourceTypeMock.Object);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });

      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 5))
                      .Returns(true);

      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 5))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object,
                                 resource.Object,
                                 resource.Object,
                                 resource.Object,
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.TransformationSelectionDistribution = transformationSelectionDistributionMock.Object;
      sut.Model = modelMock.Object;
      sut.AddTransformationRule(transformationRuleMock.Object);
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });
      sut.TransformationRuleSpecificProcessingResourcesDictionary.Add(transformationRuleOutputMock.Object, new Dictionary<IResourcePool, IDictionary<IResourceType, int>>
                                                                                                           {
                                                                                                             {
                                                                                                               resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                                                                                        {
                                                                                                                                          {
                                                                                                                                            resourceTypeMock.Object, 5
                                                                                                                                          }
                                                                                                                                        }
                                                                                                             }
                                                                                                           });

      sut.IsNotAvailableFor(product.Object);

      sut.Initialize();

      sut.IsAvailableFor(product.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product.Object);

      sut.IsInState("Processing");
      Assert.AreEqual(5, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product.Object));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(product.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeMock.Object));
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());
    }

    [Test]
    public void Enter_Processing_When_Resources_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var inputMock = new Mock<IProductTypeAmount>();
      var outputMock = new Mock<IProductTypeAmount>();
      var transformationRuleMock = new Mock<ITransformationRule>();
      var transformationRuleOutputMock = new Mock<ITransformationRuleOutput>();
      var productTypeMock = new Mock<IProductType>();
      var product = new Mock<Product>(modelMock.Object, productTypeMock.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);
      var transformationSelectionDistributionMock = new Mock<IEmpiricalIntDistribution>();

      transformationSelectionDistributionMock.Setup(tsd => tsd.TryAddEntry(0, 1))
                                             .Returns(true);

      transformationRuleMock.Setup(trm => trm.Probability)
                            .Returns(100);

      transformationRuleMock.Setup(trm => trm.Inputs)
                            .Returns(new[]
                                     {
                                       inputMock.Object
                                     });
      transformationRuleMock.Setup(trm => trm.GetSampleOutput())
                            .Returns(transformationRuleOutputMock.Object);
      transformationRuleOutputMock.Setup(trm => trm.Outputs)
                                  .Returns(new[]
                                           {
                                             outputMock.Object
                                           });
      transformationRuleOutputMock.Setup(trm => trm.Distribution)
                                  .Returns(processingDurationDistributionMock.Object);
      inputMock.Setup(m => m.ProductType)
               .Returns(productTypeMock.Object);
      inputMock.Setup(m => m.Amount)
               .Returns(1);
      outputMock.Setup(m => m.ProductType)
                .Returns(productTypeMock.Object);
      outputMock.Setup(m => m.Amount)
                .Returns(1);
      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      product.Setup(p => p.ProductType)
             .Returns(productTypeMock.Object);

      resourceTypeAmount.Setup(rta => rta.Amount)
                        .Returns(5);
      resourceTypeAmount.Setup(rta => rta.ResourceType)
                        .Returns(resourceTypeMock.Object);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });

      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(true);

      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 1))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.TransformationSelectionDistribution = transformationSelectionDistributionMock.Object;
      sut.AddTransformationRule(transformationRuleMock.Object);
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });

      sut.IsNotAvailableFor(product.Object);

      sut.Initialize();

      sut.IsAvailableFor(product.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product.Object);

      sut.IsInState("Processing");
      Assert.AreEqual(5, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product.Object));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(product.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeMock.Object));
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());
    }

    [Test]
    public void Enter_Processing_When_Resources_Got_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var inputMock = new Mock<IProductTypeAmount>();
      var outputMock = new Mock<IProductTypeAmount>();
      var transformationRuleMock = new Mock<ITransformationRule>();
      var transformationRuleOutputMock = new Mock<ITransformationRuleOutput>();
      var productTypeMock = new Mock<IProductType>();
      var product = new Mock<Product>(modelMock.Object, productTypeMock.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);
      var transformationSelectionDistributionMock = new Mock<IEmpiricalIntDistribution>();

      transformationSelectionDistributionMock.Setup(tsd => tsd.TryAddEntry(0, 1))
                                             .Returns(true);

      transformationRuleMock.Setup(trm => trm.Probability)
                            .Returns(100);

      transformationRuleMock.Setup(trm => trm.Inputs)
                            .Returns(new[]
                                     {
                                       inputMock.Object
                                     });
      transformationRuleMock.Setup(trm => trm.GetSampleOutput())
                            .Returns(transformationRuleOutputMock.Object);
      transformationRuleOutputMock.Setup(trm => trm.Outputs)
                                  .Returns(new[]
                                           {
                                             outputMock.Object
                                           });
      transformationRuleOutputMock.Setup(trm => trm.Distribution)
                                  .Returns(processingDurationDistributionMock.Object);
      inputMock.Setup(m => m.ProductType)
               .Returns(productTypeMock.Object);
      inputMock.Setup(m => m.Amount)
               .Returns(1);
      outputMock.Setup(m => m.ProductType)
                .Returns(productTypeMock.Object);
      outputMock.Setup(m => m.Amount)
                .Returns(1);
      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      product.Setup(p => p.ProductType)
             .Returns(productTypeMock.Object);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });
      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(false);
      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 1))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.TransformationSelectionDistribution = transformationSelectionDistributionMock.Object;
      sut.AddTransformationRule(transformationRuleMock.Object);
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });


      sut.IsNotAvailableFor(product.Object);

      sut.Initialize();

      sut.IsAvailableFor(product.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product.Object);

      sut.IsInState("Awaiting Processing Resources");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product.Object));
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      resourcePoolMock.Setup(r => r.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(true);

      resourcePoolMock.Raise(rp => rp.ResourcesReceived += null, EventArgs.Empty);

      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());

      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<TransformationStartEvent>(product.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeMock.Object));
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());
    }

    [Test]
    public void Stay_In_Awaiting_Resources_When_Resources_Not_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var inputMock = new Mock<IProductTypeAmount>();
      var outputMock = new Mock<IProductTypeAmount>();
      var transformationRuleMock = new Mock<ITransformationRule>();
      var transformationRuleOutputMock = new Mock<ITransformationRuleOutput>();
      var productTypeMock = new Mock<IProductType>();
      var product = new Mock<Product>(modelMock.Object, productTypeMock.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var transformationSelectionDistributionMock = new Mock<IEmpiricalIntDistribution>();

      transformationSelectionDistributionMock.Setup(tsd => tsd.TryAddEntry(0, 1))
                                             .Returns(true);

      transformationRuleMock.Setup(trm => trm.Probability)
                            .Returns(100);

      transformationRuleMock.Setup(trm => trm.Inputs)
                            .Returns(new[]
                                     {
                                       inputMock.Object
                                     });
      transformationRuleMock.Setup(trm => trm.GetSampleOutput())
                            .Returns(transformationRuleOutputMock.Object);
      transformationRuleOutputMock.Setup(trm => trm.Outputs)
                                  .Returns(new[]
                                           {
                                             outputMock.Object
                                           });
      transformationRuleOutputMock.Setup(trm => trm.Distribution)
                                  .Returns(processingDurationDistributionMock.Object);
      inputMock.Setup(m => m.ProductType)
               .Returns(productTypeMock.Object);
      inputMock.Setup(m => m.Amount)
               .Returns(1);
      outputMock.Setup(m => m.ProductType)
                .Returns(productTypeMock.Object);
      outputMock.Setup(m => m.Amount)
                .Returns(1);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      product.Setup(p => p.ProductType)
             .Returns(productTypeMock.Object);

      resourcePoolMock.Setup(r => r.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(false);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.TransformationSelectionDistribution = transformationSelectionDistributionMock.Object;
      sut.Model = modelMock.Object;
      sut.AddTransformationRule(transformationRuleMock.Object);
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });

      sut.IsNotAvailableFor(product.Object);

      sut.Initialize();

      sut.IsAvailableFor(product.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product.Object);

      sut.IsInState("Awaiting Processing Resources");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product.Object));
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
    }

    [Test]
    public void While_Processing_Failure_Return_Resources()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var inputMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var outputMock = _mockRepo.DynamicMock<IProductTypeAmount>();
      var transformationRuleMock = _mockRepo.DynamicMock<ITransformationRule>();
      var transformationRuleOutputMock = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      var resourcePoolMock = _mockRepo.DynamicMock<IResourcePool>();
      var resourceTypeMock = _mockRepo.DynamicMock<IResourceType>();

      var influenceMock = _mockRepo.DynamicMock<IInfluence>();
      var rtiMock = _mockRepo.DynamicMock<IResourceTypeInfluence>();
      resourceTypeMock.Expect(rtm => rtm.Influences)
                      .Return(new[]
                              {
                                rtiMock
                              });
      rtiMock.Expect(rtim => rtim.Influence)
             .Return(influenceMock);

      _mockRepo.ReplayAll();

      var del = new Func<double>(() => 0);
      var resource = _mockRepo.DynamicMock<Resource>(resourceTypeMock, resourcePoolMock, del);
      var transformationSelectionDistributionMock = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();

      transformationSelectionDistributionMock.Expect(tsd => tsd.TryAddEntry(0, 1))
                                             .Return(true);

      transformationRuleMock.Expect(trm => trm.Probability)
                            .Return(100);

      var product = MockProduct(modelMock, productTypeMock);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);

      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);

      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      resourcePoolMock.Expect(r => r.HasAvailable(resourceTypeMock, 1))
                      .Return(true);
      resourcePoolMock.Expect(x => x.GetResources(resourceTypeMock, 1))
                      .Return(new[]
                              {
                                resource
                              });

      transformationRuleMock.Expect(trm => trm.Inputs)
                            .Return(new[]
                                    {
                                      inputMock
                                    });
      transformationRuleMock.Expect(trm => trm.GetSampleOutput())
                            .Return(transformationRuleOutputMock);
      transformationRuleOutputMock.Expect(trm => trm.Outputs)
                                  .Return(new[]
                                          {
                                            outputMock
                                          });
      transformationRuleOutputMock.Expect(trm => trm.Distribution)
                                  .Return(processingDurationDistributionMock);
      inputMock.Expect(m => m.ProductType)
               .Return(productTypeMock);
      inputMock.Expect(m => m.Amount)
               .Return(1);
      outputMock.Expect(m => m.ProductType)
                .Return(productTypeMock);
      outputMock.Expect(m => m.Amount)
                .Return(1);

      _entities.Add(resourcePoolMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.TransformationSelectionDistribution = transformationSelectionDistributionMock;
      sut.AddTransformationRule(transformationRuleMock);
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.IsNotAvailableFor(product);

      sut.ProcessingResourcesDictionary.Add(resourcePoolMock, new Dictionary<IResourceType, int>
                                                              {
                                                                {
                                                                  resourceTypeMock, 1
                                                                }
                                                              });

      sut.Initialize();
      sut.IsAvailableFor(product);
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product);

      Assert.IsTrue(sut.AvailableProcessingResources.Count() == 1);
      Assert.IsTrue(sut.AvailableProcessingResources.Any(apr => apr.ResourceType == resourceTypeMock));

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<TransformationEndEvent>(productTypeMock));
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);

      sut.IsInState("Failure");

      Assert.IsTrue(!sut.AvailableProcessingResources.Any());

      Assert.AreEqual(4, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsDestroyedEvent>(productTypeMock));

      _mockRepo.VerifyAll();
    }
  }
}