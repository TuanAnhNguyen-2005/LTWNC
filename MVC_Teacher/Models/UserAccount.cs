using System;

namespace MVC_Teacher.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string UserName { get; set; }      // login name
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // SHA256 hash (example)
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}