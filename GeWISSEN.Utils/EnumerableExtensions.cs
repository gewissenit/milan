using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeWISSEN.Utils
{
  /// <summary>
  /// Extends <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtensions
  {
    public static TItem MinBy<TItem, TProperty>(this IEnumerable<TItem> input, Func<TItem, TProperty> getPropertyValue)
    {
      return input.OrderBy(getPropertyValue)
                  .First();
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> input, T item)
    {
      return input.Except(new[] { item });
    }
  }
}
