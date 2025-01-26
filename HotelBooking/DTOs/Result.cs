namespace HotelBooking.DTOs;

public class Result
{
    public string? ErrorMessage { get; set; }

    public bool IsSuccess { get; set; }

    public int StatusCode { get; set; }

    public static Result CreateError(string errorMessage, int statusCode)
    {
        return new Result { ErrorMessage = errorMessage, IsSuccess = false, StatusCode = statusCode };
    }

    public static Result CreateSuccess(int statusCode = 200)
    {
        return new Result { ErrorMessage = string.Empty, IsSuccess = true, StatusCode = statusCode };
    }

    public static Result<T> CreateError<T>(string errorMessage, int statusCode)
    {
        return new Result<T> { ErrorMessage = errorMessage, IsSuccess = false, StatusCode = statusCode, Data = default };
    }

    public static Result<T> CreateSuccess<T>(T data, int statusCode = 200)
    {
        return new Result<T> { ErrorMessage = string.Empty, IsSuccess = true, StatusCode = statusCode, Data = data };
    }
}

public class Result<TObject> : Result
{
    public TObject? Data { get; set; }

    public Result<TReturn> To<TReturn>(Func<TObject, TReturn>? castingCall = null) where TReturn : class
    {
        return new Result<TReturn>
        {
            ErrorMessage = this.ErrorMessage,
            IsSuccess = this.IsSuccess,
            StatusCode = this.StatusCode,
            Data = this.Data != null && castingCall != null ? castingCall(this.Data) : null
        };
    }
}

public static class ResultExtensions
{
    public static async Task<IResult> ToResultAsync(this Task<Result> task)
    {
        var result = await task;

        return result.IsSuccess
            ? (result.StatusCode == StatusCodes.Status200OK
                ? Results.Ok()
                : Results.StatusCode(result.StatusCode))
            : Results.Problem(detail: result.ErrorMessage, statusCode: result.StatusCode);
    }

    public static async Task<IResult> ToResultAsync<T>(this Task<Result<T>> task)
    {
        var result = await task;

        return result.IsSuccess
            ? (result.StatusCode == StatusCodes.Status200OK
                ? Results.Ok(result.Data)
                : Results.Json(result.Data, statusCode: result.StatusCode))
            : Results.Problem(detail: result.ErrorMessage, statusCode: result.StatusCode);
    }
}