namespace Umbraco9ContentApi.Core.Models.Response
{
    public class ApiResponse<T> where T : class
    {
        public virtual bool Succeeded { get; private set; }
        public virtual string Message { get; private set; }
        public virtual IEnumerable<string> Errors { get; private set; }
        public virtual T Data { get; private set; }
        public ApiResponse<T> Success(T data, string message)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Succeeded = true,
                Message = message
            };
        }
        public ApiResponse<T> Failure(IEnumerable<string> errors, string message)
        {
            return new ApiResponse<T>
            {
                Errors = errors,
                Message = message,
                Succeeded = false
            };
        }
    }
}