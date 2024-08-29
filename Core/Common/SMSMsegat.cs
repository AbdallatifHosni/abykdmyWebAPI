namespace Core.Common
{
    public class SMSMsegat
    {
        public string UserName { get; set; }
        public string UserSender { get; set; }
        public string ApiKey { get; set; }
        public string Uri { get; set; }
    }
     public class SmsMsegatRequestBody
    {
        public string userName { get; set; }
        public string userSender { get; set; }
        public string apiKey { get; set; }
        public string numbers { get; set; }
        public string msg { get; set; }
    }
    public class SmsMsegatResponseBody
    {
        public int Status { get; set; }
        public string Headers { get; set; }
        public string Data { get; set; }
    }
}
