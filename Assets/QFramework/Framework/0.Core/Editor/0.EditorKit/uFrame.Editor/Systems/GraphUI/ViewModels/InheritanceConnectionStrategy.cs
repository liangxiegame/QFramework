using System.Linq;
using UnityEngine;

namespace Invert.uFrame.Editor.ViewModels
{
    //public class InheritanceConnectionStrategy<TSource> :
    //    DefaultConnectionStrategy<TSource, TSource>
    //    where TSource : GenericInheritableNode, IConnectable
    //{
    //    public override Color ConnectionColor
    //    {
    //        get { return Color.green; }
    //    }

    //    protected override bool CanConnect(TSource output, TSource input)
    //    {
    //        if (output.Identifier == input.Identifier) return false;
    //        if (input.DerivedNodes.Any(p => p.Identifier == output.Identifier)) return false;
    //        return base.CanConnect(output, input);
    //    }

    //    public override bool IsConnected(TSource output, TSource input)
    //    {
    //        return input.BaseIdentifier == output.Identifier;
    //    }

    //    protected override void ApplyConnection(TSource output, TSource input)
    //    {
    //        input.SetBaseType(output);
    //    }

    //    protected override void RemoveConnection(TSource output, TSource input)
    //    {
    //        input.SetBaseType(null);
    //    }
    //}
}