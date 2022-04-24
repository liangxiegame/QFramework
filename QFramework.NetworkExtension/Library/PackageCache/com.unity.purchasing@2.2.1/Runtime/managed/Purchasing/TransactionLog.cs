using System;
using System.IO;
using System.Text;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Records processed transactions on the file system
    /// for de duplication purposes.
    /// </summary>
    internal class TransactionLog
    {
        private readonly ILogger logger;
        private readonly string persistentDataPath;

        public TransactionLog(ILogger logger, string persistentDataPath)
        {
            this.logger = logger;
            if (!string.IsNullOrEmpty(persistentDataPath))
            {
                this.persistentDataPath = Path.Combine(Path.Combine(persistentDataPath, "Unity"), "UnityPurchasing");
            }
        }

        /// <summary>
        /// Removes all transactions from the log.
        /// </summary>
        public void Clear()
        {
            Directory.Delete(persistentDataPath, true);
        }

        public bool HasRecordOf(string transactionID)
        {
            if (string.IsNullOrEmpty(transactionID) || string.IsNullOrEmpty(persistentDataPath))
                return false;
            return Directory.Exists(GetRecordPath(transactionID));
        }

        public void Record(string transactionID)
        {
            // Consumables have additional de-duplication logic.
            if (!(string.IsNullOrEmpty(transactionID) || string.IsNullOrEmpty(persistentDataPath)))
            {
                var path = GetRecordPath(transactionID);
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    // A wide variety of exceptions can occur, for all of which
                    // nothing is the best course of action.
                    logger.Log(e.Message);
                }
            }
        }

        private string GetRecordPath(string transactionID)
        {
            return Path.Combine(persistentDataPath, ComputeHash(transactionID));
        }

        /// <summary>
        /// Compute a 64 bit Knuth hash of a transaction ID.
        /// This should be more than sufficient for the few thousand maximum
        /// products expected in an App.
        /// </summary>
        internal static string ComputeHash(string transactionID)
        {
            UInt64 hash = 3074457345618258791ul;
            for (int i = 0; i < transactionID.Length; i++)
            {
                hash += transactionID[i];
                hash *= 3074457345618258799ul;
            }

            StringBuilder builder = new StringBuilder(16);
            foreach (byte b in BitConverter.GetBytes(hash))
            {
                builder.AppendFormat("{0:X2}", b);
            }
            return builder.ToString();
        }
    }
}
