namespace ytgenerator.Data.Entities
{
    public class Setting : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string OpenApiKey { get; set; }
        public string MapifyApiKey { get; set; }
        public string GoogleApiKey { get; set; }
    }
}
