#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Emporer.WPF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("HTW Berlin")]
[assembly: AssemblyProduct("Emporer.WPF")]
[assembly: AssemblyCopyright("Copyright © 2013 HTW Berlin")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("1be5e410-65ea-492d-acab-8b5d6c4c4d55")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
  //(used if a resource is not found in the page, 
  // or application resource dictionaries)
  ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
  //(used if a resource is not found in the page, 
  // app, or any theme specific resource dictionaries)
  )]
[assembly: XmlnsDefinition("http://www.gewissen-it.de/milan/wpf", "Emporer.WPF")]
[assembly: XmlnsDefinition("http://www.gewissen-it.de/milan/wpf", "Emporer.WPF.Controls")]
[assembly: XmlnsDefinition("http://www.gewissen-it.de/milan/wpf", "Emporer.WPF.Converter")]
[assembly: XmlnsDefinition("http://www.gewissen-it.de/milan/wpf", "Emporer.WPF.Behaviors")]
[assembly: XmlnsDefinition("http://www.gewissen-it.de/milan/wpf", "Emporer.WPF.ViewModels")]
[assembly: XmlnsPrefix("http://www.gewissen-it.de/milan/wpf", "wpf")]