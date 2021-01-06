using NUnit.Framework;

namespace GeWISSEN.Utils.Tests
{
  public class EnumerableExtensionsFacts
  {
    [Test]
    public void MinBy_finds_the_element_that_has_the_minimum_proeprty_value()
    {
      var a2 = new A(2);
      var a1 = new A(1);
      var a3 = new A(3);

      Assert.AreSame(a1, new [] {a1,a2,a3}.MinBy(x=>x.B));
    }


    [Test]
    public void MinBy_find_the_first_element_if_there_is_more_that_one_minimum()
    {
      var a2 = new A(2);
      var a1 = new A(1);
      var a3 = new A(1);

      Assert.AreSame(a1, new[] { a1, a2, a3 }.MinBy(x => x.B));
    }

    private class A
    {
      public A(int b)
      {
        B = b;
      }

      public int B { get; private set; }
      public override string ToString()
      {
        return B.ToString();
      }
    }
  }
}