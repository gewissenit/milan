using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Emporer.WPF;
using GeWISSEN.TestUtils;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.UI.ViewModels;
using Milan.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace Milan.UI.Tests
{
  [TestFixture]
  public class ModelingPerspectiveViewModelFacts
  {
    private Subject<Unit> _storeUpdates;
    private ModelingPerspectiveViewModel SUT { get; set; }

    private Mock<IModelEditorViewModel> GivenModelEditor { get; set; }
    private Mock<IPropertyEditorViewModel> GivenPropertyEditor { get; set; }
    private Mock<ISelection> GivenSelection { get; set; }
    private Mock<IJsonStore> GivenStore { get; set; }
    private Mock<IModelFactory> GivenModelFactory { get; set; }
    private Mock<IDeleteManager> GivenDeleteManager { get; set; }
    private IEnumerable<IEntityFactory> GivenEntityFactories { get; set; }

    [Test]
    public void It_provides_a_list_of_the_available_models()
    {
      GivenDefaultDependencies();

      var models = new[]
                   {
                     new Mock<IModel>().Object,
                     new Mock<IModel>().Object
                   };

      GivenStore.ContainsModels(models);

      WhenSutIsCreated();

      Assert.True(SUT.Models.ConsistsOf(models));
    }

    [Test]
    public void It_provides_the_currently_selected_model()
    {
      GivenDefaultDependencies();
      var models = new[]
                   {
                     new Mock<IModel>().Object,
                     new Mock<IModel>().Object
                   };

      GivenStore.ContainsModels(models);
      WhenSutIsCreated();

      Assert.AreSame(models.First(), SUT.Models.First());
    }



    private void GivenDefaultDependencies()
    {
      GivenModelEditor = Given.Some<IModelEditorViewModel>();
      GivenPropertyEditor = Given.Some<IPropertyEditorViewModel>();
      GivenSelection = Given.Some<ISelection>();

      _storeUpdates = new Subject<Unit>();
      GivenStore = Given.Some<IJsonStore>()
                        .ThatPublishesUpdatesUsing(_storeUpdates);

      GivenModelFactory = Given.Some<IModelFactory>();
      GivenDeleteManager = Given.Some<IDeleteManager>();
      GivenEntityFactories = new IEntityFactory[0];
    }

    private void WhenSutIsCreated()
    {
      SUT = new ModelingPerspectiveViewModel(GivenModelEditor.Object,
                                             GivenPropertyEditor.Object,
                                             GivenSelection.Object,
                                             GivenStore.Object,
                                             GivenModelFactory.Object,
                                             GivenEntityFactories,
                                             GivenDeleteManager.Object);
    }
  }
}