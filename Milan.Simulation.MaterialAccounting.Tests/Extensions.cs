#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components;
using Emporer.Material;
using Emporer.Unit;
using Milan.Simulation.Events;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Milan.Simulation.Observers;
using Milan.Simulation.Statistics;
using Moq;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  public static class Extensions
  {
    public static void Add(this Mock<IModel> mModel, IEnumerable<Mock<IProcessMaterialObserver<IEntity>>> materialStatistics)
    {
      mModel.Setup(m => m.Observers)
            .Returns(materialStatistics.Select(m => m.Object));
    }

    public static void Add(this Mock<IModel> mModel, Mock<IProcessMaterialObserver<IEntity>> materialStatistic)
    {
      mModel.Setup(m => m.Observers)
            .Returns(new[]
                     {
                       materialStatistic.Object
                     });
    }

    /// <summary>
    ///   Adds mocked contained materials to a material mock using the given list of material mocks-value-pairs.
    /// </summary>
    /// <remarks>
    ///   Call this only once. Further calls won't add but overwrite previously added contained materials!
    /// </remarks>
    /// <param name="mMaterial"></param>
    /// <param name="containingMaterials">A list of KeyValuePairs, key is the contained material, value the amount.</param>
    /// <returns>The material mock which now contains the given contained materials.</returns>
    public static Mock<IMaterial> Add(this Mock<IMaterial> mMaterial, IEnumerable<KeyValuePair<IMaterial, double>> containingMaterials)
    {
      mMaterial.Setup(m => m.ContainedMaterials)
               .Returns(containingMaterials.Select(kvp =>
                                                   {
                                                     var mCm = new Mock<IContainedMaterial>(MockBehavior.Strict);
                                                     mCm.Setup(m => m.Material)
                                                        .Returns(kvp.Key);
                                                     mCm.Setup(m => m.Amount)
                                                        .Returns(kvp.Value);
                                                     return mCm.Object;
                                                   }));
      return mMaterial;
    }

    public static Mock<IMaterial> Add(this Mock<IMaterial> mMaterial, Mock<IMaterialProperty> mProperty)
    {
      mMaterial.Setup(m => m.Properties)
               .Returns(new[]
                        {
                          mProperty.Object
                        });
      return mMaterial;
    }


    public static void Add(this Mock<IMaterial> mModel, IMaterial containingMaterial, double amount)
    {
      var mCm = new Mock<IContainedMaterial>(MockBehavior.Strict);
      mCm.Setup(m => m.Material)
         .Returns(containingMaterial);
      mCm.Setup(m => m.Amount)
         .Returns(amount);

      mModel.Setup(m => m.ContainedMaterials)
            .Returns(new[]
                     {
                       mCm.Object
                     });
    }


    /// <summary>
    ///   Sets up the exit point at which this end event is occurs.
    /// </summary>
    /// <param name="endEvent">The end event.</param>
    /// <param name="exitPoint">The exit point.</param>
    /// <returns></returns>
    public static Mock<ThroughputEndEvent> At(this Mock<ThroughputEndEvent> endEvent, Mock<IExitPoint> exitPoint)
    {
      endEvent.Setup(e => e.Sender)
              .Returns(exitPoint.Object);
      return endEvent;
    }
    
    public static Mock<IBatch> CreateBatch(IEnumerable<Mock<IExperiment>> experimentMocks)
    {
      var mBatch = new Mock<IBatch>(MockBehavior.Strict);
      var mExperiments = experimentMocks.ToArray();
      var experiments = mExperiments.Select(m => m.Object);

      mBatch.Setup(b => b.GetEnumerator())
            .Returns(experiments.GetEnumerator);

      return mBatch;
    }

    public static Mock<IEntity> CreateEntity(string name)
    {
      var mEntity = new Mock<IEntity>(MockBehavior.Strict);
      mEntity.Setup(m => m.Name)
             .Returns(name);
      return mEntity;
    }

    public static Mock<IExperiment> CreateExperiment(this Mock<IModel> mModel)
    {
      var mExperiment = new Mock<IExperiment>(MockBehavior.Strict);
      mExperiment.Setup(m => m.Model)
                 .Returns(mModel.Object);
      return mExperiment;
    }

    public static Mock<IMaterial> CreateMaterial(string name)
    {
      var mMaterial = new Mock<IMaterial>(MockBehavior.Strict);
      mMaterial.Setup(m => m.Name)
               .Returns(name);

      mMaterial.Setup(m => m.ContainedMaterials)
               .Returns(new IContainedMaterial[0]);

      return mMaterial;
    }


    public static Mock<IProcessMaterialObserver<IEntity>> CreateObserver(IEntity entity,
                                                                         string processName,
                                                                         Mock<IMaterial> materialMock,
                                                                         Mock<IUnit> unitMock,
                                                                         double amount,
                                                                         bool isEnabled)
    {
      return CreateObserver(entity, processName, materialMock, unitMock, amount,
        isEnabled, null);
    }

    public static Mock<IProcessMaterialObserver<IEntity>> CreateObserver(IEntity entity,
                                                                         string processName,
                                                                         Mock<IMaterial> materialMock,
                                                                         Mock<IUnit> unitMock,
                                                                         double amount,
                                                                         bool isEnabled,
                                                                         string category)
    {
      var mMaterialObserver = new Mock<IProcessMaterialObserver<IEntity>>(MockBehavior.Strict);
      var mEntityObserver = mMaterialObserver.As<IEntityObserver>();


      mMaterialObserver.Setup(m => m.Entity)
                       .Returns(entity);

      mEntityObserver.Setup(m => m.Entity)
                     .Returns(entity);

      mMaterialObserver.Setup(m => m.Name)
                       .Returns(processName);
      //todo: this have to be parameterized
      mMaterialObserver.Setup(m => m.LossRatio)
                       .Returns(0);

      mMaterialObserver.Setup(m => m.Category)
                       .Returns(category);

      mMaterialObserver.Setup(m => m.IsEnabled)
                       .Returns(isEnabled);

      mMaterialObserver.WithMaterial(materialMock, unitMock, amount);
      mMaterialObserver.WhichHasAggregated(0d);

      return mMaterialObserver;
    }

    public static Mock<IUnit> CreateUnit(string u1)
    {
      var mUnit = new Mock<IUnit>(MockBehavior.Strict);

      mUnit.Setup(m => m.FromBaseUnit(It.IsAny<double>()))
           .Returns((double v) => v);
      mUnit.Setup(u => u.Symbol)
           .Returns(u1);

      return mUnit;
    }

    public static Mock<IProcessMaterialObserver<IEntity>> WhichHasAggregated(this Mock<IProcessMaterialObserver<IEntity>> observer, double amount)
    {
      var mValueStore = new List<MaterialPosition>();
      observer.Setup(o => o.BalancePositions)
              .Returns(mValueStore);
      return observer;
    }
    
    public static Mock<IProcessMaterialObserver<IEntity>> WithMaterial(this Mock<IProcessMaterialObserver<IEntity>> observer,
                                                                       Mock<IMaterial> material,
                                                                       Mock<IUnit> unit,
                                                                       double amount)
    {
      observer.Setup(m => m.Material)
              .Returns(material.Object);
      material.Setup(m => m.DisplayUnit)
              .Returns(unit.Object);
      observer.Setup(m => m.Unit)
              .Returns(unit.Object);
      observer.Setup(o => o.Amount)
              .Returns(amount);
      return observer;
    }

    public static Mock<ThroughputEndEvent> WithProducts(this Mock<ThroughputEndEvent> endEvent, IEnumerable<Mock<Product>> productMocks)
    {
      endEvent.Setup(e => e.Products)
              .Returns(productMocks.Select(p => p.Object));
      return endEvent;
    }
  }
}