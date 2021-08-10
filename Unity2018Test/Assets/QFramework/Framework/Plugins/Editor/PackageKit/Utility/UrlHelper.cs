namespace QFramework
{
    public class UrlHelper
    {
        public static string PackageUrl(PackageRepository p)
        {
            return "https://qframework.cn/package/detail/" + p.name;
        }
    }
}