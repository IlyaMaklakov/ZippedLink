using Microsoft.AspNetCore.Mvc.Filters;

namespace MyCore.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapper
    {
        void Wrap(ResultExecutingContext actionResult);
    }
}