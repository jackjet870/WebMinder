﻿using System;
using System.Linq;
using System.Web;
using WebMinder.Core.Handlers;
using Xunit;

namespace WebMinder.Core.Tests.Handlers
{
    public class MaximumCountRuleSetHandlerTests : HandlerFixtureBase
    {

        [Fact]
        public void ShouldThrowCustomAction()
        {
            const int count = 5;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 5,
                InvalidAction = () => { throw new DivideByZeroException(ErrorDescription); },
            };
            ruleSetHandler.UseMemoryCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs.Skip(1))
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Throws<DivideByZeroException>(() => ruleSetHandler.VerifyRule(TestObject.Build()));
        }


        [Fact]
        public void ShouldCollectDataResults()
        {
            const int count = 3;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 999,
            };

            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }
            Assert.Equal(count, ruleSetHandler.Items.Count());
        }

        [Fact]
        public void ShouldNotAddToCollectionWhenUpdateRuleCollectionOnSuccessSetToFalse()
        {
            const int count = 3;
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 2,
                UpdateRuleCollectionOnSuccess = false
            };

            ruleSetHandler.UseMemoryCacheStorage(Guid.NewGuid().ToString());
            var stubs = AddTestObjects(count);
            foreach (var st in stubs)
            {
                ruleSetHandler.VerifyRule(st);
            }

            Assert.Equal(0, ruleSetHandler.Items.Count());
        }


        [Fact]
        public void ShouldThrowIfMaxCountExceeded()
        {
            var ruleSetHandler = new MaximumCountRuleSetHandler<TestObject>
            {
                RuleSetName = RuleSet,
                ErrorDescription = ErrorDescription,
                MaximumResultCount = 3
            };
            ruleSetHandler.UseMemoryCacheStorage(Guid.NewGuid().ToString());
            const int count = 3;
            var stubs = AddTestObjects(count);
            foreach (var st in stubs.Skip(1))
            {
                ruleSetHandler.VerifyRule(st);
            }
            Assert.Throws<HttpException>(() => ruleSetHandler.VerifyRule(stubs.First()));
        }


    }
}
