﻿using WebMinder.Core.Builders;
using WebMinder.Core.Rules.IpBlocker;

namespace WebMinder.Core.RuleSets
{
    public class IpBlockerRuleSet : CreateRule<IpAddressBlockerRuleSetHandler, IpAddressRequest>
    {
        public IpBlockerRuleSet()
        {
            this.SetRule(CreateRule<IpAddressBlockerRuleSetHandler, IpAddressRequest>.On<IpAddressRequest>().Build().Rule);
        }
    }
}
