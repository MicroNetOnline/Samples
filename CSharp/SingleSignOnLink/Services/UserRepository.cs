using System;
using System.Collections.Concurrent;
using SingleSignOnLink.Models;

namespace SingleSignOnLink.Services
{
    public sealed class UserRepository
    {
        readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        public User Find(string email)
        {
            if (_users.TryGetValue(email, out var user))
            {
                return user;
            }

            return null;
        }

        public void Save(User user)
        {
            _users.TryAdd(user.Email, user);
        }
    }
}