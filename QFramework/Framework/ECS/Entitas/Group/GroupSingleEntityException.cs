using System.Linq;
using QFramework;

namespace Entitas {

    public class GroupSingleEntityException<TEntity> : ExceptionWithHint where TEntity : class, IEntity {

        public GroupSingleEntityException(IGroup<TEntity> group)
            : base("Cannot get the single entity from " + group +
                "!\nGroup contains " + group.count + " entities:",
                string.Join("\n", group.GetEntities().Select(e => e.ToString()).ToArray())) {
        }
    }
}
