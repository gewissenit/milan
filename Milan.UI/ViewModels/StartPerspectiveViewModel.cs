using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro;
using Milan.JsonStore;
using Milan.UI.Properties;
using Ork.Framework;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  internal class StartPerspectiveViewModel : DocumentBase, IWorkspace
  {
    private ColorData _selectedAccentColor;
    private CultureInfo _selectedLanguage;
    private ColorData _selectedTheme;
    private readonly IJsonStore _store;
    
    [ImportingConstructor]
    public StartPerspectiveViewModel([ImportMany] IEnumerable<IShellCommand> shellCommands, IJsonStore store)
    {
      ShellCommands = shellCommands;
      _store = store;
      DisplayName = "Start";
      AccentColors = ThemeManager.Accents.Select(a => new ColorData()
                                                      {
                                                        Name = a.Name,
                                                        ColorBrush = a.Resources["AccentColorBrush"] as Brush
                                                      })
                                 .ToArray();
      _selectedAccentColor = AccentColors.Single(ac => ac.Name == Settings.Default.AccentColor);

      Themes = ThemeManager.AppThemes.Select(a => new ColorData()
                                                  {
                                                    Name = a.Name,
                                                    ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                                                  })
                           .ToArray();
      _selectedTheme = Themes.Single(t => t.Name == Settings.Default.Theme);
      ChangeAppStyle();
      Languages = new[]
                  {
                    CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("en-US")
                  };
      _selectedLanguage = Languages.Single(kvp => kvp.Name == Settings.Default.Language);
      FrameworkElement.LanguageProperty.OverrideMetadata(typeof (FrameworkElement),
                                                         new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(SelectedLanguage.IetfLanguageTag)));
    }

    public override IDialogManager Dialogs
    {
      get { return base.Dialogs; }
      set
      {
        base.Dialogs = value;
        foreach (var shellCommand in ShellCommands)
        {
          shellCommand.DialogManager = value;
        }
      }
    }

    private static void ChangeAppStyle()
    {
      ThemeManager.ChangeAppStyle(Application.Current,
                                  ThemeManager.GetAccent(Settings.Default.AccentColor),
                                  ThemeManager.GetAppTheme(Settings.Default.Theme));
    }

    public IEnumerable<IShellCommand> ShellCommands { get; set; }

    public IEnumerable<ColorData> Themes { get; private set; }

    public IEnumerable<ColorData> AccentColors { get; private set; }

    public IEnumerable<CultureInfo> Languages { get; private set; }

    public CultureInfo SelectedLanguage
    {
      get { return _selectedLanguage; }
      set
      {
        if (Equals(_selectedLanguage, value))
        {
          return;
        }
        _selectedLanguage = value;
        Settings.Default.Language = value.Name;
        SaveSettings();
        NotifyOfPropertyChange(() => SelectedLanguage);
      }
    }

    public ColorData SelectedTheme
    {
      get { return _selectedTheme; }
      set
      {
        if (_selectedTheme == value)
        {
          return;
        }
        _selectedTheme = value;
        Settings.Default.Theme = value.Name;
        SaveSettings();
        ChangeAppStyle();
        NotifyOfPropertyChange(() => SelectedTheme);
      }
    }

    public ColorData SelectedAccentColor
    {
      get { return _selectedAccentColor; }
      set
      {
        if (_selectedAccentColor == value)
        {
          return;
        }
        _selectedAccentColor = value;
        Settings.Default.AccentColor = value.Name;
        SaveSettings();
        ChangeAppStyle();
        NotifyOfPropertyChange(() => SelectedAccentColor);
      }
    }

    public int Index
    {
      get { return 0; }
    }

    public bool IsEnabled
    {
      get { return true; }
    }
    
    public override bool IsDirty
    {
      get { return _store.UnsavedChanges; }
      set { base.IsDirty = value; }
    }

    private void SaveSettings()
    {
      Settings.Default.Save();
    }

    public void HandleKeyInput(Key key)
    {
      
    }
  }
}