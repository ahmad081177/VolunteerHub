using System;
using BCrypt.Net;

namespace VolunteerHub.Helpers
{
    public static class AuthHelper
    {
        // BCrypt work factor: 2^12 = 4 096 hashing rounds.
        // Higher = slower to crack via brute force, but also slower to compute on login.
        // 12 is the recommended minimum for production as of 2024.
        private const int WorkFactor = 12;

        public static string HashPassword(string plain)
            => BCrypt.Net.BCrypt.HashPassword(plain, WorkFactor);

        public static bool VerifyPassword(string plain, string hash)
        {
            if (string.IsNullOrEmpty(plain) || string.IsNullOrEmpty(hash)) return false;
            // BCrypt.Verify can throw FormatException when the stored hash is malformed or truncated.
            // Catching here prevents a 500 error — a bad hash is treated as a failed login.
            try { return BCrypt.Net.BCrypt.Verify(plain, hash); }
            catch { return false; }
        }

        // Guid "N" format = 32 lowercase hex chars with no dashes, e.g. "a1b2c3d4e5f6...".
        // Stored in the DB and sent as the vh_remember cookie value.
        public static string GenerateRememberMeToken()
            => Guid.NewGuid().ToString("N");
    }
}
