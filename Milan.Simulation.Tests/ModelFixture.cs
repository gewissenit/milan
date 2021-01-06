#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Milan.JsonStore.Tests;
using Milan.Simulation.Observers;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class ModelFixture : DomainEntityFixture<Model>
  {
    [SetUp]
    public virtual void SetUp()
    {
      MockRepo = new MockRepository();
    }

    private MockRepository MockRepo;

    protected override Model CreateSUT()
    {
      return new Model();
    }

    [Test]
    public void Add_Entity()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<IEntity>();
      sut.Add(item);
      Assert.Contains(item, sut.Entities.ToArray());
      Assert.AreEqual(1, sut.Entities.Count());
    }

    [Test]
    public void Add_SimulationObserver()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<ISimulationObserver>();
      sut.Add(item);
      Assert.Contains(item, sut.Observers.ToArray());
      Assert.AreEqual(1, sut.Observers.Count());
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.NotNull(sut);
      Assert.True(string.IsNullOrEmpty(sut.Description));
      Assert.True(string.IsNullOrEmpty(sut.Name));
    }

    [Test]
    public void Fail_On_Add_Entity_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<IEntity>();
      sut.Add(item);
      Assert.Throws<InvalidOperationException>(() => sut.Add(item));
    }

    [Test]
    public void Fail_On_Add_SimulationObserver_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<ISimulationObserver>();
      sut.Add(item);
      Assert.Throws<InvalidOperationException>(() => sut.Add(item));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Entity()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<IEntity>();
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_SimulationObserver()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<ISimulationObserver>();
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Entity()
    {
      var sut = CreateMinimalConfiguredSUT();
      IEntity item = null;
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

// ReSharper disable ExpressionIsAlwaysNull
    [Test]
    public void Fail_On_Remove_Null_Fro_SimulationObserver()
    {
      var sut = CreateMinimalConfiguredSUT();
      ISimulationObserver item = null;
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

// ReSharper restore ExpressionIsAlwaysNull

    [Test]
    public void InitializeEntitiesAndObservers()
    {
      var entityMock = MockRepo.DynamicMock<IEntity>();
      var observerMock = MockRepo.DynamicMock<ISimulationObserver>();

      entityMock.Expect(en => en.Initialize());

      var sut = CreateMinimalConfiguredSUT();
      MockRepo.ReplayAll();

      sut.Add(entityMock);
      sut.Add(observerMock);

      sut.Initialize();

      MockRepo.VerifyAll();
    }

    [Test]
    public void Remove_Entity()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<IEntity>();
      sut.Add(item);
      sut.Remove(item);
      Assert.IsEmpty(sut.Entities.ToArray());
    }

    [Test]
    public void Remove_SimulationObserver()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = MockRepo.DynamicMock<ISimulationObserver>();
      sut.Add(item);
      sut.Remove(item);
      Assert.IsEmpty(sut.Observers.ToArray());
    }
    
    [Test]
    public void Set_Name()
    {
      var value = "model";
      SetProperty("Name", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Name = v, s => s.Name);
    }

    [Test]
    public void Set_Same_Name_Does_Not_Raise_PropertyChanged()
    {
      var value = "model";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Name = v);
    }

    [Test]
    public void Set_Same_Description_Does_Not_Raise_PropertyChanged()
    {
      var value = "short description";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Description = v);
    }

    [Test]
    public void Set_Description()
    {
      var value = "short description";
      SetProperty("Description", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Description = v, s => s.Description);
    }
  }

// ReSharper restore InconsistentNaming
}