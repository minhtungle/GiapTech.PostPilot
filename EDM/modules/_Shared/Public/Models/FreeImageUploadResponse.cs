using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class FreeImageUploadResponse
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("success")]
        public SuccessInfo Success { get; set; }

        [JsonProperty("image")]
        public ImageInfo Image { get; set; }

        [JsonProperty("status_txt")]
        public string StatusText { get; set; }
    }

    public class SuccessInfo
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class ImageInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("expiration")]
        public int Expiration { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("original_filename")]
        public string OriginalFilename { get; set; }

        [JsonProperty("is_animated")]
        public int IsAnimated { get; set; }

        [JsonProperty("id_encoded")]
        public string IdEncoded { get; set; }

        [JsonProperty("size_formatted")]
        public string SizeFormatted { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("url_short")]
        public string UrlShort { get; set; }

        [JsonProperty("url_seo")]
        public string UrlSeo { get; set; }

        [JsonProperty("url_viewer")]
        public string UrlViewer { get; set; }

        [JsonProperty("url_viewer_preview")]
        public string UrlViewerPreview { get; set; }

        [JsonProperty("url_viewer_thumb")]
        public string UrlViewerThumb { get; set; }

        [JsonProperty("image")]
        public ImageDetail ImageDetail { get; set; }

        [JsonProperty("thumb")]
        public ImageDetail Thumb { get; set; }

        [JsonProperty("medium")]
        public ImageDetail Medium { get; set; }

        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }

        [JsonProperty("display_width")]
        public int DisplayWidth { get; set; }

        [JsonProperty("display_height")]
        public int DisplayHeight { get; set; }

        [JsonProperty("views_label")]
        public string ViewsLabel { get; set; }

        [JsonProperty("likes_label")]
        public string LikesLabel { get; set; }

        [JsonProperty("how_long_ago")]
        public string HowLongAgo { get; set; }

        [JsonProperty("date_fixed_peer")]
        public string DateFixedPeer { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_truncated")]
        public string TitleTruncated { get; set; }

        [JsonProperty("title_truncated_html")]
        public string TitleTruncatedHtml { get; set; }

        [JsonProperty("is_use_loader")]
        public bool IsUseLoader { get; set; }
    }

    public class ImageDetail
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mime")]
        public string Mime { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }

}