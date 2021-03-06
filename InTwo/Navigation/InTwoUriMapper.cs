﻿using System;
using System.Collections.Generic;
using System.Windows.Navigation;

using FlurryWP8SDK.Models;
using ScottIsAFool.WindowsPhone.Logging;

namespace InTwo.Navigation
{
    public class InTwoUriMapper : UriMapperBase
    {
        public static ILog Log = new WPLogger(typeof(InTwoUriMapper));
        public override Uri MapUri(Uri uri)
        {
            var tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            if (tempUri.Contains("intwo:"))
            {
                var referral = tempUri.Substring(tempUri.IndexOf("intwo:", StringComparison.Ordinal) + 6);
                if (!string.IsNullOrEmpty(referral))
                {
                    Log.Info("App launched from {0}", referral);
                    FlurryWP8SDK.Api.LogEvent("Referral", new List<Parameter> {new Parameter("Referral", referral)});
                }
                else
                {
                    Log.Info("App launched via Uri, no referer");
                    FlurryWP8SDK.Api.LogEvent("Referral");
                }
            }

            return uri;
        }
    }
}
