namespace Interconn.common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; } = true;
        public bool NotFound { get; set; } = false;
        public string Message { get; set; }


        public static ServiceResult Success(string message = "") => new ServiceResult { IsSuccess = true, Message = message };
        public static ServiceResult Fail(string message = "") => new ServiceResult { IsSuccess = false, Message = message };
        public static ServiceResult NotFoundResult(string message = "") => new ServiceResult { IsSuccess = false, NotFound = true, Message = message };
    }

}
