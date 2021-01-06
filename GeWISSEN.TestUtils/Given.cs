using Moq;

namespace GeWISSEN.TestUtils
{
  public static class Given
  {
    public static Mock<T> Some<T>() where T : class
    {
      return new Mock<T>();
    }
  }
}