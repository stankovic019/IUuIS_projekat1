using Domain.Enums;
using Domain.Helpers;
using Domain.Models;
using Domain.Services;
using System.Collections.ObjectModel;

namespace Services.LoginService
{
    public class LoginService : ILoginService
    {

        public ObservableCollection<User> users;
        private DataIO serializer = new DataIO();

        public LoginService()
        {

            users = serializer.DeSerializeObject<ObservableCollection<User>>("Users.xml");
            if (users == null)
            {
                users = new ObservableCollection<User>();
            }

        }

        public (bool, User, LoginErrors) login(string username, string password)
        {
            User us = null;
            LoginErrors error = LoginErrors.noError;

            foreach (User user in users)
            {
                if (user.username.Equals(username))
                {
                    if (user.password.Equals(password))
                    { //if both are correct, return the user that you find
                        us = user;
                        error = LoginErrors.noError;
                        return (true, us, error);
                    }
                    else
                    {   //if username is correct, but password isn't, the error is in the password
                        error = LoginErrors.password;
                    }
                }
            }

            //if it breaks from "foreach", it means that error is in the username
            if (error != LoginErrors.password) error = LoginErrors.username;
            us = null;
            return (false, us, error);

        }
    }
}
