using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class RequestFilter
    {
        public List<RequestFilterCondition> Conditions = new List<RequestFilterCondition>();

        public Queue<Request> Filter(Queue<Request> requests)
        {
            var retRequets = requests.Where(r => Conditions.All(c => c.Filter(r)));

            return new Queue<Request>(retRequets);
        }
    }

    public class RequestFilterCondition
    {
        public Func<Request, bool> Filter = (request) => true;
    }
}