using System;
using Emporer.Material.UI.ViewModels;
using Emporer.WPF.ViewModels;
using Moq;
using NUnit.Framework;

namespace Emporer.Material.Tests
{
  [TestFixture]
  public class OrderCategoriesBeforeMaterialsThenByNameAlphabeticallyFixture
  {
    [SetUp]
    public void SetUp()
    {
      _sut = new OrderCategoriesBeforeMaterialsThenByNameAlphabetically();
    }

    [TearDown]
    public void TearDown()
    {
      _sut = null;
    }

    private OrderCategoriesBeforeMaterialsThenByNameAlphabetically _sut;

    private IViewModel CreateMaterialViewModel(string name)
    {
      var materialMock = new Mock<IMaterial>();
      var materialViewModelMock = new Mock<IViewModel>();

      materialViewModelMock.Setup(x => x.Model)
                           .Returns(materialMock.Object);

      materialMock.Setup(x => x.Name)
                  .Returns(name);

      return materialViewModelMock.Object;
    }

    private IViewModel CreateCategoryViewModel(string name)
    {
      var categoryMock = new Mock<ICategory>();
      var categoryViewModelMock = new Mock<IViewModel>();

      categoryViewModelMock.Setup(x => x.Model)
                           .Returns(categoryMock.Object);

      categoryMock.Setup(x => x.Name)
                  .Returns(name);

      return categoryViewModelMock.Object;
    }

    [Test]
    public void Any_ViewModel_Is_Greater_Than_Null()
    {
      var viewModelMock = new Mock<IViewModel>();
      Assert.AreEqual(1, _sut.Compare(viewModelMock.Object, null));
    }


    [Test]
    public void Category_Is_Smaller_Than_Material()
    {
      var categoryMock = new Mock<ICategory>();
      var categoryViewModelMock = new Mock<IViewModel>();
      categoryViewModelMock.Setup(x => x.Model)
                           .Returns(categoryMock.Object);


      var materialMock = new Mock<IMaterial>();
      var materialViewModelMock = new Mock<IViewModel>();
      materialViewModelMock.Setup(x => x.Model)
                           .Returns(materialMock.Object);


      Assert.AreEqual(-1, _sut.Compare(categoryViewModelMock.Object, materialViewModelMock.Object));
      Assert.AreEqual(1, _sut.Compare(materialViewModelMock.Object, categoryViewModelMock.Object));
    }

    [Test]
    public void Compares_Categories_Ordinal()
    {
      var ma = CreateCategoryViewModel("");
      var mA = CreateCategoryViewModel("A");
      var maa = CreateCategoryViewModel("aa");


      Assert.IsTrue(_sut.Compare(ma, maa) < 0);
      Assert.IsTrue(_sut.Compare(mA, maa) < 0);
      Assert.IsTrue(_sut.Compare(ma, mA) < 0);

      Assert.AreEqual(0, _sut.Compare(ma, ma));
      Assert.AreEqual(0, _sut.Compare(maa, maa));
      Assert.AreEqual(0, _sut.Compare(mA, mA));

      Assert.IsTrue(_sut.Compare(maa, ma) > 0);
      Assert.IsTrue(_sut.Compare(maa, mA) > 0);
      Assert.IsTrue(_sut.Compare(mA, ma) > 0);
    }

    [Test]
    public void Compares_Materials_Ordinal()
    {
      var ma = CreateMaterialViewModel("");
      var mA = CreateMaterialViewModel("A");
      var maa = CreateMaterialViewModel("aa");


      Assert.IsTrue(_sut.Compare(ma, maa) < 0);
      Assert.IsTrue(_sut.Compare(mA, maa) < 0);
      Assert.IsTrue(_sut.Compare(ma, mA) < 0);

      Assert.AreEqual(0, _sut.Compare(ma, ma));
      Assert.AreEqual(0, _sut.Compare(maa, maa));
      Assert.AreEqual(0, _sut.Compare(mA, mA));

      Assert.IsTrue(_sut.Compare(maa, ma) > 0);
      Assert.IsTrue(_sut.Compare(maa, mA) > 0);
      Assert.IsTrue(_sut.Compare(mA, ma) > 0);
    }

    [Test]
    public void Null_Equals_Null()
    {
      Assert.AreEqual(0, _sut.Compare(null, null));
    }


    [Test]
    public void Null_Is_Smaller_Than_Any_ViewModel()
    {
      var viewModelMock = new Mock<IViewModel>();
      Assert.AreEqual(-1, _sut.Compare(null, viewModelMock.Object));
    }

    [Test]
    public void Same_Objects_Return_Zero()
    {
      var categoryMock = new Mock<ICategory>();
      var categoryViewModelMock = new Mock<IViewModel>();
      categoryViewModelMock.Setup(x => x.Model)
                           .Returns(categoryMock.Object);


      var materialMock = new Mock<IMaterial>();
      var materialViewModelMock = new Mock<IViewModel>();
      materialViewModelMock.Setup(x => x.Model)
                           .Returns(materialMock.Object);


      Assert.AreEqual(0, _sut.Compare(categoryViewModelMock.Object, categoryViewModelMock.Object));
      Assert.AreEqual(0, _sut.Compare(materialViewModelMock.Object, materialViewModelMock.Object));
    }

    [Test]
    public void Throw_If_First_Parameter_Is_Neither_Null_Nor_A_ViewModel_Of_A_Catgeory_Or_A_Material()
    {
      var categoryViewModelMock = new Mock<IViewModel>();
      categoryViewModelMock.Setup(x => x.Model)
                           .Returns(new object()); // <-- returns wrong model

      var materialMock = new Mock<IMaterial>();
      var materialViewModelMock = new Mock<IViewModel>();
      materialViewModelMock.Setup(x => x.Model)
                           .Returns(materialMock.Object);


      Assert.Throws<ArgumentException>(()=>Assert.AreEqual(0, _sut.Compare(categoryViewModelMock.Object, materialViewModelMock.Object)));
    }

    [Test]
    public void Throw_If_Second_Parameter_Is_Neither_Null_Nor_A_ViewModel_Of_A_Catgeory_Or_A_Material()
    {
      var categoryMock = new Mock<ICategory>();
      var categoryViewModelMock = new Mock<IViewModel>();
      categoryViewModelMock.Setup(x => x.Model)
                           .Returns(categoryMock.Object);

      var materialViewModelMock = new Mock<IViewModel>();
      materialViewModelMock.Setup(x => x.Model)
                           .Returns(new object()); // <-- returns wrong model


      Assert.Throws<ArgumentException>(() => Assert.AreEqual(0, _sut.Compare(categoryViewModelMock.Object, materialViewModelMock.Object)));
    }
  }
}