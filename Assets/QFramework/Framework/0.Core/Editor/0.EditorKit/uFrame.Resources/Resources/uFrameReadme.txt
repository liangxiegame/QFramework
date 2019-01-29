[u]Frame Release Notes
Be sure to subscribe to our newsletter @ invertgamestudios.com for updates, notifications, and cool info.

Note for 1.5 Users: The 1.6 Version of uFrame, only falls short of a major release due to it being mostly bug fixes and changes that will support 
	the idea of ECS and MVVM working together, and promises for the 1.x release. If you already have a lot of work done in 1.5 save 1.6 for your next game or application.  
	While upgrading is possible if you are familiar enough with it, but it will require a significant effort.
1.6.2
	- New Node Styles for better visual feedback.
	- Documentation Updates
	- Interactive Tutorial Updates
	- Wiki page link on welcome screen
	- New extension method for copying viewmodels
	- New sugar methods to invoke commands on view-models.
	- Serializer fixes
	- Kernel Fix: Correctly publishes Loading instead of Loaded twice.
	- Other small enhancements & Fixes

1.601
	- Added: New Welcome Screen
	- Cleaned up Documentation look and feel
	- Documentation changes/fixes/updates are coming in the next version
	- Default project is now known as ExampleProject and installed on demand rather than existing on installation
	- Other small enhancements

1.6p1
Fixed: reference to "Mono.CompilerServices.SymbolWriter" is added to designer files

Whats new in 1.6?
	- A completely re-written kernel that gives a very easy and straightforward application life-cycle within Unity.
	- Services and an Event Aggregator drastically increase the ease of communication and portability in code.
	- uFrame is now seperated into various namespaces, the uFrame.Kernel, and uFrame.MVVM are the main ones.  
	- 100's of bug fixes and performance issues
	- New Navigation bar for easy contextual switching
	- New Tabs bar easier working withing multiple graphs
	- Brand new code templating engine, customize the output any way you like.
	- Simplified and extensible binding system, create your own designer bindings.
	- Support for all platforms including WinRT
	- Tons more...


Editor and Project:
1.5.1
Lots of bug fixes
Fixed: Unity-5 Fix: Deprecated WWW call in UniRx throws Exception on Unity 5
Fixed: Renaming .asset file for external graphs doesn't rename the node itself
Fixed: (Render Settings on GameManager is back) Render Settings from target scene are not applied due to LoadLevelAdditive in LevelLoaderView 
Fixed: Situational: SetupBindings() not called for all Views
Fixed: Create{Element}View binding will generate the same return statement twice.
Fixed: Double-click on main node of external graph: InvalidOperationException: Operation is not valid due to the current state of the object
Fixed: No default state for the statemachine results into broken inspector.
Fixed: "Generate Comments" toggle on ProjectRepository doesn't work ( Removed it, will be back in a future version)
Fixed: Need way to order the Instantiation of ViewModels defined as Instances. ( Right-click move-up/down)
Fixed: Instantiating Prefab with a 4.6 UI RectTransform causes warning about possible scale issues, when it sets the parent transform.
Fixed: Scene Manager Unloaded Method is called twice
Fixed: .Remove() on a collection item deletes references to parent elements
Fixed: Inherited views creating duplicate initialize fields causing "Hides Inheirted member" error.
Fixed: When views are instantiated from prefabs there is a strange bug that happens only when the game is ran for the second (or higher) time. uFrame doesn't initialize VM properties correctly then, instead views are created with default values (nulls).
Fixed: Remove generated RequireComponent(typeof(MyViewComponent)) from Views.designer.cs
Fixed: uFrame GameManager will always return null ActiveSceneManager on Unity4.6b21, causing uFrame to be useless.
Fixed: If a computed property doesn't have any child properties, it wouldn't setup the dependant for the property.
Fixed: ParentView on a view now searches up the transform tree to find the nearest parent view

1.5
uFrame 1.5 is out. Even thought is a minor version step, there are a lot of new exciting features,
 changes and bugfixes. This version addresses almost all of the issues in 1.4. Let's see what's new:

1.5 brings a completely new theme, with new cool UI features that make editor more responsive.
 Not only it looks more professional, but the usability is greatly improved, for example
 highlighted connections or handy Inputs and Outputs for the diagram items.

Thanks to the new project system, you can now seprate your project into as many graphs as you want, keeping it
 all under control. New workflow features, project level namespaces, and even code generation settings have arrived!

Framework:

  RX
    In 1.5 you will be able to unleash the power of Reactive Extensions!
     UniRX implementation of RX is now shipped with uFrame.
     Almost everything now is observable: viewmodels, properties, commands, collections and more!
     Reactive extension methods are integrated seamlessly into your code and definitely will save you
     incredible amount of time.
     Binding have been completely reworked to follow the RX model.

  Computed Properties
    1.5 introduces computed properties. They are just properties which depend on other properties.
     Simple and powerful. You can express dependency from the same element or from child elements.
     They can be dependent on regular properties, scene properties or other computed properties.
     They can serve as triggers for state machines.
     In your code you can easily modify all the dependencies by simply overriding a special method.

  Reactive State Machines:
    1.5 brings brand new Reactive State Machines directly into your diagrams.
     You can rapidly design states, transitions and triggers.
     Then you can generate view bindings for the states with a few mouse clicks. Finally
     you can easily setup any additional logic in the code.
     uFrame State Machines do not use any dirty checks! This can save a lot of performance
     to let other amazing things run inside your game.
     You can easily debug your states right in the diagram or from the View Inspector.

  Class Nodes:
    Now you can easily create regular classes right inside of your diagram.
     You can use those as property types or command argument types
     Generated class automagically implements INotifyPropertyChanged interface, which makes it
     compatible with other cool assets, libraries and frameworks

    Moreover, you can now use any type which does not inherit from Unity Object as a type for ViewModel property
     or command argument.

  Registered Instances:
    Subsystems now can export instances of ViewModels which will be shared around you application.
     No more problems with transferring small pieces of data around your scenes. It's all done automagically
     and only requires a few click in the diagram editor!

    It also plays nicely with the inheritance, allowing you to
     substitute certain instances with different implementations.

    Finally, you can register several instances under different names,
     and instantly get access to those in your controllers!

