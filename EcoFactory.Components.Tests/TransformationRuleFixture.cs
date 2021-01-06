#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.JsonStore.Tests;
using Milan.Simulation;

using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  internal class TransformationRuleFixture : DomainEntityFixture<TransformationRule>
  {
    [SetUp]
    public void SetUp()
    {
      _mockRepo = new MockRepository();
    }

    [TearDown]
    public void TearDown()
    {
    }

    private MockRepository _mockRepo;

    protected override TransformationRule CreateSUT()
    {
      return new TransformationRule();
    }

    [Test]
    public void Add_Input()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmount = _mockRepo.DynamicMock<IProductTypeAmount>();
      sut.AddInput(productTypeAmount);
      Assert.Contains(productTypeAmount, sut.Inputs.ToArray());
      Assert.AreEqual(1, sut.Inputs.Count());
    }

    [Test]
    public void Add_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleOutput = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      sut.AddOutput(transformationRuleOutput);
      Assert.Contains(transformationRuleOutput, sut.Outputs.ToArray());
      Assert.AreEqual(1, sut.Outputs.Count());
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateSUT();

      Assert.IsNull(sut.Distribution);
      Assert.IsEmpty(sut.Inputs.ToArray());
      Assert.IsEmpty(sut.Outputs.ToArray());
      Assert.AreEqual(0, sut.Probability);
    }

    [Test]
    public void Does_Not_Fail_On_Add_Output_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleOutput = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      sut.AddOutput(transformationRuleOutput);
      sut.AddOutput(transformationRuleOutput);
    }

    [Test]
    public void Fail_On_Add_Input_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmount = _mockRepo.DynamicMock<IProductTypeAmount>();
      sut.AddInput(productTypeAmount);
      Assert.Throws<InvalidOperationException>(() => sut.AddInput(productTypeAmount));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Input()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmount = _mockRepo.DynamicMock<IProductTypeAmount>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveInput(productTypeAmount));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleOutput = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveOutput(transformationRuleOutput));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Input()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductTypeAmount productTypeAmount = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveInput(productTypeAmount));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      ITransformationRuleOutput transformationRuleOutput = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveOutput(transformationRuleOutput));
    }

    [Test]
    public void Fail_On_Set_TransformationRuleOutput_Probability_Higher_Than_One_Hundred()
    {
      var sut = CreateMinimalConfiguredSUT();
      var distribution = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var transformationRuleOutput1 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput2 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput3 = _mockRepo.DynamicMock<ITransformationRuleOutput>();

      transformationRuleOutput1.Expect(tro => tro.Probability)
                               .Return(50);
      transformationRuleOutput2.Expect(tro => tro.Probability)
                               .Return(1);
      transformationRuleOutput3.Expect(tro => tro.Probability)
                               .Return(50);
      distribution.Expect(d => d.TryAddEntry(0, 0))
                  .IgnoreArguments()
                  .Return(true);
      _mockRepo.ReplayAll();

      sut.AddOutput(transformationRuleOutput1);
      sut.AddOutput(transformationRuleOutput2);
      sut.AddOutput(transformationRuleOutput3);
      Assert.Contains(transformationRuleOutput1, sut.Outputs.ToArray());
      Assert.Contains(transformationRuleOutput2, sut.Outputs.ToArray());
      Assert.Contains(transformationRuleOutput3, sut.Outputs.ToArray());
      Assert.Throws<InvalidOperationException>(() => sut.Distribution = distribution);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Fail_On_Set_TransformationRuleOutput_Probability_Lower_Than_One_Hundred()
    {
      var sut = CreateMinimalConfiguredSUT();
      var distribution = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var transformationRuleOutput1 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput2 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput3 = _mockRepo.DynamicMock<ITransformationRuleOutput>();

      transformationRuleOutput1.Expect(tro => tro.Probability)
                               .Return(1);
      transformationRuleOutput2.Expect(tro => tro.Probability)
                               .Return(2);
      transformationRuleOutput3.Expect(tro => tro.Probability)
                               .Return(50);
      distribution.Expect(d => d.TryAddEntry(0, 0))
                  .IgnoreArguments()
                  .Return(true);
      _mockRepo.ReplayAll();

      sut.AddOutput(transformationRuleOutput1);
      sut.AddOutput(transformationRuleOutput2);
      sut.AddOutput(transformationRuleOutput3);
      Assert.Contains(transformationRuleOutput1, sut.Outputs.ToArray());
      Assert.Contains(transformationRuleOutput2, sut.Outputs.ToArray());
      Assert.Contains(transformationRuleOutput3, sut.Outputs.ToArray());
      Assert.Throws<InvalidOperationException>(() => sut.Distribution = distribution);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Fail_On_Set_TransformationRuleOutput_With_Distribution_Exception()
    {
      var sut = CreateMinimalConfiguredSUT();
      var distribution = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var transformationRuleOutput1 = _mockRepo.DynamicMock<ITransformationRuleOutput>();

      transformationRuleOutput1.Expect(tro => tro.Probability)
                               .Return(0);
      distribution.Expect(d => d.TryAddEntry(0, 0))
                  .IgnoreArguments()
                  .Return(false);
      _mockRepo.ReplayAll();

      sut.AddOutput(transformationRuleOutput1);
      Assert.Contains(transformationRuleOutput1, sut.Outputs.ToArray());
      Assert.Throws<InvalidOperationException>(() => sut.Distribution = distribution);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Get_Sample_Output()
    {
      var distribution = _mockRepo.DynamicMock<IEmpiricalIntDistribution>();
      var transformationRuleOutput1 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput2 = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      var transformationRuleOutput3 = _mockRepo.DynamicMock<ITransformationRuleOutput>();

      transformationRuleOutput1.Expect(tro => tro.Probability)
                               .Return(20);
      transformationRuleOutput2.Expect(tro => tro.Probability)
                               .Return(30);
      transformationRuleOutput3.Expect(tro => tro.Probability)
                               .Return(50);
      distribution.Expect(d => d.GetSample())
                  .Return(2);
      distribution.Expect(d => d.TryAddEntry(0, 0))
                  .IgnoreArguments()
                  .Return(true);
      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.AddOutput(transformationRuleOutput1);
      sut.AddOutput(transformationRuleOutput2);
      sut.AddOutput(transformationRuleOutput3);
      sut.Distribution = distribution;
      Assert.AreEqual(sut.Distribution, distribution);
      Assert.AreEqual(sut.GetSampleOutput(), transformationRuleOutput3);


      _mockRepo.VerifyAll();
    }

    [Test]
    public void Remove_Input()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmount = _mockRepo.DynamicMock<IProductTypeAmount>();
      sut.AddInput(productTypeAmount);
      sut.RemoveInput(productTypeAmount);
      Assert.IsEmpty(sut.Inputs.ToArray());
    }

    [Test]
    public void Remove_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var transformationRuleOutput = _mockRepo.DynamicMock<ITransformationRuleOutput>();
      sut.AddOutput(transformationRuleOutput);
      sut.RemoveOutput(transformationRuleOutput);
      Assert.IsEmpty(sut.Outputs.ToArray());
    }

    [Test]
    public void Set_Probability()
    {
      var value = 5;
      SetProperty("Probability", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Probability = v, s => s.Probability);
    }


    [Test]
    public void Set_Same_Probability_Does_Not_Raise_PropertyChanged()
    {
      var value = 4;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Probability = v);
    }
  }
}