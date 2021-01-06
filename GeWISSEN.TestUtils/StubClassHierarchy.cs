using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeWISSEN.TestUtils
{
  /// <summary>
  /// Inherits B, IA and IZ
  /// </summary>
  public class A : B, IA, IZ
  {
  }

  public interface IA
  {
  }

  /// <summary>
  /// Inherits IB
  /// </summary>
  public class B : IB
  {
  }

  public interface IB
  {
  }

  public interface IZ
  {
  }
}
