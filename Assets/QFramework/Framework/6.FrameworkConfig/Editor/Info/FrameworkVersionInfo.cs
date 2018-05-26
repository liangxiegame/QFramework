/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2018.3 liangxie
 ****************************************************************************/


namespace QFramework
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