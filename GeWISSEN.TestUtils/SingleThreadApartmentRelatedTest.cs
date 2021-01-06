using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeWISSEN.TestUtils
{
  /// <summary>
  /// Unit tests that involve controls (WPF, WinForms) mostly won't work correctly in all runners
  /// if they are not run in a Single Thread Apartment (STA). This helper solves that problem.
  ///
  /// The internet suggest it should work by setting [Apartment(ApartmentState.STA)],
  /// but this does not work (at least not during TeamCity agent test run).
  /// 
  /// INFO: taken from https://sachabarbs.wordpress.com/2008/11/18/nunit-sta-threads-testing-wpf/
  /// INFO: Invalid solutions: http://stackoverflow.com/a/35587531
  /// </summary>
  public class SingleThreadApartmentRelatedTest
  {
    public static void Run(Action test)
    {
      AutoResetEvent signal = new AutoResetEvent(false);

      var thread = new Thread(() => { ExecuteTest(test, signal); });

      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();

      signal.WaitOne();
    }

    private static void ExecuteTest(Action test, AutoResetEvent are)
    {
      try
      {
        test();
        are.Set();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}
