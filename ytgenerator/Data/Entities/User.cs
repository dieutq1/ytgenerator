using ytgenerator.Shared;

namespace ytgenerator.Data.Entities
{
    public class User : BaseEntity
    {
        public string PassWordHash { get; set; }
        public string Email { get; set; }
        public SystemRole Role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public ICollection<AccessToken> AccessTokens { get; set; }
        public Setting Setting { get; set; }
        public Guid? SettingId { get; set; }
    }
}
