using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }   
        public bool IsFailure => !IsSuccess;    

        protected Result(bool isSuccess, string? error)
        {
            if(isSuccess && error != null)
                throw new InvalidOperationException("Ugurlu netice xeta mesaji gostere bilmez");

            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Ugursuz netice xeta mesaji gostermelidir");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T data) => new(data, true, null);
        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        protected internal Result(T? data, bool isSuccess, string? error) : base(isSuccess, error)
        {
            Data = data;    
        }
    }
}
