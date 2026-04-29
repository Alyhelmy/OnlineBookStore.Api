namespace OnlineBookStore.Api.Shared.Helpers
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; } // Generic property to hold any type of data

        public static ServiceResult<T> Success(T data, string message = "")  // Static method to create a successful result with data and an optional message 
        {
            return new ServiceResult<T> 
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string message) 
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };


        }
    }
}