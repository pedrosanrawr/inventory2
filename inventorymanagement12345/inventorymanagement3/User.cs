using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventorymanagement3
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserIdentifier { get; set; }
        public bool IsAuthenticated { get; set; }

        public User()
        {
            IsAuthenticated = false;
        }

        public User(string userIdentifier)
        {
            UserIdentifier = userIdentifier;
        }
    }
}
