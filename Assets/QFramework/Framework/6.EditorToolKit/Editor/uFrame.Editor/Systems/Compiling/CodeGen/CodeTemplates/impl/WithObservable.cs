using System;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WithObservable : WithField
    {
        public WithObservable(System.Action selector)
        {
            Selector = selector;
        }

        public System.Action Selector { get; set; }
        public override int Priority
        {
            get { return 1; }
        }
        public string Format { get; set; }
        public WithObservable(string format = "{0}Observable")
        {
            Format = format;
        }

        protected override void Apply(TemplateContext ctx)
        {
            base.Apply(ctx);
            

        }
        //public IObservable<_ITEMTYPE_> _Name_Observable
        //{
        //    get
        //    {
        //        // return _MaxNavigatorsObservable ?? (_MaxNavigatorsObservable = new Subject<int>());
        //    }
        //}
        //public virtual Int32 MaxNavigators
        //{
        //    get
        //    {
        //        return _MaxNavigators;
        //    }
        //    set
        //    {
        //        _MaxNavigators = value;
        //        if (_MaxNavigatorsObservable != null)
        //        {
        //            _MaxNavigatorsObservable.OnNext(value);
        //        }
        //    }
        //}
    }
}