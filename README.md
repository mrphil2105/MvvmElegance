MvvmElegance
============

[![NuGet](https://img.shields.io/nuget/v/MvvmElegance.svg)](https://www.nuget.org/packages/MvvmElegance/)

Introduction
------------

MvvmElegance is an elegant and powerful ViewModel-First MVVM framework for Avalonia. It makes development easier by:

 - providing essential MVVM primitives like `Screen`, `Conductor`, `EventAggregator`, and more
 - launching dialog windows and message boxes easily with the `ViewService` class
 - providing asynchronous methods for common operations on MVVM primitives
 - disabling button clicking while calling asynchronous methods on your view models with Actions
 - avoiding view-types in view models by splitting the framework into multiple projects.

The project is inspired by [Stylet](https://github.com/canton7/Stylet) and provides many of the same types, but split into two separate projects:
a project you reference in your ViewModel-project and another used with your View-project. This ensures that no view-types are ever used in your view models.

Getting Started
---------------
To get started using MvvmElegance in your project follow the [Quick Start](https://github.com/mrphil2105/MvvmElegance/wiki/Quick-Start) guide (currently in the works).

Documentation
-------------

For a comprehensive documentation of MvvmElegance visit the [Wiki](https://github.com/mrphil2105/MvvmElegance/wiki) (currently in the works). The most up-to-date information is presented there.

Contributing
------------

Feel free to contribute to the project. [Raise an issue](https://github.com/mrphil2105/MvvmElegance/issues) if you have a problem or a question.
Please read the Wiki for the [Contributing guidelines](https://github.com/mrphil2105/MvvmElegance/wiki/Contributing) (currently in the works), if you want to contribute code to the project.
