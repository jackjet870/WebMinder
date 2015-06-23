﻿using System;
using System.Collections.Generic;
using System.Web;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core
{
    public class SiteMinderModule : IHttpModule
    {
        public delegate void WebMinderRuleRequestReportedEventHandler(object sender, SiteMinderFailuresEventArgs e);
        public event WebMinderRuleRequestReportedEventHandler RuleRequestReported;
        public static SiteMinder SiteMinder { get; set; }

        public void Init(HttpApplication app)
        {
            app.BeginRequest += AppBeginRequest;
            SiteMinder = SiteMinder.Create()
             //  .WithSslEnabled()
               .WithNoSpam(5, TimeSpan.FromHours(1));

            SiteMinder.Initialise();
        }

        void AppBeginRequest(object sender, EventArgs eventArgs)
        {
            SiteMinder.VerifyRule(IpAddressRequest.GetCurrentIpAddress());
           
            if (SiteMinder.AllRulesValid()) return;
            var args = new SiteMinderFailuresEventArgs {Failures = SiteMinder.Failures};
            OnRuleRequestReported(args);
        }

        protected virtual void OnRuleRequestReported(SiteMinderFailuresEventArgs e)
        {
            if (RuleRequestReported != null)
            {
                RuleRequestReported(this, e);
            }
        }

        public void Dispose()
        {

        }
    }

    public class SiteMinderFailuresEventArgs : EventArgs
    {
        public List<Exception> Failures { get; set; }
    }
}
