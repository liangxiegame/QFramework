using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QF;
using Invert.uFrame.Editor;
using QF.Json;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    //public abstract class ServiceRequest
    //{
    //    public static int RequestCount = 0;
    //    public string Url { get; set; }
    //    public Action<Response> OnError { get; set; }
    //    public Action<Response> OnSuccess { get; set; }
    //    public DateTime SendTime { get; set; }
    //    public bool Finished { get; set; }

    //    public virtual void InitializeData(WWWForm form)
    //    {

    //    }
    //    public virtual string BuildUrl()
    //    {
    //        return Url;
    //    }

    //    private WWWForm _form;


    //    public void Post()
    //    {
    //        Post(null);
    //    }

    //    public virtual void Post(string[] items, params string[] values)
    //    {
    //        Post(null, items, values);
    //    }

    //    public virtual void Post(Dictionary<string, object> additionalParameters, string[] items, params string[] values)
    //    {

    //        //#if DEBUG
    //        var sb = new StringBuilder();
    //        sb.AppendLine("Post: " + BuildUrl()).AppendLine();
    //        if (items != null)
    //        {
    //            for (int i = 0; i < values.Length; i++)
    //            {
    //                sb.AppendFormat("{0} - {1}", items[i], values[i]).AppendLine();
    //            }
    //        }
    //        if (additionalParameters != null)
    //        {
    //            foreach (var p in additionalParameters)
    //            {
    //                sb.AppendFormat("{0} - {1}", p.Key, p.Value.ToString()).AppendLine();
    //            }
    //        }
    //        Debug.Log(sb.ToString());
    //        //#endif
    //        SendTime = DateTime.UtcNow;

    //        _form = new WWWForm();
    //        // Add any required elements to the form like a session id
    //        InitializeData(_form);

    //        if (items != null)
    //        {
    //            for (int i = 0; i < values.Length; i++)
    //            {
    //                _form.AddField(items[i], values[i]);
    //            }
    //        }
    //        if (additionalParameters != null)
    //        {
    //            foreach (var item in additionalParameters)
    //            {
    //                _form.AddField(item.Key, item.Value.ToString());
    //            }
    //        }
    //        InternalInvoke();
    //    }

    //    protected virtual Dictionary<string,string> BuildHeaders()
    //    {
    //        return null;
    //    }

    //    private void InternalInvoke()
    //    {

    //        var request = new WWW(BuildUrl(), _form.data, BuildHeaders());
    //     while(!request.isDone) System.Threading.Thread.Sleep(0);
    //        Finished = true;
    //        var serviceResult = ProcessRequest(request);

    //        if (serviceResult == null) throw new Exception("Service result is null.  This is not allowed.");

    //        if (!serviceResult.Success)
    //        {

    //            if (OnError != null)
    //                OnError(serviceResult);
    //            Debug.Log(serviceResult.Message);
    //        }
    //        else
    //        {

    //            if (OnSuccess != null)
    //                OnSuccess(serviceResult);
    //        }

    //    }

    //    protected abstract Response ProcessRequest(WWW request);

    //}

    //public class MarketRequest : ServiceRequest
    //{
    //    private const string SERVICE_BASE_URL = "http://micah-desktop/API/MarketService/";

    //    public MarketRequest()
    //    {

    //    }

    //    protected override Dictionary<string, string> BuildHeaders()
    //    {
    //        var hashTbl = new Dictionary<string, string>();
    //        //if (BattleStratService.SessionId != null)
    //        //    hashTbl["Cookie"] = "ASP.NET_SessionID=" + BattleStratService.SessionId;
    //        return hashTbl;
    //    }
    //    public override string BuildUrl()
    //    {
    //        return SERVICE_BASE_URL + Url;
    //    }
    //    protected override Response ProcessRequest(WWW request)
    //    {
    //        Debug.Log("RESPONSE HEADERS: " + string.Join(", ", request.responseHeaders.Keys.ToArray()));
    //        if (request.responseHeaders.ContainsKey("SET-COOKIE"))
    //        {
    //            // extract the session identifier cookie and save it
    //            // the cookie will be named, "auth" (this could be something else in your case)
    //            char[] splitter = { ';' };
    //            string[] v = request.responseHeaders["SET-COOKIE"].Split(splitter);
    //            foreach (string s in v)
    //            {
    //                Debug.Log("Found Cookie: " + s);
    //                if (string.IsNullOrEmpty(s)) continue;
    //                if (s.Substring(0, 4).ToLower().Equals("auth"))
    //                {   // found it

    //                    //BattleStratService.SessionId = s;
    //                    break;
    //                }
    //            }
    //        }
    //        return CreateResponse(request.text);
    //    }

    //    public virtual Response CreateResponse(string text)
    //    {
    //        return null;
    //    }
    //}

    //public class LoginRequest : MarketRequest
    //{
    //    public LoginRequest()
    //    {
    //        Url = "Login";
    //    }

    //    public override Response CreateResponse(string text)
    //    {
    //        return base.CreateResponse(text);
    //    }
    //}
    public static class MarketPlace
    {
        private static Dictionary<string, string> _cookies = new Dictionary<string, string>();
        public const string MARKET_PLACE_URL = "http://micah-desktop/API/MarketService/";

        public static string Call(string name)
        {
            var request = new WWW(MARKET_PLACE_URL + name);
            while (!request.isDone)
            {
                System.Threading.Thread.Sleep(0);
            }
            return request.text;
        }


        public static Dictionary<string, string> Cookies
        {
            get { return _cookies; }
            set { _cookies = value; }
        }

        public static JSONClass CallJson(string name, string paramters = null)
        {
            var url = MARKET_PLACE_URL + name;
            if (paramters != null)
            {
                url += "?";
                if (!string.IsNullOrEmpty(Token))
                {
                    url += string.Format("token={0}&{1}", Token, paramters);
                }
                else
                {
                    url += paramters;
                }

            }
            else if (!string.IsNullOrEmpty(Token))
            {
                url += "?token=" + Token;
            }
            
            Debug.Log(url);
            var headers = new Dictionary<string, string>()
            {

            };
      
                var sb = new StringBuilder();
                foreach (var item in Cookies)
                {
                    sb.AppendFormat("{0}={1};", item.Key,item.Value);
                }
            var x = sb.ToString();
                headers.Add("Cookie",x);
            Debug.Log("SENDING COOKIES: " + x);
            
            var request = new WWW(url,null,headers);
            while (!request.isDone)
            {
                System.Threading.Thread.Sleep(1000);
            }
            foreach (var item in request.responseHeaders)
            {
                Debug.Log(string.Format("{0}::{1}",item.Key,item.Value));
            }
            if (request.responseHeaders.ContainsKey("SET-COOKIE"))
            {
                var result = request.responseHeaders["SET-COOKIE"];
                var items = result.Split(';');
                foreach (var item in items)
                {
                    var pair = item.Split('=');
                    if (pair.Length < 2) continue;
                    if (Cookies.ContainsKey(pair[0]))
                    {
                        Cookies.Remove(pair[0]);
                    }
                    Cookies.Add(pair[0],pair[1]);
                }
                //if (result.Contains("ASP"))
                //{
                  
                //}
                    
                
            }
            try
            {
                return (JSONClass) JSON.Parse(request.text)[string.Format("{0}Result", name)].AsObject;
            }
            catch (Exception ex)
            {
                Token = null;
                return null;
            }
        }

        public static string Token
        {
            get { return EditorPrefs.GetString("INVERT_TOKEN", null); }
            set { EditorPrefs.SetString("INVERT_TOKEN", value); }
        }

        public static Response Login(string username, string password)
        {
            var result = CallJson("Login", string.Format("email={0}&password={1}", username, password));
            if (result == null) return null;
            var response = CreateResponse<Response>(result);
            if (response.Success)
            {
                Token = response.Message;
                
            }
            return response;
        }

        public static TResponse CreateResponse<TResponse>(JSONClass response) where TResponse : Response, new()
        {
            return new TResponse()
            {
                Message = response["Message"].Value,
                Success = response["Success"].AsBool
            };
        }

        public static MarketInfo GetMarketInfo()
        {
            var marketInfo = new MarketInfo();
            var jsonInfo = CallJson("GetMarketInfo");
            if (jsonInfo == null) return null;
            var marketItems = jsonInfo["MarketItems"].AsArray;
            foreach (JSONNode item in marketItems)
            {
                marketInfo.MarketItems.Add(new MarketItem(item.AsObject));
            }

            return marketInfo;
        }


    }
}