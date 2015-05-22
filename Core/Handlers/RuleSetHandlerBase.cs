﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public class RuleSetHandlerBase<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        protected T _ruleRequest;
        protected Func<IQueryable<T>> _storageMechanism;

        protected Action<string, string> Logger;
        public string RuleSetName { get; set; }
        public string ErrorDescription { get; set; }

        public T RuleRequest
        {
            get { return _ruleRequest; }
        }

        public Type RuleType
        {
            get { return typeof (T); }
        }

        public IEnumerable<T> Items
        {
            get { return StorageMechanism().AsEnumerable(); }
        }

        public Action InvalidAction { get; set; }

        public Func<IQueryable<T>> StorageMechanism
        {
            get
            {
                if (_storageMechanism == null)
                {
                    Logger("WARNING", "No storage mechanism specified. Default to Cache storage");
                    UseCacheStorage();
                }
                return _storageMechanism;
            }
            set { _storageMechanism = value; }
        }

        protected RuleSetHandlerBase()
        {
            if (Logger == null)
            {
                Logger = (a, b) => Console.WriteLine(a + " :: " + b);
            }
            RuleSetName = RuleType.Name + " ruleset";
            ErrorDescription = RuleType.Name + " was invalid";

            if (InvalidAction == null)
            {
                InvalidAction = () => { throw new HttpException(403, ErrorDescription); };
            }

            UpdateRuleCollectionOnSuccess = true;
        }

        public void UseCacheStorage(string cacheName = null)
        {
            if (string.IsNullOrEmpty(cacheName))
            {
                cacheName = RuleType.Name;
            }

            var cache = MemoryCache.Default.Get(cacheName) as IQueryable<T>;
            if (cache == null)
            {
                MemoryCache.Default.Add(cacheName, new List<T>().AsQueryable(), null);
                cache = MemoryCache.Default.Get(cacheName) as IQueryable<T>;
            }
            _storageMechanism = () => cache;
        }

        public void AddExistingItems(IEnumerable<T> existingItems)
        {
            var enumerable = existingItems as T[] ?? existingItems.ToArray();
            if (existingItems != null && enumerable.Any())
            {
                Logger("DEBUG", "AddExistingItems: " + enumerable.Count());
                var storage = _storageMechanism().ToList();
                enumerable.ToList().ForEach(storage.Add);
                _storageMechanism = () => storage.AsQueryable();
            }
        }

        public void Log(Action<string, string> logger)
        {
            Logger = logger;
        }

        protected void RecordRequest(IRuleRequest request)
        {
            Logger("DEBUG", "RecordRequest: " + request.Id);
            if (!UpdateRuleCollectionOnSuccess) return;
            var item = (T) request;
            var storage = StorageMechanism().ToList();
            if (!storage.Contains(item))
            {
                storage.Add(item);
            }
            _storageMechanism = () => storage.AsQueryable();
        }

        public virtual void VerifyRule(IRuleRequest request = null)
        {
            if (request == null)
            {
                request = Activator.CreateInstance<T>();
            }
            else
            {
                RecordRequest(request);
            }

            _ruleRequest = (T) request;
        }

        public bool UpdateRuleCollectionOnSuccess { get; set; }

        protected void FailRule(string logmessage = null)
        {
            Logger("WARN", string.Format("FAILED RULE: {0} {1}", RuleSetName, logmessage));
            InvalidAction();
        }
    }
}