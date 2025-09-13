using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainLayer.models.AuthModles
{
    
    public class AuthResponse
    {
        public bool Success { get; set; } // Indicates success or failure
        public string Message { get; set; } // General message for the response

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Data { get; set; } // Optional payload (e.g., token details)

        public List<string> Errors { get; set; } = new List<string>(); // List of error messages
    }
    
}
