using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class InputOutputViewModel : GraphItemViewModel
    {
        private bool _allowSelection;
        public override Vector2 Position { get; set; }
        public override string Name { get; set; }
        public bool IsInput { get; set; }
        public bool IsOutput { get; set; }

        public override string Description
        {
            get { return ReferenceItem == null ? null : ReferenceItem.Description; }
        }

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            if (ReferenceItem != null)
            {
                AllowSelection = ReferenceItem.AllowSelection;
            }

            if (ReferenceItem != null)
            {
                SelectedItemName = ReferenceItem.SelectedDisplayName;

            }
            else
            {
                SelectedItemName = "-- Select Item --";
            }
            
        }

        public bool AllowSelection
        {
            get
            {
                
                return _allowSelection;
            }
            set { _allowSelection = value; }
        }

        public override ConnectorViewModel InputConnector
        {
            get
            {
                if (!IsInput) return null;
                return base.InputConnector;
            }
        }
        public override ConnectorViewModel OutputConnector
        {
            get
            {
                if (!IsOutput || AllowSelection) return null;
                return base.OutputConnector;
            }
        }

        public GenericSlot ReferenceItem
        {
            get { return DataObject as GenericSlot; }
        }
        public string SelectedItemName { get; set; }

        public void SelectItem()
        {

            var menu = new SelectionMenu();
            menu.AddItem(new SelectionMenuItem(string.Empty,"[None]", () =>
            {
                ReferenceItem.SetInput(null);
            }));
            menu.ConvertAndAdd(ReferenceItem.GetAllowed().OfType<IItem>(), _ =>
            {
                var item = _ as IValueItem;
                if (item == null) return;
                if (IsInput)
                {
                    ReferenceItem.SetInput(item);
                }
                else
                {
                    ReferenceItem.SetOutput(item);
                }
            });

            InvertApplication.SignalEvent<IShowSelectionMenu>(_=>_.ShowSelectionMenu(menu));

//            InvertGraphEditor.WindowManager.InitItemWindow(ReferenceItem.GetAllowed().ToArray(), _ =>
//            {
//                InvertApplication.Execute(new LambdaCommand("Set Item", () =>
//                {
//                    
//                   
//                }));
//            });
        }
    }

   
;}