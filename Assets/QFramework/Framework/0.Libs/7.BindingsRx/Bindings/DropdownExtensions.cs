using System;
using System.Collections.Generic;
using BindingsRx.Filters;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BindingsRx.Bindings
{
    public static class DropdownExtensions
    {
        public static IDisposable BindValueTo(this Dropdown input, IReactiveProperty<int> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, property, bindingType, filters).AddTo(input); }

        public static IDisposable BindValueTo(this Dropdown input, Func<int> getter, Action<int> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<int>[] filters)
        { return GenericBindings.Bind(() => input.value, x => input.value = x, getter, setter, bindingType, filters).AddTo(input); }

        public static IDisposable BindValueTo(this Dropdown input, IReactiveProperty<string> property, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        {
            return GenericBindings.Bind(() => input.options[input.value].text, x =>
            {
                var matchingIndex = 0;
                for (var i = 0; i < input.options.Count; i++)
                {
                    if (input.options[i].text == property.Value)
                    {
                        matchingIndex = i;
                        break;
                    }
                }
                input.value = matchingIndex;
            }, property, bindingType, filters).AddTo(input);
        }

        public static IDisposable BindValueTo(this Dropdown input, Func<string> getter, Action<string> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        {
            return GenericBindings.Bind(() => input.options[input.value].text, x =>
            {
                var matchingIndex = 0;
                var currentValue = getter();
                for (var i = 0; i < input.options.Count; i++)
                {
                    if (input.options[i].text == currentValue)
                    {
                        matchingIndex = i;
                        break;
                    }
                }
                input.value = matchingIndex;
            }, getter, setter, bindingType, filters).AddTo(input);
        }

        public static IDisposable BindOptionsTo(this Dropdown input, IReactiveCollection<string> options)
        {
            var addSubscription = options.ObserveAdd().Subscribe(x =>
            {
                var newOption = new Dropdown.OptionData { text = x.Value };
                input.options.Insert(x.Index, newOption);
            });

            var updateSubscription = options.ObserveReplace().Subscribe(x =>
            {
                var existingOption = input.options[x.Index];
                existingOption.text = x.NewValue;
            });

            var removeSubscription = options.ObserveRemove().Subscribe(x => input.options.RemoveAt(x.Index));

            input.options.Clear();

            foreach (var option in options)
            {
                var newOption = new Dropdown.OptionData { text = option };
                input.options.Add(newOption);
            }
            
            return new CompositeDisposable(addSubscription, updateSubscription, removeSubscription).AddTo(input);
        }
        
        public static IDisposable BindOptionsTo(this Dropdown input, IReactiveCollection<Dropdown.OptionData> options)
        {
            var addSubscription = options.ObserveAdd().Subscribe(x => input.options.Insert(x.Index, x.Value));
            var removeSubscription = options.ObserveRemove().Subscribe(x => input.options.RemoveAt(x.Index));
            var updateSubscription = options.ObserveReplace().Subscribe(x =>
            {
                input.options.RemoveAt(x.Index);
                input.options.Insert(x.Index, x.NewValue);
            });

            input.options.Clear();

            foreach (var option in options)
            { input.options.Add(option); }

            return new CompositeDisposable(addSubscription, updateSubscription, removeSubscription).AddTo(input);
        }

        public static IDisposable BindOptionsTo<T>(this Dropdown input, IReactiveCollection<T> options, Func<T, string> textLocator, Func<T, Sprite> spriteLocator = null)
        {
            var addSubscription = options.ObserveAdd().Subscribe(x =>
            {
                var newOption = new Dropdown.OptionData { text = textLocator(x.Value) };
                if(spriteLocator != null)
                { newOption.image = spriteLocator(x.Value); }

                input.options.Insert(x.Index, newOption);
            });

            var updateSubscription = options.ObserveReplace().Subscribe(x =>
            {
                var existingOption = input.options[x.Index];
                existingOption.text = textLocator(x.NewValue);

                if (spriteLocator != null)
                { existingOption.image = spriteLocator(x.NewValue); }
            });

            var removeSubscription = options.ObserveRemove().Subscribe(x => input.options.RemoveAt(x.Index));

            input.options.Clear();

            foreach (var option in options)
            {
                var newOption = new Dropdown.OptionData { text = textLocator(option) };

                if (spriteLocator != null)
                { newOption.image = spriteLocator(option); }

                input.options.Add(newOption);
            }

            return new CompositeDisposable(addSubscription, updateSubscription, removeSubscription).AddTo(input);
        }

        /// <summary>
        /// This is a best guess attempt for users but it is not fast and does not pick up on entries changing, just the count changing
        /// </summary>
        public static IDisposable BindOptionsTo<T>(this Dropdown input, ICollection<T> options, Func<T, string> textLocator, Func<T, Sprite> spriteLocator = null)
        {
            var lastCount = options.Count;

            return Observable.EveryUpdate()
                .TakeWhile(x => lastCount != options.Count)
                .Subscribe(x =>
                {
                    input.options.Clear();
                    foreach (var option in options)
                    {
                        var newOption = new Dropdown.OptionData { text = textLocator(option) };

                        if (spriteLocator != null)
                        { newOption.image = spriteLocator(option); }

                        input.options.Add(newOption);
                    }
                }).AddTo(input);
        }
    }
}