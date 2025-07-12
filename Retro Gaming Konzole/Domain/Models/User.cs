using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Serializable]

    public class User
    {
        public string username { get; set; }
        public string password { get; set; }

        public UserRole role { get; set; }


        public User() { }
        public User(string username, string password, UserRole role) 
        { 
            this.username = username;
            this.password = password;
            this.role = role;
        }

    }
}
