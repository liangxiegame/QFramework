using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    
    public class ConsoleViewModel : WindowViewModel
    {
        private List<LogMessage> _messages;
        private Type[] _availableTypes;
        private IRepository _repository;


        public IRepository Repository
        {
            get { return _repository ?? (InvertApplication.Container.Resolve<IRepository>()); }
            set { _repository = value; }
        }

        public ConsoleViewModel()
        {
            var messageTypes = this.GetType().Assembly.GetTypes().Where(t => typeof (LogMessage).IsAssignableFrom(t)).Except(new []{typeof(LogMessage)});
            AvailableTypes = messageTypes.ToArray();
            UpdateMessages();
        }

        public List<LogMessage> Messages
        {
            get { return _messages ?? (_messages = new List<LogMessage>()); }
            set { _messages = value; }
        }

        public MessageType? CurrentMessageTypeFilter { get; set; }

        public Type CurrentTypeFilter{ get; set; }

        public void SelectFilterMessageType(MessageType? newType)
        {
            CurrentMessageTypeFilter = newType;
            UpdateMessages();
        }

        public void SelectFilterType(Type filter)
        {
            CurrentTypeFilter = filter;
            UpdateMessages();
        }

        public void UpdateMessages()
        {
            var type = CurrentTypeFilter ?? typeof(LogMessage);
            var messageType = CurrentMessageTypeFilter;

            var messages = Repository.AllOf(type).Cast<LogMessage>();

            if(CurrentMessageTypeFilter.HasValue) messages = messages.Where(m => m.MessageType == CurrentMessageTypeFilter.Value);
        
            Messages.Clear();
            Messages.AddRange(messages);

        }

        public Type[] AvailableTypes
        {
            get { return _availableTypes ?? (_availableTypes = new Type[0]); }
            set { _availableTypes = value; }
        }
                
    }

}