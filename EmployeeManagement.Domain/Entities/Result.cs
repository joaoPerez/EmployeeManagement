namespace EmployeeManagement.Domain
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public List<string> Errors { get; }

        private Result(T? value, bool isSuccess, List<string> errors)
        {
            Value = value;
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result<T> Success(T value) =>
            new(value, true, []);

        public static Result<T> Failure(List<string> errors) =>
            new(default, false, errors);
    }
}