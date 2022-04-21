using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Definition of a purchase payout
    /// </summary>
    [Serializable]
    public class PayoutDefinition
    {
        [SerializeField]
        PayoutType m_Type = PayoutType.Other;

        [SerializeField]
        string m_Subtype = string.Empty;

        [SerializeField]
        double m_Quantity;

        [SerializeField]
        string m_Data = string.Empty;

        /// <summary>
        /// Type of the payout
        /// </summary>
        public PayoutType type {
            get {
                return m_Type;
            }
            private set {
                m_Type = value;
            }
        }


        /// <summary>
        /// Type of the payout as a string
        /// </summary>
        public string typeString {
            get {
                return m_Type.ToString ();
            }
        }


        /// <summary>
        /// Maximum string length of the payout subtype
        /// </summary>
        public const int MaxSubtypeLength = 64;

        /// <summary>
        /// Subtype of the payout
        /// </summary>
        public string subtype {
            get {
                return m_Subtype;
            }
            private set {
                if (value.Length > MaxSubtypeLength)
                    throw new ArgumentException(string.Format("subtype cannot be longer than {0}", MaxSubtypeLength));
                m_Subtype = value;
            }
        }

        /// <summary>
        /// Quantity or value of the payout
        /// </summary>
        public double quantity {
            get {
                return m_Quantity;
            }
            private set {
                m_Quantity = value;
            }
        }

        /// <summary>
        /// Maximum number of bytes of the payout data
        /// </summary>
        public const int MaxDataLength = 1024;

        /// <summary>
        /// Payload data of the payout
        /// </summary>
        public string data {
            get {
                return m_Data;
            }
            private set {
                if (value.Length > MaxDataLength)
                    throw new ArgumentException (string.Format ("data cannot be longer than {0}", MaxDataLength));
                m_Data = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PayoutDefinition()
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="typeString"> The payout type, as a string. </param>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        public PayoutDefinition (string typeString, string subtype, double quantity) : this (typeString, subtype, quantity, string.Empty)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="typeString"> The payout type, as a string. </param>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        /// <param name="data"> The payout data. </param>
        public PayoutDefinition (string typeString, string subtype, double quantity, string data)
        {
            PayoutType t = PayoutType.Other;
            if (Enum.IsDefined(typeof(PayoutType), typeString))
                t = (PayoutType)Enum.Parse (typeof (PayoutType), typeString);

            this.type = t;
            this.subtype = subtype;
            this.quantity = quantity;
            this.data = data;
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        public PayoutDefinition (string subtype, double quantity) : this (PayoutType.Other, subtype, quantity, string.Empty)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        /// <param name="data"> The payout data. </param>
        public PayoutDefinition (string subtype, double quantity, string data) : this (PayoutType.Other, subtype, quantity, data)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="type"> The payout type. </param>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        public PayoutDefinition (PayoutType type, string subtype, double quantity) : this (type, subtype, quantity, string.Empty)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="type"> The payout type. </param>
        /// <param name="subtype"> The payout subtype. </param>
        /// <param name="quantity"> The payout quantity. </param>
        /// <param name="data"> The payout data. </param>
        public PayoutDefinition (PayoutType type, string subtype, double quantity, string data)
        {
            this.type = type;
            this.subtype = subtype;
            this.quantity = quantity;
            this.data = data;
        }
    }
}
