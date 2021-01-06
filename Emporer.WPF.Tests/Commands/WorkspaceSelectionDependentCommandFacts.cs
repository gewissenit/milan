using Emporer.WPF.Commands;
using GeWISSEN.TestUtils;
using GeWISSEN.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Emporer.WPF.Tests.Commands
{
  [TestFixture]
  public class WorkspaceSelectionDependentCommandFacts
  {
    public Mock<ISelection> GivenSelection { get; private set; }
    public ICommand SUT { get; private set; }
    public Action<A> GivenAction { get; private set; }

    [Test]
    public void It_can_be_created_given_its_dependencies()
    {
      GivenSelection = Given.Some<ISelection>();
      WhenSutIsCreated<A>(Do.Nothing<A>);
    }


    [Test]
    public void It_subscibes_for_all_selection_changes_on_the_given_selection()
    {
      GivenSelection = Given.Some<ISelection>();
     
      WhenSutIsCreated<A>(Do.Nothing<A>);

      GivenSelection.Verify(m => m.Subscribe<object>(It.Is<object>(o => o == SUT), It.IsAny<Action<object>>()));
    }


    [Test]
    [TestCaseSource("Selections")]
    public void It_raises_CanExecuteChanged_when_the_selection_selects_objects_of_the_specified_generic_type(object selection, bool canExecute)
    {
      GivenSelection = Given.Some<ISelection>();
      GivenSelection.Setup(m => m.Current).Returns(selection);

      WhenSutIsCreated<B>(Do.Nothing<B>);

      Assert.AreEqual(canExecute, SUT.CanExecute(null));
    }


    private void WhenSutIsCreated<TSubject>(Action<TSubject> action) where TSubject :class
    {
      SUT = new WorkspaceSelectionDependentCommand<TSubject>(GivenSelection.Object, action);
    }

    static object[] Selections =
    {
      new object[] { new A(), true },
      new object[] { new B(), true },
    };

  }
}
