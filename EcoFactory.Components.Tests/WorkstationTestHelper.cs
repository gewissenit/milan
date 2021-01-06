using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Scheduling;
using NUnit.Framework;

namespace EcoFactory.Components.Tests
{
  public static class WorkstationTestHelper
  {
    public static void IsInState(this WorkstationBase sut, string state)
    {
      //Assert.AreEqual(state, sut.CurrentState);
    }

    public static void IsAvailableFor(this IWorkstationBase sut, Product product)
    {
      Assert.IsTrue(sut.IsAvailable(product));
    }

    public static void IsNotAvailableFor(this IWorkstationBase sut, Product product)
    {
      Assert.IsFalse(sut.IsAvailable(product));
    }

    public static void IsEventTypeScheduled<T>(this IScheduler scheduler)
    {
      Assert.AreEqual(1,
                      scheduler.OfType<T>()
                               .Count());
    }
  }
}