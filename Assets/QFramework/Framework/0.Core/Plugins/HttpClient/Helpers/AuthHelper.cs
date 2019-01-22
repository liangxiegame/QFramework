
namespace CI.HttpClient.Helpers
{
    public static class AuthHelper
    {
        public static string CreateBasicAuthHeader(string username, string password)
        {
            return "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
        }

        public static string CreateOAuth2Header(string token)
        {
            return "Bearer " + token;
        }
    }
}