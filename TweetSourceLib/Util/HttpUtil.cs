﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweetSourceLib.Util
{
    public class HttpUtil
    {
        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        public static string GetQueryString(string url)
        {
            int indexCut = url.IndexOf('?');

            if (indexCut < 0) return "";

            return url.Substring(indexCut);
        }

        public static string RemoveQueryString(string url)
        {
            int indexCut = url.IndexOf('?');

            if (indexCut < 0) return url;

            return url.Substring(0, indexCut);
        }

        /// <summary>
        /// Escape string according to RFC 3986 - 
        /// http://blog.nerdbank.net/2009/05/uriescapedatapath-and.html
        /// </summary>
        /// <param name="s">String to escape</param>
        /// <returns>Escaped string</returns>
        public static string Esc(string s)
        {
            return EscapeUriDataStringRfc3986(s);
        }

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        public static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }
    }
}