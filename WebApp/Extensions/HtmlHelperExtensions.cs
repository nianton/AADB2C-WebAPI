using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp
{
    public static class HelperExtensions
    {
        public static string RequestUrl(this UrlHelper urlHelper)
        {
            return urlHelper.RequestContext.HttpContext.Request.Url.AbsoluteUri;
        }

        public static string AbsoluteUrl(this UrlHelper urlHelper, string relativePath)
        {
            var currentUrl = urlHelper.RequestContext.HttpContext.Request.Url;
            var uriBuilder = new UriBuilder(currentUrl.Scheme, currentUrl.Host, currentUrl.Port, relativePath);
            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}