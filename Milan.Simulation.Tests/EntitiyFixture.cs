#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using Milan.JsonStore.Tests;
using NUnit.Framework;

namespace Milan.Simulation.Tests
{
  public abstract class EntityFixture<T> : DomainEntityFixture<T>
    where T : Entity
  {
    protected abstract void Default_Ctor(T sut);

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.NotNull(sut);
      Assert.IsEmpty(sut.ExtendingPropertyValues.ToArray());
      Assert.True(string.IsNullOrEmpty(sut.Description));
      Assert.True(string.IsNullOrEmpty(sut.Name));
      Assert.IsNull(sut.CurrentExperiment);
      Default_Ctor(sut);
    }

    [Test]
    public void Set_Name()
    {
      var value = "entity";
      SetProperty("Name", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Name = v, s => s.Name);
    }


    [Test]
    public void Set_Same_Name_Does_Not_Raise_PropertyChanged()
    {
      var value = "entity";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Name = v);
    }

    [Test]
    public void Set_Description()
    {
      var value = "short description";
      SetProperty("Description", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Description = v, s => s.Description);
    }


    [Test]
    public void Set_Same_Description_Does_Not_Raise_PropertyChanged()
    {
      var value = "short description";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Description = v);
    }
  }
}