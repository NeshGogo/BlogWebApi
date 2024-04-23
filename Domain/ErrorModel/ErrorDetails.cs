using System.Text.Json;


namespace Domain.ErrorModel
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        /// <summary>
        /// Return a JSON that represent the object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonSerializer.Serialize(this);        
    }
}
