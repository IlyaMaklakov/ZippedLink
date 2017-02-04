using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

using Castle.Core.Logging;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Runtime.Session;
using MyCoreFramework.Timing;

namespace MyCoreFramework.Auditing
{
    public class AuditingHelper : IAuditingHelper, ITransientDependency
    {
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }
        public IAuditingStore AuditingStore { get; set; }

        private readonly IAuditInfoProvider _auditInfoProvider;
        private readonly IAuditingConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAuditSerializer _auditSerializer;

        public AuditingHelper(
            IAuditInfoProvider auditInfoProvider, 
            IAuditingConfiguration configuration, 
            IUnitOfWorkManager unitOfWorkManager,
            IAuditSerializer auditSerializer)
        {
            this._auditInfoProvider = auditInfoProvider;
            this._configuration = configuration;
            this._unitOfWorkManager = unitOfWorkManager;
            this._auditSerializer = auditSerializer;

            this.AbpSession = NullAbpSession.Instance;
            this.Logger = NullLogger.Instance;
            this.AuditingStore = SimpleLogAuditingStore.Instance;
        }

        public bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (!this._configuration.IsEnabled)
            {
                return false;
            }

            if (!this._configuration.IsEnabledForAnonymousUsers && (this.AbpSession?.UserId == null))
            {
                return false;
            }

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.IsDefined(typeof(AuditedAttribute), true))
                {
                    return true;
                }

                if (classType.IsDefined(typeof(DisableAuditingAttribute), true))
                {
                    return false;
                }

                if (this._configuration.Selectors.Any(selector => selector.Predicate(classType)))
                {
                    return true;
                }
            }

            return defaultValue;
        }

        public AuditInfo CreateAuditInfo(MethodInfo method, object[] arguments)
        {
            return this.CreateAuditInfo(method, CreateArgumentsDictionary(method, arguments));
        }

        public AuditInfo CreateAuditInfo(MethodInfo method, IDictionary<string, object> arguments)
        {
            var auditInfo = new AuditInfo
            {
                TenantId = this.AbpSession.TenantId,
                UserId = this.AbpSession.UserId,
                ImpersonatorUserId = this.AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = this.AbpSession.ImpersonatorTenantId,
                ServiceName = method.DeclaringType != null
                    ? method.DeclaringType.FullName
                    : "",
                MethodName = method.Name,
                Parameters = this.ConvertArgumentsToJson(arguments),
                ExecutionTime = Clock.Now
            };

            this._auditInfoProvider.Fill(auditInfo);

            return auditInfo;
        }

        public void Save(AuditInfo auditInfo)
        {
            using (var uow = this._unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                this.AuditingStore.Save(auditInfo);
                uow.Complete();
            }
        }

        public async Task SaveAsync(AuditInfo auditInfo)
        {
            using (var uow = this._unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await this.AuditingStore.SaveAsync(auditInfo);
                await uow.CompleteAsync();
            }
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    if (argument.Value != null && this._configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return this._auditSerializer.Serialize(dictionary);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }

        private static Dictionary<string, object> CreateArgumentsDictionary(MethodInfo method, object[] arguments)
        {
            var parameters = method.GetParameters();
            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < parameters.Length; i++)
            {
                dictionary[parameters[i].Name] = arguments[i];
            }

            return dictionary;
        }
    }
}