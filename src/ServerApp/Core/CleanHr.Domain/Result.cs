using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CleanHr.Domain;

public class Result
{
    public bool IsSuccess { get; }

    public string Error { get; }

    public Dictionary<string, string> Errors { get; }

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = new Dictionary<string, string>() { { "", error } };
    }

    protected Result(bool isSuccess, string errorKey, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = new Dictionary<string, string> { { errorKey, error } };
    }

    protected Result(bool isSuccess, Dictionary<string, string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? [];
        Error = string.Empty;

        if (Errors.Count > 0)
        {
            Error = string.Join("; ", errors.Select(e => $"{e.Key}: {e.Value}"));
        }
    }

    protected Result(bool isSuccess, IDictionary<string, string[]> errors)
    {
        IsSuccess = isSuccess;
        Errors = new Dictionary<string, string>();
        Error = string.Empty;

        if (errors != null && errors.Count > 0)
        {
            Errors = errors.ToDictionary(
                kvp => kvp.Key,
                kvp => string.Join(", ", kvp.Value));

            Error = string.Join("; ", Errors.Select(e => $"{e.Key}: {e.Value}"));
        }
    }

    public static Result Success() => new(true, string.Empty);

    public static Result Failure(string error) => new(false, error);

    public static Result Failure(string erroKey, string error) => new(false, erroKey, error);

    public static Result Failure(Dictionary<string, string> errors) => new(false, errors);

    public static Result Failure(IDictionary<string, string[]> errors) => new(false, errors);
}

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
public class Result<T> : Result
{
    public T Value { get; }

    private Result(bool isSuccess, string error, T value)
        : base(isSuccess, error)
    {
        Value = value;
    }

    private Result(bool isSuccess, string errorKey, string error, T value)
        : base(isSuccess, errorKey, error)
    {
        Value = value;
    }

    private Result(bool isSuccess, Dictionary<string, string> errors, T value)
       : base(isSuccess, errors)
    {
        Value = value;
    }

    private Result(bool isSuccess, IDictionary<string, string[]> errors, T value)
      : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) =>
        new(true, string.Empty, value);

    public static new Result<T> Failure(string error) =>
        new(false, error, default!);

    public static new Result<T> Failure(string errorKey, string error) =>
       new(false, errorKey, error, default!);

    public static new Result<T> Failure(Dictionary<string, string> errors) =>
        new(false, errors, default!);

    public static new Result<T> Failure(IDictionary<string, string[]> errors) =>
        new(false, errors, default!);
}

