
namespace Honamic.PayMaster.PaymentProvider.Core.Extensions;

public static class ExceptionExtensions
{
    public static ExceptionInfo ExceptionToExceptionInfo(this Exception exception)
    {
        return new ExceptionInfo(exception, true, true);
    }


    public class ExceptionInfo
    {
        public ExceptionInfo() { }

        internal ExceptionInfo(Exception exception, bool includeInnerException = true, bool includeStackTrace = false)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Type = exception.GetType().FullName;
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = includeStackTrace ? exception.StackTrace : null;
            if (includeInnerException && exception.InnerException is not null)
            {
                InnerException = new ExceptionInfo(exception.InnerException, includeInnerException, includeStackTrace);
            }
        }

        public string Type { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public ExceptionInfo InnerException { get; set; }
    }
}
