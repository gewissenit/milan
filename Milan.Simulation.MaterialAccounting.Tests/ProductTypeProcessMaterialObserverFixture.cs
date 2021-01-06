#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components;
using Emporer.Material;
using Emporer.Unit;
using Milan.Simulation.Logging;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using Moq;
using NUnit.Framework;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  [TestFixture]
  public class ProductTypeProcessMaterialObserverFixture
  {
    private Mock<IUnit> _currency;
    private Mock<IExperiment> _experiment;

    private Mock<IExperimentLogWriterProvider> _logProvider;
    private Mock<ILogFileWriter> _logWriter;

    private Mock<IMaterial> _material;
    private Mock<IModel> _model;

    private Mock<IProductType> _ptAlpha;
    private Mock<IProductType> _ptBeta;

    private MockRepository _repo;
    private Mock<IScheduler> _scheduler;
    private Mock<IUnit> _unit;

    private Mock<IWorkstation> _ws;

    [SetUp]
    public void SetUp()
    {
      _repo = new MockRepository(MockBehavior.Loose);

      _material = new Mock<IMaterial>();

      _unit = new Mock<IUnit>();
      _currency = new Mock<IUnit>();

      _ws = _repo.Create<IWorkstation>();

      _ptAlpha = _repo.Create<IProductType>();
      _ptBeta = _repo.Create<IProductType>();

      _logProvider = _repo.Create<IExperimentLogWriterProvider>();
      _logWriter = _repo.Create<ILogFileWriter>();
      _experiment = _repo.Create<IExperiment>();
      _scheduler = _repo.Create<IScheduler>();
      _model = _repo.Create<IModel>();

      // setup
      _experiment.Setup(x => x.LogProvider)
                 .Returns(_logProvider.Object);
      _experiment.Setup(x => x.Scheduler)
                 .Returns(_scheduler.Object);
      _logProvider.Setup(x => x.GetLogger(_experiment.Object))
                  .Returns(_logWriter.Object);

      _material.Setup(x => x.DisplayUnit)
               .Returns(_unit.Object);
      _material.Setup(x => x.Currency)
               .Returns(_currency.Object);
      _unit.Setup(x => x.IsBasicUnit)
           .Returns(true);
      _unit.Setup(x => x.BasicUnit)
           .Returns(_unit.Object);
      _unit.Setup(x => x.ToBaseUnit(It.IsAny<double>()))
           .Returns((double v) => v);
    }


    [TearDown]
    public void TearDown()
    {
      _repo.VerifyAll();
      _repo = null;
      _scheduler = null;
    }

    private SUT CreateSut(IWorkstation entity, IMaterial material, IUnit unit, double amount, TimeReference timeRef, QuantityReference quantityRef, IProductType productType, bool isProducttypeSpecific)
    {
      var sut = new SUT();

      sut.Entity = entity;
      sut.Unit = unit;
      sut.Model = _model.Object;
      sut.Material = material;
      sut.IsProductTypeSpecific = isProducttypeSpecific;
      sut.QuantityReference = quantityRef;
      sut.TimeReference = timeRef;
      sut.Amount = amount;

      sut.Configure(_experiment.Object);

      sut.IsEnabled = true;

      sut.Initialize();
      return sut;
    }

    private void RaiseProcessingEnd(IEntity entity, IEnumerable<Product> products)
    {
      _scheduler.Raise(x => x.SchedulableHandled += null, new SchedulerEventArgs(new EndEventStub(entity, products, new StartEventStub(entity, products))));
    }

    private Mock<Product> CreateProduct(IProductType productType)
    {
      var product = _repo.Create<Product>(_model.Object, productType, 0d);
      product.Setup(x => x.ProductType)
             .Returns(productType);
      return product;
    }

    [Test]
    public void Correct_Observation_For_Once_PerBatch_NotProductTypeSpecific_If_Process_Occurred_Once_With_Multiple_Products_Of_Different_Type()
    {
      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerBatch, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      var p2 = CreateProduct(_ptBeta.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object, p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(0.5d, sut.BalancePositions.First()
                               .Total);
      Assert.AreEqual(0.5d, sut.BalancePositions.Last()
                               .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerBatch_NotProductTypeSpecific_If_Process_Occurred_Once_With_Multiple_Products_Of_Same_Type()
    {
      /* observer:
       *  not product specific (all products in the batch are affected)
       *  once per process (amount is not multiplied by process duration)
       *  per batch (amount is shared by all products)
       * 
       * scenario:
       *  2 products of the same type are processed together
       *  
       * checks:
       *  1 observation for stationary element, with full amount
       *  each product has corresponding experiment property, each with half amount       
       */

      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerBatch, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      var p2 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object, p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(0.5d, sut.BalancePositions.First()
                               .Total);
      Assert.AreEqual(0.5d, sut.BalancePositions.Last()
                               .Total);
    }


    [Test]
    public void Correct_Observation_For_Once_PerBatch_NotProductTypeSpecific_If_Process_Occurred_Once_With_One_Product()
    {
      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerBatch, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object
                                     });

      Assert.AreEqual(1, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerBatch_NotProductTypeSpecific_If_Process_Occurred_Twice_With_One_Product()
    {
      /* observer:
       *  not product specific (all products in the batch are affected)
       *  once per process (amount is not multiplied by process duration)
       *  per batch (amount is shared by all products)
       * 
       * scenario:
       *  2 products of the same type are processed independently
       *  
       * checks:
       *  2 observations for stationary element, both with full amount
       *  each product has corresponding experiment property, each with full amount 
       
       */

      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerBatch, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object
                                     });

      var p2 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
      Assert.AreEqual(1, sut.BalancePositions.Last()
                            .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerProduct_NotProductTypeSpecific_If_Process_Occurred_Once_With_Multiple_Products_Of_Same_Type()
    {
      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerProduct, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      var p2 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object, p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
      Assert.AreEqual(1, sut.BalancePositions.Last()
                            .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerProduct_NotProductTypeSpecific_If_Process_Occurred_Once_With_Multiple_Products_Of_Different_Type()
    {
      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerProduct, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      var p2 = CreateProduct(_ptBeta.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object, p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(1.0d, sut.BalancePositions.First()
                               .Total);
      Assert.AreEqual(1.0d, sut.BalancePositions.Last()
                               .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerProduct_NotProductTypeSpecific_If_Process_Occurred_Once_With_Multiple_Products_Of_The_Same_Type()
    {
      /* observer:
       *  not product specific (all products in the batch are affected)
       *  once per process (amount is not multiplied by process duration)
       *  per batch (amount is shared by all products)
       * 
       * scenario:
       *  2 products of the same type are processed together
       *  
       * checks:
       *  1 observation for stationary element, with double the observers amount
       *  each product has corresponding experiment property, each with the observers amount       
       */

      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerProduct, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      var p2 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object, p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
      Assert.AreEqual(1, sut.BalancePositions.Last()
                            .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerProduct_NotProductTypeSpecific_If_Process_Occurred_Once_With_One_Product()
    {
      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerProduct, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object
                                     });

      Assert.AreEqual(1, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
    }

    [Test]
    public void Correct_Observation_For_Once_PerProduct_NotProductTypeSpecific_If_Process_Occurred_Twice_With_One_Product()
    {
      /* observer:
       *  not product specific (all products in the batch are affected)
       *  once per process (amount is not multiplied by process duration)
       *  per batch (amount is shared by all products)
       * 
       * scenario:
       *  2 products of the same type are processed independently
       *  
       * checks:
       *  2 observations for stationary element, both with full amount
       *  each product has corresponding experiment property, each with full amount       
       */

      var sut = CreateSut(_ws.Object, _material.Object, _unit.Object, 1, TimeReference.Once, QuantityReference.PerProduct, null, false);

      var p1 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p1.Object
                                     });

      var p2 = CreateProduct(_ptAlpha.Object);
      RaiseProcessingEnd(_ws.Object, new[]
                                     {
                                       p2.Object
                                     });

      Assert.AreEqual(2, sut.BalancePositions.Count());
      Assert.AreEqual(1, sut.BalancePositions.First()
                            .Total);
      Assert.AreEqual(1, sut.BalancePositions.Last()
                            .Total);
    }
  }
}