using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace QFramework.Example
{
    public partial class CounterAppController : Control,IController
    {
        [Export] public Label CountText;

        [Export] public Button BtnAdd;

        [Export] public Button BtnSub;
        
        public override void _Ready()
        {
            this.GetModel<ICounterAppModel>().Count.RegisterWithInitValue(count =>
            {
                CountText.Text = count.ToString();
            }).UnRegisterWhenNodeExitTree(this);

            
            BtnAdd.Pressed += () =>
            {
                this.SendCommand<IncreaseCountCommand>();
                GD.Print(this.GetModel<ICounterAppModel>().Count.Value);
            };

            BtnSub.Pressed += () =>
            {
                GD.Print("Hello");
                this.SendCommand<DecreaseCountCommand>();
            };
        }

        public override void _Process(double delta)
        {
        }


        public override void _ExitTree()
        {
            base._ExitTree();
        }

        public IArchitecture GetArchitecture() => CounterApp.Interface;


        public class IncreaseCountCommand : AbstractCommand 
        {
            protected override void OnExecute()
            {
                var model = this.GetModel<ICounterAppModel>();
                model.Count.Value++;
            }
        }

        public class DecreaseCountCommand : AbstractCommand
        {
            protected override void OnExecute()
            {
                this.GetModel<ICounterAppModel>().Count.Value--;
            }
        }
        
        public class CounterApp : Architecture<CounterApp>
        {
            protected override void Init()
            {
                this.RegisterSystem<IAchievementSystem>(new AchievementSystem()); 
                this.RegisterModel<ICounterAppModel>(new CounterAppModel());
                this.RegisterUtility<IStorage>(new Storage());
            }
        }

        public interface ICounterAppModel : IModel
        {
            BindableProperty<int> Count { get; }
        }

        public class CounterAppModel : AbstractModel, ICounterAppModel
        {
            public BindableProperty<int> Count { get; } = new BindableProperty<int>();

            protected override void OnInit()
            {
                var storage = this.GetUtility<IStorage>();

                // 设置初始值（不触发事件）
                Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));

                // 当数据变更时 存储数据
                Count.Register(newCount => { storage.SaveInt(nameof(Count), newCount); });
            }
        }

        public interface IAchievementSystem : ISystem
        {
        }

        public class AchievementSystem : AbstractSystem, IAchievementSystem
        {
            protected override void OnInit()
            {
                this.GetModel<ICounterAppModel>() // -+
                    .Count
                    .Register(newCount =>
                    {
                        if (newCount == 10)
                        {
                            GD.Print("触发 点击达人 成就");
                        }
                        else if (newCount == 20)
                        {
                            GD.Print("触发 点击专家 成就");
                        }
                        else if (newCount == -10)
                        {
                            GD.Print("触发 点击菜鸟 成就");
                        }
                    });
            }
        }

        public interface IStorage : IUtility
        {
            void SaveInt(string key, int value);
            int LoadInt(string key, int defaultValue = 0);
        }

        public class Storage : IStorage
        {
            public Godot.Collections.Dictionary<string, Variant> KeyValues;

            Godot.Collections.Dictionary<string, Variant> GetKeyValues()
            {
                if (KeyValues == null)
                {
                    if (FileAccess.FileExists($"user://user.save"))
                    {
                        using var user = FileAccess.Open($"user://user.save", FileAccess.ModeFlags.Read);
                        var jsonContent = user.GetAsText();

                        if (string.IsNullOrEmpty(jsonContent))
                        {
                            KeyValues = new Godot.Collections.Dictionary<string, Variant>();
                        }
                        else
                        {
                            KeyValues = new Godot.Collections.Dictionary<string, Variant>(
                                (Dictionary)Json.ParseString(jsonContent));
                        }
                    }
                    else
                    {
                        KeyValues = new Godot.Collections.Dictionary<string, Variant>();
                    }
                }

                return KeyValues;
            }

            void Save()
            {
                using var user = FileAccess.Open($"user://user.save", FileAccess.ModeFlags.Write);
                user.StoreString(Json.Stringify(GetKeyValues()));
            }

            public void SaveInt(string key, int value)
            {
                var keyValues = GetKeyValues();
                if (keyValues.ContainsKey(key))
                {
                    keyValues[key] = value;
                }
                else
                {
                    keyValues.Add(key, value);
                }
                Save();
            }

            public int LoadInt(string key, int defaultValue = 0)
            {

                var keyValues = GetKeyValues();
                if (keyValues.ContainsKey(key))
                {
                    return GetKeyValues()[key].AsInt32();
                }
                else
                {
                    return defaultValue;
                }
                
            }
        }


    }
}