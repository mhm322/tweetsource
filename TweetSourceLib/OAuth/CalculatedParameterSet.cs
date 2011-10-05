﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace TweetSource.OAuth
{
    public abstract class CalculatedParameterSet : AuthorizationHeader.ParameterSet
    {
        public abstract string Nonce { get; }
        public abstract string Timestamp { get; }
        public abstract string Signature { get; }

        public CalculatedParameterSet(AuthorizationHeader.ParameterSet baseParams)
            : base(baseParams) { }
    }

    public class CalculatedParameterSet10Impl : CalculatedParameterSet
    {
        protected readonly DateTime BASE_DATE_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override string Nonce
        {
            get
            {
                string tickNow = DateTime.Now.Ticks.ToString();
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(tickNow));
            }
        }

        public override string Timestamp
        {
            get
            {
                var ts = DateTime.UtcNow - BASE_DATE_TIME;
                long seconds = (long)ts.TotalSeconds;
                return seconds.ToString();
            }
        }

        public override string Signature
        {
            get
            {
                if (SignatureMethod != "HMAC-SHA1")
                    throw new ApplicationException(string.Format(
                        "Signature method {0} is not supported", SignatureMethod));

                string signingKey = string.Format("{0}&{1}",
                    Uri.EscapeDataString(ConsumerSecret),
                    Uri.EscapeDataString(TokenSecret));

                //GS - Sign the request
                string baseString = GetBaseString();
                Debug.WriteLine("Base string: " + baseString);
                var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
                byte[] hashes = hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString));
                string signatureString = Convert.ToBase64String(hashes);
                Debug.WriteLine("Signature string: " + signatureString);
                return signatureString;
            }
        }

        protected string GetBaseString()
        {
            //GS - When building the signature string the params
            //must be in alphabetical order. I can't be bothered
            //with that, get SortedDictionary to do it's thing
            var sd = new SortedDictionary<string, string>();
            sd.Add("oauth_version", Version);
            sd.Add("oauth_consumer_key", ConsumerKey);
            sd.Add("oauth_nonce", Nonce);
            sd.Add("oauth_signature_method", SignatureMethod);
            sd.Add("oauth_timestamp", Timestamp);
            sd.Add("oauth_token", Token);

            //GS - Build the signature string
            var elements = new List<string>();
            //elements.Add("POST");
            //elements.Add(Uri.EscapeDataString(Url));

            foreach (KeyValuePair<string, string> entry in sd)
            {
                elements.Add(entry.Key + "=" + entry.Value);
            }
            string parameters = string.Join("&", elements.ToArray());
            string baseString = string.Format("POST&{0}&{1}",
                Uri.EscapeDataString(Url), Uri.EscapeDataString(parameters));

            return baseString;
        }

        public CalculatedParameterSet10Impl(AuthorizationHeader.ParameterSet baseParams)
            : base(baseParams) { }
    }
}