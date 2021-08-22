using System.Runtime.InteropServices;
using System.Text;

namespace Tool
{
    public class FileNameHelper
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]string path,
            [MarshalAs(UnmanagedType.LPTStr)]StringBuilder short_path,
            int short_len
        );

        public static string GetShortPath(string name)
        {
            
            int lenght = 0;

            lenght = GetShortPathName(name, null, 0);
            if (lenght == 0)
            {
                //new nghmp.GenericErrorForm("Can't get short path name", name, true);
                return name;
            }
            StringBuilder short_name = new StringBuilder(lenght);
            lenght = GetShortPathName(name, short_name, lenght);
            if (lenght == 0)
            {
                //new nghmp.GenericErrorForm("Can't get short path name", name, true);
                return name;
            }
            return short_name.ToString();
        }
        
    }
}