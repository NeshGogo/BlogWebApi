using System.Text.Json;
using System.Text.Json.Serialization;


namespace Domain.ErrorModel
{
    public class ErrorDetails
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Return a JSON that represent the object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonSerializer.Serialize(this);        
    }
}
