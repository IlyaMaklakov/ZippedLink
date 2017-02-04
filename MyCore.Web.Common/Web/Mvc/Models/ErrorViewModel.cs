using System;

using MyCore.Web.Models;

namespace MyCore.Web.Mvc.Models
{
    public class ErrorViewModel
    {
        public ErrorInfo ErrorInfo { get; set; }

        public Exception Exception { get; set; }

        public ErrorViewModel()
        {
            
        }

        public ErrorViewModel(ErrorInfo errorInfo, Exception exception = null)
        {
            this.ErrorInfo = errorInfo;
            this.Exception = exception;
        }
    }
}
