#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Material;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Moq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.MaterialAccounting.Tests
{
  [TestFixture]
  public class UtilsFixture
  {
    private static Mock<IMaterialProperty> CreatePropertyProperty(Mock<IPropertyType> mPropertyType, double propertyFactor)
    {
      var mProperty = new Mock<IMaterialProperty>(MockBehavior.Strict);

      mProperty.Setup(m => m.PropertyType)
               .Returns(mPropertyType.Object);

      mProperty.Setup(m => m.Mean)
               .Returns(propertyFactor);
      return mProperty;
    }

    private static Mock<IPropertyType> CreatePropertyType(string propertyId, string propertyUnit)
    {
      var mPropertyType = new Mock<IPropertyType>(MockBehavior.Strict);

      mPropertyType.Setup(m => m.DataSourceId)
                   .Returns(propertyId);

      mPropertyType.Setup(m => m.Unit)
                   .Returns(propertyUnit);
      return mPropertyType;
    }

    private static Mock<MaterialPosition> CreateBatchBalance(Mock<IMaterial> material, double total, double loss)
    {
      var mBatchBalance = new Mock<MaterialPosition>(MockBehavior.Strict);

      mBatchBalance.Setup(m => m.Material)
                   .Returns(material.Object);

      mBatchBalance.Setup(m => m.Total)
                   .Returns(total);

      mBatchBalance.Setup(m => m.Loss)
                   .Returns(loss);
      return mBatchBalance;
    }


    [Test]
    public void CalculatePropertyValue___Correct_Value_With_Containing_Materials_Multiple_Levels_Deep()
    {
      const double loss = 2d;
      const double total = 4d;
      const string propertyId = "pID";
      const string propertyUnit = "unit";
      const double propertyFactorForMaterial = 1d;
      const double propertyFactorForContained1 = 1.1d;
      const double propertyFactorForContained2 = 1.2d;
      const double propertyFactorForContained3 = 1.3d;
      const double amountOfContained1InMaterial = 10d;
      const double amountOfContained2InContained1 = 100d;
      const double amountOfContained3InContained2 = 1000d;

      var mPropertyType = CreatePropertyType(propertyId, propertyUnit);

      var contained3 = Extensions.CreateMaterial("cm3")
                                 .Add(CreatePropertyProperty(mPropertyType, propertyFactorForContained3));

      var contained2 = Extensions.CreateMaterial("cm2")
                                 .Add(CreatePropertyProperty(mPropertyType, propertyFactorForContained2))
                                 .Add(new[]
                                      {
                                        new KeyValuePair<IMaterial, double>(contained3.Object, amountOfContained3InContained2)
                                      });

      var contained1 = Extensions.CreateMaterial("cm1")
                                 .Add(CreatePropertyProperty(mPropertyType, propertyFactorForContained1))
                                 .Add(new[]
                                      {
                                        new KeyValuePair<IMaterial, double>(contained2.Object, amountOfContained2InContained1)
                                      });


      var material = Extensions.CreateMaterial("m1")
                               .Add(CreatePropertyProperty(mPropertyType, propertyFactorForMaterial))
                               .Add(new[]
                                    {
                                      new KeyValuePair<IMaterial, double>(contained1.Object, amountOfContained1InMaterial)
                                    });

      var mBatchBalance = CreateBatchBalance(material, total, loss);

      var result = ReportDataProviders.Utils.GetImpactValueGroup(mPropertyType.Object, mBatchBalance.Object, new[]
                                                                                                             {
                                                                                                               mPropertyType.Object
                                                                                                             });

      var lossSum = loss * propertyFactorForMaterial;
      lossSum += loss * amountOfContained1InMaterial * propertyFactorForContained1;
      lossSum += loss * amountOfContained1InMaterial * amountOfContained2InContained1 * propertyFactorForContained2;
      lossSum += loss * amountOfContained1InMaterial * amountOfContained2InContained1 * amountOfContained3InContained2 * propertyFactorForContained3;

      var totalSum = total * propertyFactorForMaterial;
      totalSum += total * amountOfContained1InMaterial * propertyFactorForContained1;
      totalSum += total * amountOfContained1InMaterial * amountOfContained2InContained1 * propertyFactorForContained2;
      totalSum += total * amountOfContained1InMaterial * amountOfContained2InContained1 * amountOfContained3InContained2 * propertyFactorForContained3;

      Assert.AreEqual(totalSum, result[0]);
      Assert.AreEqual(lossSum, result[1]);
      Assert.AreEqual(propertyUnit, result[2]);
    }

    [Test]
    public void CalculatePropertyValue___Correct_Value_With_Containing_Materials_One_Level_Deep()
    {
      const double total = 2d;
      const double loss = 3d;
      const string propertyId = "pID";
      const string propertyUnit = "unit";
      const double propertyFactorForMaterial = 10d;
      const double propertyFactorForContained1 = 10d;
      const double propertyFactorForContained2 = 10d;
      const double amountOfContained1InMaterial = 2d;
      const double amountOfContained2InMaterial = 1000d;


      var mPropertyType = CreatePropertyType(propertyId, propertyUnit);

      var contained1 = Extensions.CreateMaterial("cm1")
                                 .Add(CreatePropertyProperty(mPropertyType, propertyFactorForContained1));

      var contained2 = Extensions.CreateMaterial("cm2")
                                 .Add(CreatePropertyProperty(mPropertyType, propertyFactorForContained2));

      var material = Extensions.CreateMaterial("m1")
                               .Add(CreatePropertyProperty(mPropertyType, propertyFactorForMaterial))
                               .Add(new[]
                                    {
                                      new KeyValuePair<IMaterial, double>(contained1.Object, amountOfContained1InMaterial), new KeyValuePair<IMaterial, double>(contained2.Object, amountOfContained2InMaterial)
                                    });

      var mBatchBalance = CreateBatchBalance(material, total, loss);


      var result = ReportDataProviders.Utils.GetImpactValueGroup(mPropertyType.Object, mBatchBalance.Object, new[]
                                                                                                             {
                                                                                                               mPropertyType.Object
                                                                                                             });

      var totalSum = total * propertyFactorForMaterial;
      totalSum += total * amountOfContained1InMaterial * propertyFactorForContained1;
      totalSum += total * amountOfContained2InMaterial * propertyFactorForContained2;

      var lossSum = loss * propertyFactorForMaterial;
      lossSum += loss * amountOfContained1InMaterial * propertyFactorForContained1;
      lossSum += loss * amountOfContained2InMaterial * propertyFactorForContained2;

      Assert.AreEqual(totalSum, result[0]);
      Assert.AreEqual(lossSum, result[1]);
      Assert.AreEqual(propertyUnit, result[2]);
    }


    [Test]
    public void CalculatePropertyValue___Correct_Value_Without_Containing_Materials()
    {
      const double propertyFactor = 10d;
      const double total = 2d;
      const double loss = 3d;
      const string propertyId = "pID";
      const string propertyUnit = "unit";

      var mPropertyType = CreatePropertyType(propertyId, propertyUnit);
      var mProperty = CreatePropertyProperty(mPropertyType, propertyFactor);
      var material = Extensions.CreateMaterial("m1")
                               .Add(mProperty);
      var mBatchBalance = CreateBatchBalance(material, total, loss);


      var result = ReportDataProviders.Utils.GetImpactValueGroup(mPropertyType.Object, mBatchBalance.Object, new[]
                                                                                                             {
                                                                                                               mPropertyType.Object
                                                                                                             });

      Assert.AreEqual(total * propertyFactor, result[0]);
      Assert.AreEqual(loss * propertyFactor, result[1]);
      Assert.AreEqual(propertyUnit, result[2]);
    }
  }
}

// ReSharper restore InconsistentNaming