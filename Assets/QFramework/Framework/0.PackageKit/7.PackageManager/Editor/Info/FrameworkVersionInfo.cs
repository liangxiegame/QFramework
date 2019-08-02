/****************************************************************************
 * Copyright (c) 2018.7 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/


namespace QF
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AllReleaseInfo
    {
        public List<FrameworkVersionInfo> FrameworkVersionInfos;
    }

    public class FrameworkVersionInfo
    {
        [JsonProperty("url")]              public string Url;
        [JsonProperty("assets_url")]       public string AssetsUrl;
        [JsonProperty("upload_url")]       public string UploadUrl;
        [JsonProperty("html_url")]         public string HTMLUrl;
        [JsonProperty("id")]               public int    Id;
        [JsonProperty("tag_name")]         public string TagName;
        [JsonProperty("target_commitish")] public string TargetCommitish;
        [JsonProperty("name")]             public string Name;
        [JsonProperty("draft")]            public bool   Draft;
        [JsonProperty("author")]           public Author Author;
        [JsonProperty("prerelease")]       public bool   PreRelease;
        [JsonProperty("created_at")]       public string CreateAt;
        [JsonProperty("published_at")]     public string PublishedAt;
        [JsonProperty("assets")]           public List<Asset>  Assets;
        [JsonProperty("tarball_url")]      public string TarballUrl;
        [JsonProperty("zipball_url")]      public string ZipballUrl;
        [JsonProperty("body")]             public string Body;

        public static FrameworkVersionInfo ParseLatest(string latestContent)
        {
            return latestContent.FromJson<FrameworkVersionInfo>();
        }
    }

    public class Author
    {
        [JsonProperty("login")]               public string Login;
        [JsonProperty("id")]                  public int    Id;
        [JsonProperty("avatar_url")]          public string AvatarUrl;
        [JsonProperty("gravatar_id")]         public string GravatarId;
        [JsonProperty("url")]                 public string Url;
        [JsonProperty("html_url")]            public string HTMLUrl;
        [JsonProperty("followers_url")]       public string FollowersUrl;
        [JsonProperty("following_url")]       public string FollowingUrl;
        [JsonProperty("gists_url")]           public string GistsUrl;
        [JsonProperty("starred_url")]         public string StarredUrl;
        [JsonProperty("subscriptions_url")]   public string SubscripionsUrl;
        [JsonProperty("organizations_url")]   public string OrganizationsUrl;
        [JsonProperty("repos_url")]           public string ReposUrl;
        [JsonProperty("events_url")]          public string EventsUrl;
        [JsonProperty("received_events_url")] public string ReceivedEventsUrl;
        [JsonProperty("type")]                public string Type;
        [JsonProperty("site_admin")]          public bool   SiteAdmin;
    }

    public class Asset
    {
        [JsonProperty("url")]                  public string Login;
        [JsonProperty("id")]                   public int    Id;
        [JsonProperty("name")]                 public string Name;
        [JsonProperty("label")]                public string Label;
        [JsonProperty("uploader")]             public Author Uploader;
        [JsonProperty("content_type")]         public string ContentType;
        [JsonProperty("size")]                 public int    Size;
        [JsonProperty("download_count")]       public int    DownloadCount;
        [JsonProperty("created_at")]           public string CreateAt;
        [JsonProperty("updated_at")]           public string UpdateAt;
        [JsonProperty("browser_download_url")] public string BrowserDownloadUrl;
    }
}