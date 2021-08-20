namespace QFramework.Example
 {
     public static class UIEventID
     {
         public enum MenuPanel
         {
             //ui事件ID为3000～5999，见QMsgSpan
             Start = QMgrID.UI,
             ChangeMenuColor,
             End,
         }
         
         public enum SomeEvent
         {
             Start = MenuPanel.End,
             SomeOperation,
             End
         }
     }
 }