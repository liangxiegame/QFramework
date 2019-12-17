using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Data;

namespace QFramework.CodeGen
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
    }
}