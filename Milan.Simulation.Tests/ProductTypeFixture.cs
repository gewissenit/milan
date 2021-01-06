using NUnit.Framework;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class ProductTypeFixture : EntityFixture<ProductType>
  {
    protected override ProductType CreateSUT()
    {
      return new ProductType();
    }

    protected override void Default_Ctor(ProductType sut)
    {
      Assert.True(string.IsNullOrEmpty(sut.IconId));
    }

    [Test]
    public void Set_IconId()
    {
      var value = "id";
      SetProperty("IconId", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.IconId = v, s => s.IconId);
    }


    [Test]
    public void Set_Same_IconId_Does_Not_Raise_PropertyChanged()
    {
      var value = "id";
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.IconId = v);
    }
  }
}