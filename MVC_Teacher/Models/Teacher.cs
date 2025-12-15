using System;

namespace MVC_Teacher.Models
{
    // Expanded POCO to represent a Teacher used by views/controllers
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Additional fields referenced by views
        public string Subject { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
    }
}
