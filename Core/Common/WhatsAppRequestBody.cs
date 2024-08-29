namespace Core.Common
{
    public class WhatsAppRequestBody
    {
        public string from { get; set; }
        public string to { get; set; }
        public string message_type { get; set; }
        public string channel { get; set; }
        public string text { get; set; }
      
    }
    public class WhatsAppResponseBody
    {
        public int Status { get; set; }
        public string Data { get; set; }
    }
    public class NexmoSettings
    {
        public string from { get; set; }
        public string channel { get; set; }
        public string message_type { get; set; }
        public string uri { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
}
