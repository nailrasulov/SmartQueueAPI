namespace API.Models
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static BaseResponse<T> Ok(T data, string? message = null)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };  
        }

        public static BaseResponse<T> Fail(string message)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
            };
        }
    }

    public class BaseResponse : BaseResponse<object>
    {
        public static BaseResponse Ok(string? message = null)
        {
            return new BaseResponse
            {
                Success = true,
                Message = message
            };
        }

        public new static BaseResponse Fail(string message)
        {
            return new BaseResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}
