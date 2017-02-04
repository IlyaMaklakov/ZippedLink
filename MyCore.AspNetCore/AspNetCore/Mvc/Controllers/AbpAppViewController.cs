using System;

using Microsoft.AspNetCore.Mvc;

using MyCoreFramework.Auditing;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.Extensions;
using MyCoreFramework.Runtime.Validation;

namespace MyCore.AspNetCore.Mvc.Controllers
{
    public class AbpAppViewController : AbpController
    {
        [DisableAuditing]
        [DisableValidation]
        [UnitOfWork(IsDisabled = true)]
        public ActionResult Load(string viewUrl)
        {
            if (viewUrl.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(viewUrl));
            }

            return this.View(viewUrl.EnsureStartsWith('~'));
        }
    }
}