Major Changes:

  SIE

    No more single instance elements!
     Since you can now register any ViewModel instance in the subsystem,
     you have ultimate control over your shared instances. That's why Single Instance Elements are removed from uFrame.
     This also brings consistency into controller code, as now every command receives a sender.

  Scene Transitions:
    Scene transitions have been reworked to become as user-friendly as possible.
     You can still use inspector to define linear transition logic.
     But as soon as it comes to transitions based on command arguments / shared instances data,
     you can easily do it in the code, by just overriding a couple of methods.
     You can set scenes and scene manager settings with any condition you want.

  Scene Persistence:
    Save'n'Load functionality was greatly solidified. Now every user has an ability
     to implement saving and loading without worrying too much about the low-level functionality.
     ViewModel saving/loading is already there. But you also have an ability persist some of your view data
     like transforms, states and so on.
     You can access Read and Write methods in your View and operate over any data you want.

  2-Way-Bindings
    2-Way-Bindings were completely reworked and are now called Scene Properties.

    Those little handy things allow you to access specific View information, like positions and distances.
     Scene properties rely heavily on Observables. This means you get exceptional control over you performance.
     You define how and when those things are calculated. The rest is automagically done by uFrame.

  1.5 includes lots of other changes and bug fixes. And even more will come in the future versions. So stay tuned!

1.41 ---
Important Note: Element Inheritance,Property Types,Collections Types, Command Types and View links may be broken.  Double-check that they are correctly wired together.
Added: Computed properties on view-models and controllers. Right-Click and choose "Computed By->{PropertyName}" on a Element Property.


1.406 8/13/2014
Added: Scrollbar on Binding's window.
Fixed: Initialize{ElementName} not being called when using {ElementName}Controller.Create{ElementName}();
Fixed: Removed "YUPYUPYUP" debug log message when removing a binding :)
Fixed: When transitioning to and from another scene single instance view-models wern't resetting.
Fixed: Default Identifier for multiple single instance elements that where derived from a specific type was causing issues.
Fixed: Settings Colors where off when creating a new diagram
Added: When creating an new node it auto selects it for editing.
Added: When creating an item it auto selects it for editing.
Added: Keyboard shortcuts for Saving, Adding nodes, deleting nodes, and deleting items.


1.401 8/5/2014
Fixed: Scaling the diagram down caused element properties to exceed the width of the box.
Fixed: Unlinking of ViewComponents not working.
Fixed: Two-Way Properties on views not generating correctly.

1.4 8/5/2014
Important: When importing the new package ( you  may need to remove the old first. ) Be sure to open your diagram and click on 'Save & Compile'.
Important: see upgrading to 1.4 tutorial http://invertgamestudios.com/home/video-tutorials/upgrading-to-1-4
Note: uFrame Playmaker Plugin is obsolete.
   After making a very tough decision to deprecate the Playmaker plugin we felt with the new features it could be drastically optimized and more seemless. 
   If you are currently dependent on the old playmaker plugin and still want to upgrade you'll need to map View-Model properties directly to fsm's manually when properties change.  
   We feel that having a diagram node for FSM templates and using links to keep things synchronized will drastically improve the workflow when working with Playmaker.

Feature: View-Model property editing at run-time.
Feature: View Bindings in diagram.
  This feature drastically increases the workflow of uFrame. You now longer have to worry about checking bindings. Just add the binding in the diagram and it will insert the code directly into the view code.
Feature: Two-Way Properties in views.
Feature: Add properties from any component in the project directly onto a view in the designer. This will automatically wire the properties up and create the View-Model serialization code for you.
Feature: Scene State Saving and Loading with new generic serialization classes (Currently in beta).
Feature: View Properties for data persistance and Element References.
Feature: See all registered ViewModels in the SceneContext under the SceneManager inspector at run-time.
Feature: Visibility into the dependency container VIA GameManager Inspector
Feature: Code Generators now generate serialization code for ViewModels using uFrames new generic serializer implementation.
Feature: Generic Scene State Serialization (Beta)
Fixed: ArgumentException: Getting control 2's position in a group with only 2 controls when doing KeyDown error on rename.
Change: View-Models now contain a reference to a controller. This removes the responsibility from the controller and makes the framework work with View-Models and controllers in a more cohesive manner.
Change: Moved Controller WireCommands to ViewModels
Fix: When an element property is on a element with a short name the delete button can't be clicked.
Fix: UnityVS plugin now uses "Visual Studio Tools/Generate Project Files"

For Support Questions Contact:

Micah Osborne
invertgamestudios@gmail.com