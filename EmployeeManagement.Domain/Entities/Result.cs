namespace EmployeeManagement.Domain.Entities
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public List<string> Errors { get; }

        protected Result(bool isSuccess, T? value, List<string> errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            Errors = errors;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, new List<string>());
        public static Result<T> Fail(List<string> errors) => new Result<T>(false, default, errors);
        public static Result<T> Fail(string error) => new Result<T>(false, default, new List<string> { error });
    }
}