#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using Emporer.Unit;
using GeWISSEN.TestUtils;
using Milan.JsonStore.Tests;
using NUnit.Framework;
using System;
using System.Linq;

namespace Emporer.Material.Tests
{
  [TestFixture]
  public class MaterialFacts : DomainEntityFixture<Material>
  {
    protected override Material CreateSUT()
    {
      return new Material();
    }

    private IMaterial SUT { get; set; }

    private void WhenSutIsCreated()
    {
      SUT = new Material();
    }

    [Test]
    public void Add_ContainedMaterial()
    {
      var givenContainedMaterial = Given.Some<IContainedMaterial>();
      var givenMaterialInContained = Given.Some<IMaterial>().Object;
      givenContainedMaterial.Setup(m => m.Material)
          .Returns(givenMaterialInContained);

      WhenSutIsCreated();

      SUT.Add(givenContainedMaterial.Object);
      Assert.IsNotEmpty(SUT.ContainedMaterials.ToArray());
      Assert.Contains(givenContainedMaterial.Object, SUT.ContainedMaterials.ToArray());
      Assert.True(SUT.Contains(givenMaterialInContained));
    }

    [Test]
    public void Add_Property()
    {
      WhenSutIsCreated();
      var givenMaterialProperty = Given.Some<IMaterialProperty>().Object;

      SUT.Add(givenMaterialProperty);

      Assert.IsNotEmpty(SUT.Properties.ToArray());
      Assert.Contains(givenMaterialProperty, SUT.Properties.ToArray());
    }

    [Test]
    public void Default_Ctor()
    {
      WhenSutIsCreated();
      Assert.AreEqual(0, SUT.Price);
      Assert.True(string.IsNullOrEmpty(SUT.Name));
      Assert.True(string.IsNullOrEmpty(SUT.Description));
      Assert.IsNull(SUT.Currency);
      Assert.IsNull(SUT.OwnUnit);
      Assert.IsNull(SUT.DisplayUnit);
      Assert.IsEmpty(SUT.Properties.ToList());
      Assert.IsEmpty(SUT.ContainedMaterials.ToList());
      Assert.IsEmpty(SUT.Categories.ToList());
    }

    [Test]
    public void Do_Not_Fail_On_Removing_NonExisting_ContainedMaterial()
    {
      WhenSutIsCreated();
      var mock = Given.Some<IContainedMaterial>().Object;

      SUT.Remove(mock);
    }

    [Test]
    public void Do_Not_Fail_On_Removing_NonExisting_Property()
    {
      WhenSutIsCreated();
      var mock = Given.Some<IMaterialProperty>().Object;

      SUT.Remove(mock);
    }

    [Test]
    public void Fail_On_Add_Two_ContainedMaterials_With_Same_Material()
    {
      WhenSutIsCreated();
      var mock = Given.Some<IContainedMaterial>();
      var mock2 = Given.Some<IContainedMaterial>();
      var matMock = Given.Some<IMaterial>().Object;
      mock.Setup(m => m.Material)
          .Returns(matMock);
      mock2.Setup(m => m.Material)
           .Returns(matMock);
      SUT.Add(mock.Object);

      Assert.Throws<ArgumentException>(() => SUT.Add(mock2.Object));
    }

    [Test]
    public void Remove_Existing_ContainedMaterial()
    {
      WhenSutIsCreated();
      var mock = Given.Some<IContainedMaterial>().Object;
      SUT.Add(mock);

      SUT.Remove(mock);

      Assert.IsEmpty(SUT.Categories.ToArray());
    }

    [Test]
    public void Remove_Existing_Property()
    {
      WhenSutIsCreated();
      var categoryMock = Given.Some<IMaterialProperty>().Object;
      SUT.Add(categoryMock);

      SUT.Remove(categoryMock);

      Assert.IsEmpty(SUT.Properties.ToArray());
    }

    [Test]
    public void Set_Currency()
    {
      WhenSutIsCreated();
      var givenCurrency = Given.Some<IUnit>().Object;

      SUT.Currency = givenCurrency;

      Assert.AreEqual(givenCurrency, SUT.Currency);
    }

    [Test]
    public void Set_DisplayUnit()
    {
      WhenSutIsCreated();
      var testValue = Given.Some<IUnit>().Object;

      SUT.DisplayUnit = testValue;

      Assert.AreEqual(testValue, SUT.DisplayUnit);
    }

    [Test]
    public void Set_Doubles()
    {
      WhenSutIsCreated();
      var testValue = 5;

      SUT.Price = testValue;

      Assert.AreEqual(testValue, SUT.Price);
    }

    [Test]
    public void Set_OwnUnit()
    {
      WhenSutIsCreated();
      var testValue = Given.Some<IUnit>().Object;

      SUT.OwnUnit = testValue;

      Assert.AreEqual(testValue, SUT.OwnUnit);
    }

    [Test]
    public void Set_Strings()
    {
      WhenSutIsCreated();
      var testValue = "Test";

      SUT.Name = testValue;
      SUT.Description = testValue;

      Assert.AreEqual(testValue, SUT.Description);
      Assert.AreEqual(testValue, SUT.Name);
    }

    [Test]
    public void Add_Category()
    {
      WhenSutIsCreated();
      var categoryMock = Given.Some<ICategory>().Object;

      SUT.Add(categoryMock);

      Assert.IsNotEmpty(SUT.Categories);
      Assert.AreEqual(1, SUT.Categories.Count());
      Assert.IsTrue(SUT.Categories.Contains(categoryMock));
    }

    [Test]
    public void Do_Not_Fail_On_Removing_NonExisting_Category()
    {
      WhenSutIsCreated();
      var categoryMock = Given.Some<ICategory>().Object;
      SUT.Remove(categoryMock);
    }

    [Test]
    public void Fail_On_Add_Null_Category()
    {
      WhenSutIsCreated();
      ICategory category = null;
      Assert.Throws<ArgumentNullException>(() => SUT.Add(category));
    }

    [Test]
    public void Fail_On_Removing_Null_Category()
    {
      WhenSutIsCreated();
      ICategory nullCategory = null;

      Assert.Throws<ArgumentNullException>(() => SUT.Remove(nullCategory));
    }

    [Test]
    public void Remove_Existing_Category()
    {
      WhenSutIsCreated();
      var categoryMock = Given.Some<ICategory>().Object;
      SUT.Add(categoryMock);

      SUT.Remove(categoryMock);

      Assert.IsEmpty(SUT.Categories);
    }
  }
}