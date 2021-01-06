#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Emporer.Material.Tests
{
  [TestFixture]
  internal class CategoryFixture : DomainEntityFixture<Category>
  {
    protected override Category CreateSUT()
    {
      return new Category();
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.AreEqual(null, sut.ParentCategory);
      Assert.True(string.IsNullOrEmpty(sut.Name));
      Assert.True(string.IsNullOrEmpty(sut.Description));
    }

    [Test]
    public void SetDescription()
    {
      var sut = CreateSUT();
      var description = "CategoryDescription";
      sut.Description = description;
      Assert.AreEqual(description, sut.Description);
    }

    [Test]
    public void SetName()
    {
      var sut = CreateSUT();
      var name = "TestCategory";
      sut.Name = name;
      Assert.AreEqual(name, sut.Name);
    }

    [Test]
    public void SetParentCategory()
    {
      var sut = CreateSUT();
      var parentCategory = MockRepository.GenerateMock<ICategory>();
      sut.ParentCategory = parentCategory;
      Assert.AreEqual(parentCategory, sut.ParentCategory);
    }
  }
}