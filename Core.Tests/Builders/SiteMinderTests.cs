﻿using System;
using WebMinder.Core.Builders;
using WebMinder.Core.Rules;
using WebMinder.Core.Rules.UrlIsValid;
using WebMinder.Core.RuleSets;
using Xunit;

namespace WebMinder.Core.Tests.Builders
{
    public class SiteMinderTests
    {
        [Fact]
        public void RuleMinderAddsExistingRuleMinder()
        {
            var existingRuleMinder = SiteMinder.Create().AddRule<UrlIsValidRuleSet, UrlIsValidRuleHandler, UrlRequest>(x =>
                x.Using<UrlRequest>(request => request.Url = ("/SampleWebServiceEndPoint"))
                    .Build());

            Assert.Equal(1, existingRuleMinder.Rules.Count);
        }

        [Fact]
        public void RuleMinderAddsExistingRule()
        {
            var ruleSet =
                CreateRule<UrlIsValidRuleHandler, UrlRequest>.On<UrlRequest>(request => request.Url = "/SomeWebService")
                    .Build();

            var rm =
                SiteMinder.Create()
                    .AddRule<UrlIsValidRuleSet, UrlIsValidRuleHandler, UrlRequest>(webServiceUpRuleSet => ruleSet);

            Assert.Equal(1, rm.Rules.Count);
        }


        [Fact]
        public void RuleMinderCanChainRules()
        {
            // Fluent builder to add many custom or inbuilt rules in  global.asax application_onstart
            var siteMinder = SiteMinder.Create()
                .WithSslEnabled() // predefined rule redirect all http traffic to https
                .WithNoSpam(maxAttemptsWithinDuration: 100, withinDuration: TimeSpan.FromHours(1))
                .AddRule<UrlIsValidRuleSet, UrlIsValidRuleHandler, UrlRequest>(webServiceUpRuleSet =>
                    webServiceUpRuleSet.Using<UrlRequest>(request => request.Url = "/SomeWebService").Build()) // build a custom rule
                ;

            siteMinder.VerifyAllRules(); // global.asax  run via Application_BeginRequest 

            //siteMinder.VerifyRule(IpAddressRequest.GetCurrentIpAddress(recordBadIp: true)); // or verify individual request on demand  / via attribute
            Assert.Equal(2, siteMinder.Rules.Count);
        }
    }
}