# Yammer Chat

Yammer Chat was a standalone app focused on the private messaging experience of Yammer. Originally built by Yammer as a promising experiment but for iOS only, this project offered the same feature set (and design...) to Windows Phone users. 

The app was released in 2014 and ran for about a year.

<img src="Submission Files/screenshot_1.png" width="150" /> <img src="Submission Files/screenshot_2.png" width="150" /> <img src="Submission Files/screenshot_3.png" width="150" /> <img src="Submission Files/screenshot_4.png" width="150" /> <img src="Submission Files/screenshot_5.png" width="150" /> <img src="Submission Files/screenshot_6.png" width="150" /> <img src="Submission Files/screenshot_7.png" width="150" /> 

## Architecture

Project is composed of Universal packages (reusable between phone and eventual desktop app) and Windows Phone-specific modules (with "WP").

Notable features include a dependency injection framework (see Bootstrapper.cs), tests, realtime updates, adoption of the async/await syntax and extensive use of interfaces. 

A neat trick is the pattern of `IProgressIndicator.Show()` returning an `IDisposable` (implemented as an action hiding the indicator), coupled with feature code putting that call from within a `using` construct like so:

```csharp
using (this.progressIndicator.Show())
{
    await this.identityStore.LoginAsync(token);
    ...
} // No need to worry about forgetting to hide the progress indicator; the Show() method's IDisposable gets called automatically
```

#### Yammer.Chat.Core

* Generic API service with cancellable async REST methods & domain-specific wrappers
* Plugin interfaces (serialization, file pickers, settings, ...)
* Network & app models, parsers
* Repositories (identity, users, threads, ...)

#### Yammer.Chat.ViewModels

* BaseViewModel enabling databinding
* Domain-specific view models

#### Yammer.Chat.WP

* Screens
* Custom controls
* Platform implementations of plugin interfaces

#### Yammer.Chat.WP.BackgroundAgent

* Standalone module responsible for the Live Tile unread count

## Reviews

> Great app. Love it loads.
>
> Andy W.

> Just downloaded the app, it really looks great, well done. Looking forward to using it.
>
> Charles N.