using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.Web.Models;

namespace MyCore.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpEmptyActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            actionResult.Result = new ObjectResult(new AjaxResponse());
        }
    }
}