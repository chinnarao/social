using Microsoft.AspNetCore.Identity;

namespace identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialId { get; set; }
        public string PictureUrl { get; set; }
        public string SocialType { get; set; } // it could be : facebook, twitter, linkedin, instagram
    }
}
