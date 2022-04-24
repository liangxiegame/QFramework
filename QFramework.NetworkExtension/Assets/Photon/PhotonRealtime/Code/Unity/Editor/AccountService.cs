// ----------------------------------------------------------------------------
// <copyright file="AccountService.cs" company="Exit Games GmbH">
//   Photon Cloud Account Service - Copyright (C) 2012 Exit Games GmbH
// </copyright>
// <summary>
//   Provides methods to register a new user-account for the Photon Cloud and
//   get the resulting appId.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_2017_4_OR_NEWER
#define SUPPORTED_UNITY
#endif


#if UNITY_EDITOR

namespace Photon.Realtime
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ExitGames.Client.Photon;


    /// <summary>
    /// Creates a instance of the Account Service to register Photon Cloud accounts.
    /// </summary>
    public class AccountService
    {
        private const string ServiceUrl = "https://partner.photonengine.com/api/{0}/User/RegisterEx";

        private readonly Dictionary<string, string> RequestHeaders = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "x-functions-key", "" }
        };

        private const string DefaultContext = "Unity";

        private const string DefaultToken = "VQ920wVUieLHT9c3v1ZCbytaLXpXbktUztKb3iYLCdiRKjUagcl6eg==";

        /// <summary>
        /// third parties custom context, if null, defaults to DefaultContext property value
        /// </summary>
        public string CustomContext = null;     // "PartnerCode" on the server

        /// <summary>
        /// third parties custom token. If null, defaults to DefaultToken property value
        /// </summary>
        public string CustomToken = null;

        /// <summary>
        /// If this AccountService instance is currently waiting for a response. While pending, RegisterByEmail is blocked.
        /// </summary>
        public bool RequestPendingResult = false;

        /// <summary>
        /// Attempts to create a Photon Cloud Account asynchronously. Blocked while RequestPendingResult is true.
        /// </summary>
        /// <remarks>
        /// Once your callback is called, check ReturnCode, Message and AppId to get the result of this attempt.
        /// </remarks>
        /// <param name="email">Email of the account.</param>
        /// <param name="serviceTypes">Defines which type of Photon-service is being requested.</param>
        /// <param name="callback">Called when the result is available.</param>
        /// <param name="errorCallback">Called when the request failed.</param>
        /// <param name="origin">Can be used to identify the origin of the registration (which package is being used).</param>
        public bool RegisterByEmail(string email, List<ServiceTypes> serviceTypes, Action<AccountServiceResponse> callback = null, Action<string> errorCallback = null, string origin = null)
        {
            if (this.RequestPendingResult)
            {
                Debug.LogError("Registration request pending result. Not sending another.");
                return false;
            }

            if (!IsValidEmail(email))
            {
                Debug.LogErrorFormat("Email \"{0}\" is not valid", email);
                return false;
            }

            string serviceTypeString = GetServiceTypesFromList(serviceTypes);
            if (string.IsNullOrEmpty(serviceTypeString))
            {
                Debug.LogError("serviceTypes string is null or empty");
                return false;
            }

            string fullUrl = GetUrlWithQueryStringEscaped(email, serviceTypeString, origin);

            RequestHeaders["x-functions-key"] = string.IsNullOrEmpty(CustomToken) ? DefaultToken : CustomToken;


            this.RequestPendingResult = true;

            PhotonEditorUtils.StartCoroutine(
                PhotonEditorUtils.HttpPost(fullUrl,
                    RequestHeaders,
                    null,
                    s =>
                    {
                        this.RequestPendingResult = false;
                        //Debug.LogWarningFormat("received response {0}", s);
                        if (string.IsNullOrEmpty(s))
                        {
                            if (errorCallback != null)
                            {
                                errorCallback("Server's response was empty. Please register through account website during this service interruption.");
                            }
                        }
                        else
                        {
                            AccountServiceResponse ase = this.ParseResult(s);
                            if (ase == null)
                            {
                                if (errorCallback != null)
                                {
                                    errorCallback("Error parsing registration response. Please try registering from account website");
                                }
                            }
                            else if (callback != null)
                            {
                                callback(ase);
                            }
                        }
                    },
                    e =>
                    {
                        this.RequestPendingResult = false;
                        if (errorCallback != null)
                        {
                            errorCallback(e);
                        }
                    })
            );
            return true;
        }


        private string GetUrlWithQueryStringEscaped(string email, string serviceTypes, string originAv)
        {
            string emailEscaped = UnityEngine.Networking.UnityWebRequest.EscapeURL(email);
            string st = UnityEngine.Networking.UnityWebRequest.EscapeURL(serviceTypes);
            string uv = UnityEngine.Networking.UnityWebRequest.EscapeURL(Application.unityVersion);
            string serviceUrl = string.Format(ServiceUrl, string.IsNullOrEmpty(CustomContext) ? DefaultContext : CustomContext );

            return string.Format("{0}?email={1}&st={2}&uv={3}&av={4}", serviceUrl, emailEscaped, st, uv, originAv);
        }

        /// <summary>
        /// Reads the Json response and applies it to local properties.
        /// </summary>
        /// <param name="result"></param>
        private AccountServiceResponse ParseResult(string result)
        {
            try
            {
                AccountServiceResponse res = JsonUtility.FromJson<AccountServiceResponse>(result);
                // Unity's JsonUtility does not support deserializing Dictionary, we manually parse it, dirty & ugly af, better then using a 3rd party lib
                if (res.ReturnCode == AccountServiceReturnCodes.Success)
                {
                    string[] parts = result.Split(new[] { "\"ApplicationIds\":{" }, StringSplitOptions.RemoveEmptyEntries);
                    parts = parts[1].Split('}');
                    string applicationIds = parts[0];
                    if (!string.IsNullOrEmpty(applicationIds))
                    {
                        parts = applicationIds.Split(new[] { ',', '"', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        res.ApplicationIds = new Dictionary<string, string>(parts.Length / 2);
                        for (int i = 0; i < parts.Length; i = i + 2)
                        {
                            res.ApplicationIds.Add(parts[i], parts[i + 1]);
                        }
                    }
                    else
                    {
                        Debug.LogError("The server did not return any AppId, ApplicationIds was empty in the response.");
                        return null;
                    }
                }
                return res;
            }
            catch (Exception ex) // probably JSON parsing exception, check if returned string is valid JSON
            {
                Debug.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Turns the list items to a comma separated string. Returns null if list is null or empty.
        /// </summary>
        /// <param name="appTypes">List of service types.</param>
        /// <returns>Returns null if list is null or empty.</returns>
        private static string GetServiceTypesFromList(List<ServiceTypes> appTypes)
        {
            if (appTypes == null || appTypes.Count <= 0)
            {
                return null;
            }

            string serviceTypes = ((int)appTypes[0]).ToString();
            for (int i = 1; i < appTypes.Count; i++)
            {
                int appType = (int)appTypes[i];
                serviceTypes = string.Format("{0},{1}", serviceTypes, appType);
            }

            return serviceTypes;
        }

        // RFC2822 compliant matching 99.9% of all email addresses in actual use today
        // according to http://www.regular-expressions.info/email.html [22.02.2012]
        private static Regex reg = new Regex("^((?>[a-zA-Z\\d!#$%&'*+\\-/=?^_{|}~]+\\x20*|\"((?=[\\x01-\\x7f])[^\"\\]|\\[\\x01-\\x7f])*\"\\x20*)*(?<angle><))?((?!\\.)(?>\\.?[a-zA-Z\\d!#$%&'*+\\-/=?^_{|}~]+)+|\"((?=[\\x01-\\x7f])[^\"\\]|\\[\\x01-\\x7f])*\")@(((?!-)[a-zA-Z\\d\\-]+(?<!-)\\.)+[a-zA-Z]{2,}|\\[(((?(?<!\\[)\\.)(25[0-5]|2[0-4]\\d|[01]?\\d?\\d)){4}|[a-zA-Z\\d\\-]*[a-zA-Z\\d]:((?=[\\x01-\\x7f])[^\\\\[\\]]|\\[\\x01-\\x7f])+)\\])(?(angle)>)$",
             RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        public static bool IsValidEmail(string mailAddress)
        {
            if (string.IsNullOrEmpty(mailAddress))
            {
                return false;
            }
            var result = reg.Match(mailAddress);
            return result.Success;
        }
    }

    [Serializable]
    public class AccountServiceResponse
    {
        public int ReturnCode;
        public string Message;
        public Dictionary<string, string> ApplicationIds; // Unity's JsonUtility does not support deserializing Dictionary
    }


    public class AccountServiceReturnCodes
    {
        public static int Success = 0;
        public static int EmailAlreadyRegistered = 8;
        public static int InvalidParameters = 12;
    }

    public enum ServiceTypes
    {
        Realtime = 0,
        Turnbased = 1,
        Chat = 2,
        Voice = 3,
        TrueSync = 4,
        Pun = 5,
        Thunder = 6,
        Quantum = 7,
        Fusion = 8,
        Bolt = 20
    }
}

#endif