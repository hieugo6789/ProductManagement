﻿using System.Security.Cryptography;
using System.Text;

namespace SE170311.Lab3.Utils
{
    public static class PasswordUtil
    {
        public static string HashPassword(string rawPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
