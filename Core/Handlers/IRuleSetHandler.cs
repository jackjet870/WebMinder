﻿using System;
using System.Collections.Generic;

namespace WebMinder.Core.Handlers
{
    public interface IRuleSetHandler<T> : IRuleRunner where T : IRuleRequest, new()
    {
        string RuleSetName { get; set; }

        string ErrorDescription { get; set; }

        Type RuleType { get; }

        IEnumerable<T> Items { get; }

        T RuleRequest { get; }

        Func<IList<T>> StorageMechanism { get; set; }

        Action InvalidAction { get; set; }

        void UseCacheStorage(string cacheName = null);

        void AddExistingItems(IEnumerable<T> existingItems);

        void Log(Action<string, string> logger);

        void VerifyRule(IRuleRequest request = null);

        bool UpdateRuleCollectionOnSuccess { get; set; }
    }
}