  Loxodon Framework
Version: 1.9.2
© 2016, Clark Yang
=======================================

Thank you for purchasing the LoxodonFramework!
I hope you enjoy using the product and that it makes your game development faster and easier.
If you have a moment,please leave me a review on the Asset Store.

The plugin is compatible with MacOSX,Windows,Linux,UWP,IOS and Android etc.

Please email yangpc.china@gmail.com for any help or issues.

Notices:
AOT Compilation Options: "nrgctx-trampolines=8192,nimt-trampolines=8192,ntrampolines=8192" for IOS.

UPDATE NOTES
----------------------------------------
version 1.9.2
	Fixed bug with multithreading on webgl platform.

version 1.9.1
	Fixed a bug that Lua expressions have been destroyed when rebinding.

version 1.9.0
	Added localization of Sprite,Texture,AudioClip,VideoClip,Font,etc.
	Fixed a bug that does not trigger a value change notification when an expression is bound to a static "IObservableProperty".

version 1.8.10
	Fixed a bug that WindowManager.Clear() cannot clear.
	Added AES CTR encryption algorithm, and supports stream encryption and the "Seek" feature of encrypted stream.
	Fixed bugs on the UWP.
	Fixed a bug that failed to compile on the ios platform.
	Added System.Type.IsSZArray to the blacklist to avoid errors when xlua generates code.

version 1.8.9
	Added CoroutineTask.
	Fixed a bug in the PathParser.
	Added LocalizedAudioSourceInResources,LocalizedImageInResources,LocalizedRawImageInResources.

version 1.8.7
	Added encryption and decryption feature.
	Added lua precompilation tools
	Added lua script loader

version 1.8.6
	Changed Localization.GetText(string key, params object[] args) to GetFormattedText(string key, params object[] args).
	Changed IConfiguration.GetString(string key, params object[] args) to GetFormattedString(string key, params object[] args).
	Deleted Localization.GetText(string key, string defaultValue, params object[] args) and IConfiguration.GetString(string key, string defaultValue, params object[] args).
	In order to support localized files in csv format,changed IDocumentParser.Parse(Stream input) to IDocumentParser.Parse(Stream input, CultureInfo cultureInfo)

version 1.8.5
	Added configuration module to read properties file.
	Added "Subset" method in the "Localization" class to support the creation of a subset of Localization.

version 1.8.4
	Added a logging system to the Lua plugin
	Added CommandParameter for data binding,support multiple buttons to bind to the same command.
	Added InteractionAction.
	Added IDialogService.

version 1.8.2
	Fixed a bug in expression binding.
	Fixed a bug when loading lua scripts via file path.

version 1.8.0
	Rewrite the code of the data binding module to optimize data binding performance and reduce gc as much as possible.Please remove the old version of the code before updating to 1.8.0.

version 1.7.8
	Supports Lua to override method of the parent class of C#,and added the ability to access methods and properties of the parent class of C#.

version 1.7.6
	Added Chinese document.

version 1.7.5
	Added support for XLua.

version 1.7.1
	Changed the namespaces of INotifyCollectionChanged and NotifyCollectionChangedEventArgs to be compatible with .net 4.6 and .net standard2.0.
	Fixed a bug in the UWP(window10).

version 1.7.0
	Provided all source code for the project.
	Merged Loxodon.Framework.Free and Loxodon.Framework.Pro.

version 1.6.14
	Fixed warning that many variables are not used.

version 1.6.13
	Added an example of the asynchronous loading of sprites.
	Added an example of localized resource binding to UI controls.
	Improved Executors and localization scripts.

version 1.6.9
	Fixed formatting errors for several prefabs.

version 1.6.8
	Added an example of data bindings of ListView and Sprite.

version 1.6.7
	Fixed the error that the editor is stuck because of the background thread is not finished.
	Fixed a bug in the asynchronous loading function of the "ResourcesViewLocator" class.

version 1.6.5
	Added interactive requests and modified examples of interactive requests.
	Added data binding for interactive requests.
	Added the Variable and examples of Variable.
	Improved data binding.
	Fixed a bug about the WeakAction.
	Deleted the class named ObjectSourceProxyFactory.

version 1.5.5
	AsyncTask,ProgressTask:Added a new feature that is capturing coroutine's exceptions and supporting cancellation of coroutine task.

version 1.5.2
	Fixed a bug that the Toggle bind to the bool value.

version 1.5.0
	Added support for UWP(Windows10).
	Refactor prefs module for compatibility with UWP platform.
	Fixed a few bugs.

version 1.1.5
	Updated PathParser and BindingBuilder to support a new feature,eg:bindingSet.Bind(this.button).For(v => v.onClick).To(vm=>vm.OnClick()).
	Loxodon.Framework.Localizations.ITypeConver method 'GetByte' renamed to 'GetType'.
	Added a method 'Create' in the Executors class.

version 1.1.0
	Added IScriptInvoker interface for lua scripting.
	Added BehaviourBindingExtension and deleted ViewBindingExtensions.
	Added non-generic classes,BindingSet and BindingBuilder.

version 1.0.5
	Added RemarkAttribute.cs.
	Fixed a bug about initialization of the Executors.

version 1.0.4
	Added InterceptableEnumerator,Supports to catch exceptions in the coroutine.
	Added WeakValueDictionary.ICache is deprecated,use WeakValueDictionary or Dictionary instead.
	Improved ServiceContainer.

version 1.0.3
	Fixed an exception that occurred when the method StrongCache.Get is invoked.

version 1.0.2
	IProgressResult: Added callback about the Progress value.
	Improved the ICache and ProgressResult and ObservableObject etc.

version 1.0.1
	Removed unused assets for the examples.
	Fixed a bug,an exception occurred when InputField.text bind to the "Null" in Unity5.3.0 version.


