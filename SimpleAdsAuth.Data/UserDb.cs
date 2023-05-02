using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class UserDb
    {
        private readonly string _connectionString;

        public UserDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Users.Add(user);
            context.SaveChanges();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isValidPassword)
            {
                return user; //success!!
            }

            return null;
        }

        public User GetByEmail(string email)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool IsEmailAvailable(string email)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            return !context.Users.Any(u => u.Email == email);
        }
    }
}