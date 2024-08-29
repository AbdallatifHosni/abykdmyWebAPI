namespace AbyKhedma.Entities
{
    public class UserNotification : BaseEntity
    {
        public bool IsRead { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
