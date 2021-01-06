#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Emporer.Unit.Tests
{
  [TestFixture]
  public class UnitFixture : DomainEntityFixture<Unit>
  {
    protected override Unit CreateSUT()
    {
      return new Unit();
    }


    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.AreEqual(0.0, sut.Coefficient);
      Assert.True(string.IsNullOrEmpty(sut.Name));
      Assert.True(string.IsNullOrEmpty(sut.Dimension));
      Assert.True(string.IsNullOrEmpty(sut.Symbol));
      Assert.IsNull(sut.ReferencedUnit);
      Assert.IsTrue(sut.IsBasicUnit);
      Assert.IsFalse(sut.IsReadonly);
    }

    [Test]
    public void FromBaseUnit___Converts_Value_With_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      sut.Coefficient = 2;

      var mock = MockRepository.GenerateMock<IUnit>();
      sut.ReferencedUnit = mock;

      var factor = 10;
      var result = 5;
      mock.Expect(m => m.FromBaseUnit(result))
          .Return(result);
      Assert.AreEqual(result, sut.FromBaseUnit(factor));
    }

    [Test]
    public void FromBaseUnit___Converts_Value_Without_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      sut.Coefficient = 2;

      var factor = 5;
      var result = 5;

      Assert.AreEqual(result, sut.FromBaseUnit(factor));
    }

    [Test]
    public void Get_As_Basic_Value_Without_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var coefficient = 2;
      sut.Coefficient = coefficient;
      var factor = 5;
      var result = factor;

      Assert.AreEqual(result, sut.ToBaseUnit(factor));
    }

    [Test]
    public void Get_Basic_Unit_With_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var mock = MockRepository.GenerateMock<IUnit>();
      sut.ReferencedUnit = mock;
      mock.Expect(m => m.BasicUnit)
          .Return(mock);

      var result = sut.BasicUnit;

      Assert.AreEqual(mock, result);
    }

    [Test]
    public void Get_Basic_Unit_Without_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var result = sut.BasicUnit;

      Assert.AreEqual(sut, result);
    }

    [Test]
    public void Get_Basic_Value_With_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var mock = MockRepository.GenerateMock<IUnit>();
      sut.ReferencedUnit = mock;
      var coefficient = 2;
      sut.Coefficient = coefficient;
      var value = 5;
      var result = value * coefficient;
      mock.Expect(m => m.ToBaseUnit(result))
          .Return(result);

      Assert.AreEqual(result, sut.ToBaseUnit(value));
    }

    [Test]
    public void Get_Is_Convertable_To_Referenced_Unit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var mock = MockRepository.GenerateMock<IUnit>();
      sut.ReferencedUnit = mock;

      Assert.True(sut.IsConvertibleTo(sut.ReferencedUnit));
    }


    [Test]
    public void Set_Coefficient()
    {
      var value = 1.0;
      SetProperty("Coefficient", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Coefficient = v, s => s.Coefficient);
    }


    [Test]
    public void Set_Coefficient_Twice()
    {
      var value = 1.0;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Coefficient = v);
    }


    [Test]
    public void Set_Dimension()
    {
      var value = "dimension";
      SetProperty("Dimension", value, Assert.AreEqual, (s, v) => s.Dimension = v, s => s.Dimension);
    }


    [Test]
    public void Set_Dimension_Twice()
    {
      var value = "dimension";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Dimension = v);
    }

    [Test]
    public void Set_IsReadonly()
    {
      var sut = CreateMinimalConfiguredSUT();
      sut.IsReadonly = true;

      Assert.IsTrue(sut.IsReadonly);
    }

    [Test]
    public void Set_Name()
    {
      var value = "name";
      SetProperty("Name", value, Assert.AreEqual, (s, v) => s.Name = v, s => s.Name);
    }


    [Test]
    public void Set_Name_Twice()
    {
      var value = "name";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Name = v);
    }

    [Test]
    public void Set_ReferencedUnit()
    {
      var sut = CreateMinimalConfiguredSUT();

      var unitMock = MockRepository.GenerateMock<IUnit>();
      sut.ReferencedUnit = unitMock;
      Assert.NotNull(sut.ReferencedUnit);
      Assert.AreEqual(sut.ReferencedUnit, unitMock);
      Assert.IsFalse(sut.IsBasicUnit);
    }


    [Test]
    public void Set_Symbol()
    {
      var value = "symbol";
      SetProperty("Symbol", value, Assert.AreEqual, (s, v) => s.Symbol = v, s => s.Symbol);
    }


    [Test]
    public void Set_Symbol_Twice()
    {
      var value = "symbol";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Symbol = v);
    }


    [Test]
    public void Set_Unit_With_Null()
    {
      var sut = CreateMinimalConfiguredSUT();
      IUnit unit = null;
      sut.ReferencedUnit = unit;
      Assert.IsNull(sut.ReferencedUnit);
    }
  }
}