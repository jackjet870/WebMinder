﻿using System;

using System.Linq;

namespace WebMinder.Core.Handlers
{
    public class MaximumCountRuleSetHandler<T> :
        RuleSetHandlerBase<T>,
        IRuleSetHandler<T>,
        IMaximumCountRuleSetHandler<T>
        where T : IRuleRequest, new()
    {

        public int? MaximumResultCount { get; set; }

        public override void VerifyRule(IRuleRequest request = null)
        {
            base.VerifyRule(request);
            if (!StorageMechanism().Any()) return;
            if (MaximumResultCount.HasValue && StorageMechanism().Count() >= MaximumResultCount)
            {
                _logger("WARN", "Rule Failed VerifyMaximumCount");
                InvalidAction();
            }
        }
    }
}