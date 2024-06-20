# CitaviAddOnEx

This repository contains a class library that extends the addon programming model of Swiss Academic Citavi to support all dialogs inherited from the `FormBase` class.

## Getting Started

### Prerequisites

The simplest way to program an add-on for Citavi with this library is to include the latest release from [nuget.org](https://www.nuget.org/packages/CitaviAddOnEx/1.0.3) and ship it with it. Citavi should be installed in the default directory (`C:\Program Files (x86)\Citavi 6`). The Community Edition of Microsoft Visual Studio 2019 can be used as the development environment. 

The target framework used under Citavi 6 is the .net Framework 4.6.1.

### Programming

```csharp
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System;
using System.Windows.Forms;

namespace [NAMESPACE]
{
    public class [CLASSNAME] : CitaviAddOnEx<T>
    {
        // Called through System.Windows.Forms.Application.Idle event and can used to check if as example button states changed
        public override void OnApplicationIdle(T form)
        {

        }
       
        // Called for every form of T when its load
        public override void OnHostingFormLoaded(T form)
        {
           
        }
        
        // Called for every form of T when its close
        public virtual void OnHostingFormClosed(T form) 
        { 
           
        }

        // Called when user click on something in the form
        public override void OnBeforePerformingCommand(T form, BeforePerformingCommandEventArgs e)
        {
           
        }

        // Called when application language is changed
        public override void OnLocalizing(T form)
        {
            
        }
    }
}
```

To develop an addon you derive from the `CitaviAddOnEx<T>` class and implement the corresponding methods if required. `T` must be a Form derived from the Citavi model provided base class `FormBase`.

**Example**
- MainForm
- KnowledgeItemFileForm
- MacroEditor

If a form is specified that provides inherent add-on support, the class will use it.

## Wiki

Some extra informations you will find in the [wiki](../..//wiki)

## Projects

The [FormatCodeAddon](https://github.com/lutz/FormatCodeAddon) uses this library to extend the MacroEditor and can be viewed for assistance.

## Disclaimer

>There are no support claims by the company **Swiss Academic Software GmbH**, the provider of **Citavi** or other liability claims for problems or data loss. Any use is at your own risk. All rights to the name **Citavi** and any logos used are owned by **Swiss Academic Software GmbH**.

## License

This project is licensed under the [MIT](LICENSE) License
