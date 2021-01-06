using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Milan.JsonStore;
using Milan.Simulation;
using Moq;

namespace Milan.UI.Tests
{
  public static class MockExtensions
  {
    public static Mock<IJsonStore> ThatPublishesUpdatesUsing(this Mock<IJsonStore> mock,
                                                             IObservable<Unit> projectChanges)
    {
      mock.Setup(m => m.ProjectChanged)
          .Returns(projectChanges);
      return mock;
    }

    public static Mock<IJsonStore> ContainsModels(this Mock<IJsonStore> mock, IEnumerable<IModel> models)
    {
      mock.Setup(m => m.Content)
          .Returns(models);
      return mock;
    }

    public static bool ConsistsOf<T>(this IEnumerable<T> subject, IEnumerable<T> other)
    {
      var a = subject.ToArray();
      var b = other.ToArray();

      return a.Length == b.Length && b.All(item => a.Contains(item));
    }
  }
}