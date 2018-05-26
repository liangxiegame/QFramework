# UnityEditorUI

A wrapper around the Unity editor GUI system for constructing editor windows using a fluent API instead of Unity's `OnGUI` functions. 

This system supports two-way data binding between the UI and a ViewModel class, allowing you to create simpler, unit testable code for editor extensions by moving the logic and the UI code into separate classes.

## Constructing editor windows

Set up a simple editor window with a label and a button:

```
// Create an instance of the view you want to bind the UI to
var viewModel = new ExampleView();

// Create the UI
var gui = new UnityEditorUI.GUI();
gui.Root()
    .Label()
        .Text.Value("My new editor window")
    .End()
    .Button()
        .Text.Value("Do something!")
        .Click.Bind(() => viewModel.DoSomething())
        .Tooltip.Value("Click to trigger an event")
    .End()
    
// Bind the UI to the view
gui.BindViewModel(viewModel);
```

And then render and update it by adding the following line to your editor window's existing `OnGUI()` method:
```
gui.OnGUI();
```

Every property on a GUI widget can have its value set to a constant value using `.Value()`, or bound to another property using `.Bind()`. If the class being bound to implements [`INotifyPropertyChanged`](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx), this will set up a two way data binding, so that properties in the bound class get updated when the UI changes and the UI gets .

## Examples
The project in the `Examples` directory has been tested with Unity 5.2.3p2 and should contain everything you need to load up and run the examples. Since this library is purely for Unity editor extensions, there is no scene included in the project.

### Example 1
This example shows the most basic sample of a use case for the UnityEditorUI system, binding a Unity editor window to a simple view model class but not subscribing to property changed events.

### Example 2
This example demonstrates how to set up a view model class that implements [`INotifyPropertyChanged`](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx) and sends events back to the UI when properties are changed. 

## Widgets
### Button
Clickable push button widget.
#### Bindable properties
- Text : `string`
- Tooltip : `string`
- Width : `int`
- Height : `int`

#### Bindable events
- Click
     
### Checkbox
Boolean check box widget.
#### Bindable properties 
- Checked : `bool`
- Label : `string`

### DateTimePicker
Widget for entering a date and time. Essentially a TextBox with date validation on it.
#### Bindable properties
- Date : `DateTime`
- Width : `int`
- Height : `int`

### DropdownBox
Drop-down selection field. Labels for individual items are set by calling the bound object's `ToString()` method.
#### Bindable properties
- SelectedItem : `object`
- Items : `object[]`
- Label : `string`
- Tooltip : `string`

### Label
Widget for displaying read-only text.
#### Bindable properties
- Text : `string`
- Tooltip : `string`
- Bold : `bool`
- Width : `int`
- Height : `int`

### LayerPicker
Widget for selecting Unity layers.
#### Bindable properties
- Label : `string`
- SelectedLayers : `string[]`

### Spacer
Inserts a space between other widgets

### TextBox
Widget for entering text
#### Bindable properties
- Text : `string`
- Width : `int`
- Height : `int`

### Vector3Field
Widget for entering vectors with X, Y and Z coordinates.
#### Bindable properties
- Label : `string`
- Tooltip : `string`
- Vector : `Vector3`

## Layouts
### RootLayout
### HorizontalLayout
### VerticalLayout


