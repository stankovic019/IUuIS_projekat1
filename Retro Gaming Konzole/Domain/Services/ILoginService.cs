using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ILoginService
    {

        public (bool, User, LoginErrors) login(string username, string password);

    }
}
