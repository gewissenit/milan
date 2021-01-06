using System.Collections.Generic;

//INFO: taken from http://www.scottlogic.com/blog/2010/07/21/exposing-and-binding-to-a-silverlight-scrollviewers-scrollbars.html
namespace Milan.VisualModeling.Utilities
{
  /// <summary>
  ///   Defines an interface that must be implemented to generate the LinqToTree methods
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ILinqTree<T>
  {
    T Parent { get; }
    IEnumerable<T> Children();
  }
}