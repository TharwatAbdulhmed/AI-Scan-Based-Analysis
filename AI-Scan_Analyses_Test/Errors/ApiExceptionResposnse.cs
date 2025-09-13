namespace Talabat.API.Errors
{
    public class ApiExceptionResponse:ApiResponse
    {
        public string? Details { get; set; }

        public ApiExceptionResponse(int statuesCode,string? message= null,string? details=null ):base(statuesCode,message)
        {
            Details = details;
        }
    }
}
