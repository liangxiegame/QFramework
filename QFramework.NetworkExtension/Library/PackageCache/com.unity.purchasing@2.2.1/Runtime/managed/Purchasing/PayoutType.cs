using System;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Type of the purchase payout.
    /// </summary>
    public enum PayoutType
    {
        /// <summary>
        /// A payout that doesn't fit one of the other presumed categories. Specified by subtype and/or data in <c>Payout Definition</c>.
        /// </summary>
        Other,

        /// <summary>
        /// Currency. Usually of a specified quantity in <c>Payout Definition</c>.
        /// </summary>
        Currency,

        /// <summary>
        /// An Item. Specified by subtype and/or data in <c>Payout Definition</c>.
        /// </summary>
        Item,

        /// <summary>
        /// A Resource. Usually of a specified quantity, and specified by subtype and/or data in <c>Payout Definition</c>.
        /// </summary>
        Resource
    }
}
