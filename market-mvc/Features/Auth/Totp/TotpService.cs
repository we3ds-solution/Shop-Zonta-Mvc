using System.Security.Cryptography;
using System.Text;

namespace market_mvc.Features.Auth.Totp
{
    /// <summary>
    /// RFC 6238 TOTP (SHA-1, 30s step, 6 digits) with no external packages.
    /// Secrets are 160-bit Base32 strings shown once at setup; users enter them
    /// manually into any authenticator app (otpauth:// URL provided, no QR lib).
    /// </summary>
    public static class TotpService
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const int StepSeconds = 30;
        private const int Digits = 6;

        public static string GenerateSecret(int bytes = 20)
        {
            var data = RandomNumberGenerator.GetBytes(bytes);
            var sb = new StringBuilder();
            int buffer = 0, bits = 0;
            foreach (var b in data)
            {
                buffer = (buffer << 8) | b;
                bits += 8;
                while (bits >= 5)
                {
                    bits -= 5;
                    sb.Append(Alphabet[(buffer >> bits) & 31]);
                }
            }
            if (bits > 0) sb.Append(Alphabet[(buffer << (5 - bits)) & 31]);
            return sb.ToString();
        }

        public static string BuildKeyUri(string email, string issuer, string secret) =>
            $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
            $"?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&digits={Digits}&period={StepSeconds}";

        public static string ComputeCode(string base32Secret, DateTimeOffset time)
        {
            var key = FromBase32(base32Secret);
            var counter = (ulong)(time.ToUnixTimeSeconds() / StepSeconds);
            var msg = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian) Array.Reverse(msg);
            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(msg);
            var offset = hash[^1] & 0x0F;
            var code = ((hash[offset] & 0x7F) << 24) | (hash[offset + 1] << 16) | (hash[offset + 2] << 8) | hash[offset + 3];
            return (code % 1_000_000).ToString("D6");
        }

        /// <summary>Accepts codes from the previous, current, or next step (clock skew).</summary>
        public static bool VerifyCode(string base32Secret, string code, DateTimeOffset now)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != Digits || !code.All(char.IsDigit))
                return false;
            for (var step = -1; step <= 1; step++)
            {
                if (CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(ComputeCode(base32Secret, now.AddSeconds(step * StepSeconds))),
                    Encoding.ASCII.GetBytes(code)))
                    return true;
            }
            return false;
        }

        internal static byte[] FromBase32(string input)
        {
            var clean = new string(input.Trim().TrimEnd('=').ToUpperInvariant().Where(c => Alphabet.Contains(c)).ToArray());
            var output = new List<byte>();
            int buffer = 0, bits = 0;
            foreach (var c in clean)
            {
                buffer = (buffer << 5) | Alphabet.IndexOf(c);
                bits += 5;
                if (bits >= 8)
                {
                    bits -= 8;
                    output.Add((byte)((buffer >> bits) & 0xFF));
                }
            }
            return output.ToArray();
        }
    }
}
