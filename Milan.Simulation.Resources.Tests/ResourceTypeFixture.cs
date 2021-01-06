#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using NUnit.Framework;

namespace Milan.Simulation.Resources.Tests
{
  [TestFixture]
  public class ResourceTypeFixture
  {
    public IResourceType SUT { get; private set; }

    private ResourceType CreateSUT()
    {
      return new ResourceType();
    }

    [Test]
    public void Set_Name()
    {
      var sut = CreateSUT();
      var check = "sut";
      sut.Name = check;
      Assert.AreEqual(check, sut.Name);
    }
    
    private void WhenSutIsCreated()
    {
      SUT = CreateSUT();
    }
  }
}