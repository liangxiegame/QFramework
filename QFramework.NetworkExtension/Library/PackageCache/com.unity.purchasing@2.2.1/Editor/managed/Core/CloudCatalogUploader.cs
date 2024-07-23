using System;
using System.Net;
using UnityEngine;
using UnityEditor.Connect;
using System.Collections.Generic;

namespace UnityEditor.Purchasing
{
    /// <summary>
    /// Utility to upload the Product Catalog to the cloud backend
    /// </summary>
    public static class CloudCatalogUploader
    {
        /// <summary>
        /// Upload the catalog data to the cloud backend with no callbacks
        /// </summary>
        /// <param name="catalogJson"> Raw json string containing the catalog data </param>
        public static void Upload(string catalogJson)
        {
            Upload(catalogJson, null, null);
        }

        /// <summary>
        /// Upload the catalog data to the cloud backend, with a completion callback event
        /// </summary>
        /// <param name="catalogJson"> Raw json string containing the catalog data </param>
        /// <param name="onComplete"> Completion callback function </param>
        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete)
        {
            Upload(catalogJson, onComplete, null);
        }

        /// <summary>
        /// Upload the catalog data to the cloud backend, with a completion callback event and a progress event
        /// </summary>
        /// <param name="catalogJson"> Raw json string containing the catalog data </param>
        /// <param name="onComplete"> Completion callback function </param>
        /// <param name="onProgressChanged"> Progress callback function </param>
        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete, Action<UploadProgressChangedEventArgs> onProgressChanged)
        {
            Upload(catalogJson, onComplete, onProgressChanged, UnityConnect.instance.GetCoreConfigurationUrl());
        }

        /// <summary>
        /// Upload the catalog data to the cloud backend, with a completion callback event and a progress event, and specifying the base backend URL
        /// </summary>
        /// <param name="catalogJson"> Raw json string containing the catalog data </param>
        /// <param name="onComplete"> Completion callback function </param>
        /// <param name="onProgressChanged"> Progress callback function </param>
        /// <param name="baseURL"> Base URL of the backend to upload the catalog file to </param>
        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete, Action<UploadProgressChangedEventArgs> onProgressChanged, string baseURL)
        {
            string exportURL = baseURL;
            exportURL += "/api/v2/projects/";
            exportURL += UnityConnect.instance.GetProjectGUID();
            exportURL += "/iap_catalog";
            var uri = new Uri(exportURL);

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(catalogJson);


            // Only OSX supports SSL certificate validation, disable checking on other platforms.
            var originalCallback = ServicePointManager.ServerCertificateValidationCallback;
            if (Application.platform != RuntimePlatform.OSXEditor)
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            var client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", UnityConnect.instance.GetAccessToken()));
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            if (onComplete != null)
            {
                client.UploadDataCompleted += (object o, UploadDataCompletedEventArgs eventArgs) => {
                        ServicePointManager.ServerCertificateValidationCallback = originalCallback;
                        onComplete(eventArgs);
                    };
            }
            if (onProgressChanged != null)
            {
                client.UploadProgressChanged += (object o, UploadProgressChangedEventArgs eventArgs) => onProgressChanged(eventArgs);
            }

            client.UploadDataAsync(uri, "POST", bytes);
        }
    }
}
