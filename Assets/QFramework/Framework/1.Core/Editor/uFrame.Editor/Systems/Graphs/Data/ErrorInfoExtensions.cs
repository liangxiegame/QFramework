using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Data;

namespace QF.GraphDesigner
{
    public static class ErrorInfoExtensions
    {
        public static ErrorInfo AddError(this List<ErrorInfo> list, string message, IDataRecord record,
            System.Action autoFix = null)
        {
            var error = new ErrorInfo()
            {
                Message = message,
                Record = record,
                Identifier = record.Identifier,
                AutoFix = autoFix,
                Siverity = ValidatorType.Error
            };
            if (!list.Any(p => p.Equals(error)))
                list.Add(error);
            return error;
        }

        public static ErrorInfo AddWarning(this List<ErrorInfo> list, string message, IDataRecord record,
        System.Action autoFix = null)
        {
            var error = new ErrorInfo()
            {
                Message = message,
                Identifier = record.Identifier,
                Record = record,
                AutoFix = autoFix,
                Siverity = ValidatorType.Warning
            };
            if (!list.Any(p => p.Equals(error)))
                list.Add(error);
            return error;
        }

        //public static ErrorInfo AddError(this List<ErrorInfo> list, string message, string identifier = null,
        //    Action autoFix = null)
        //{
        //    var error = new ErrorInfo()
        //    {
        //        Message = message,
        //        Identifier = identifier,
        //        AutoFix = autoFix,
        //        Siverity = ValidatorType.Error
        //    };
        //    if (!list.Any(p=>p.Equals(error)))
        //    list.Add(error);
        //    return error;
        //}
        public static ErrorInfo AddWarning(this List<ErrorInfo> list, string message, string identifier = null,
            System.Action autoFix = null)
        {
            var error = new ErrorInfo()
            {
                Message = message,
                Identifier = identifier,
                AutoFix = autoFix,
                Siverity = ValidatorType.Warning
            };
            list.Add(error);
            return error;
        }
        public static ErrorInfo AddInfo(this List<ErrorInfo> list, string message, string identifier = null,
            System.Action autoFix = null)
        {
            var error = new ErrorInfo()
            {
                Message = message,
                Identifier = identifier,
                AutoFix = autoFix,
                Siverity = ValidatorType.Info
            };
            list.Add(error);
            return error;
        }
    }
}