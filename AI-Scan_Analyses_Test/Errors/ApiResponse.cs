
namespace Talabat.API.Errors
{
    public class ApiResponse
    {
        public int StatuesCode { get; set; }

        public string? Message { get; set; }

        public ApiResponse(int statuesCode, string? message = null)
        {
            StatuesCode = statuesCode;
            Message = message ?? GetMessageFromStatuesCode(statuesCode);
        }

        private string? GetMessageFromStatuesCode(int statuesCode)
        {
            return statuesCode switch
            {
                400 => "A Bad Request,you have Made",
                401 => "Authorize , you are not",
                404 => "Resources we are Not Found",
                500 => "the Server is no work",
                 _  => null
            };
        }
    }
}
