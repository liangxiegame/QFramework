////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.MDV
{
    public static class Utils
    {
        //------------------------------------------------------------------------------
        // path combine with basic normalization (reduces '.' and '..' relative paths)

        static char[] separators = new char[] { '/', '\\' };

        public static string PathCombine( string _a, string _b, string separator = "/" )
        {
            var a = (_a ?? "").Split( separators, StringSplitOptions.RemoveEmptyEntries );
            var b = (_b ?? "").Split( separators, StringSplitOptions.RemoveEmptyEntries );

            var combined = a.Concat( b ).Where( el => el != "." );

            var path = new List<string>();

            foreach( var el in combined )
            {
                if( el != ".." )
                {
                    path.Add( el );
                }
                else if( path.Count > 0 )
                {
                    path.RemoveAt( path.Count - 1 );
                }
            }

            return string.Join( separator, path.ToArray() );
        }

        public static string PathNormalise( string _a, string separator = "/" )
        {
            var a = (_a ?? "").Split( separators, StringSplitOptions.RemoveEmptyEntries );

            var path = new List<string>();

            foreach( var el in a )
            {
                if( el == "." )
                {
                    continue;
                }
                if( el != ".." )
                {
                    path.Add( el );
                }
                else if( path.Count > 0 )
                {
                    path.RemoveAt( path.Count - 1 );
                }
            }

            return string.Join( separator, path.ToArray() );
        }
    }
}

