﻿using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.Runners;
using WebMinder.Core.StorageProviders;
using Xunit;

namespace WebMinder.Core.Tests.Runners
{
    public class RuleSetRunnerTests
    {
       
        private AggregateRuleSetHandler<TestObject> _ruleSetHandler;
        private IpAddressBlockerRuleSetHandler _ruleset;
        private const string RuleSet = "RuleSetRunnerFixture Test Rule";
        const string ErrorDescription = "RuleSetRunnerFixture Error exception for logging";

        public RuleSetRunnerTests()
        {
            _ruleset = new IpAddressBlockerRuleSetHandler()
            {
                UpdateRuleCollectionOnSuccess = false,
                StorageMechanism = new MemoryCacheStorageProvider<IpAddressRequest>(),
                RuleSetName = Guid.NewGuid().ToString()
            };
            _ruleset.UseMemoryCacheStorage(Guid.NewGuid().ToString());
        }

        [Fact(Skip = "")]
        public void ShouldGetRulesFromRuleSets()
        {
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(_ruleset);
            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(1, rules.Count());
             
        }

        [Fact]
        public void ShouldVerifyAllRules()
        {
            int count = 0;
            var rule1 = new MaximumCountRuleSetHandler<TestObject>()
            {
                MaximumResultCount = 0,
                InvalidAction = () => count++
            };

            var rule2 = new MaximumCountRuleSetHandler<IpAddressRequest>()
            {
                MaximumResultCount = 0,
                InvalidAction = () => count++,

            };

            RuleSetRunner.Instance.AddRule<MaximumCountRuleSetHandler<TestObject>>(rule1);
            RuleSetRunner.Instance.AddRule<MaximumCountRuleSetHandler<IpAddressRequest>>(rule2);

            rule1.VerifyRule(new TestObject());
            rule2.VerifyRule(new IpAddressRequest());

            RuleSetRunner.Instance.VerifyAllRules();
            Assert.Equal(4, count);  // rule will auto trigger once each time it hits the rule - then again once run
        }

        [Fact]
        public void ShouldGetRulesCountFromRuleSets()
        {
            var rule = new IpAddressBlockerRuleSetHandler(){StorageMechanism = new MemoryCacheStorageProvider<IpAddressRequest>()};
            rule.UseMemoryCacheStorage(Guid.NewGuid().ToString());
           
            RuleSetRunner.Instance.AddRule<IpAddressRequest>(rule);
           
            rule.VerifyRule(new IpAddressRequest() {Id=Guid.NewGuid(), IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            Assert.Equal(1, rule.Items.Count());

            rule.VerifyRule(new IpAddressRequest() { Id = Guid.NewGuid(), IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            Assert.Equal(2, rule.Items.Count());

            rule.VerifyRule(new IpAddressRequest() { Id = Guid.NewGuid(), IpAddress = "127.0.01", CreatedUtcDateTime = DateTime.UtcNow, IsBadRequest = true });
            var rules = RuleSetRunner.Instance.GetRules<IpAddressRequest>();
            Assert.Equal(3, rules.First().Items.Count());
    
        }
    }
}
