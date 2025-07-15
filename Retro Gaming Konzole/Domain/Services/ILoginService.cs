using Domain.Enums;
using Domain.Models;

namespace Domain.Services
{
    public interface ILoginService
    {

        public (bool, User, LoginErrors) login(string username, string password);

    }
}
