namespace ytgenerator.Data.Entities
{
    public class AccessToken : BaseEntity
    {
        public string Value { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsValid { get; set; } = true;
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
