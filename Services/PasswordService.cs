using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Helluz.Services
{
    public class PasswordService
    {
        /// <summary>
        /// Encripta una contraseña usando PBKDF2 con salt aleatorio
        /// </summary>
        public string HashPassword(string password)
        {
            // Generar salt aleatorio de 128 bits
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            // Derivar hash de 256 bits con 100,000 iteraciones
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Retornar salt + hash separados por punto
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado
        /// </summary>
        public bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // Separar salt y hash
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var hash = parts[1];

                // Generar hash con la misma sal
                string hashToCompare = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                // Comparar hashes
                return hash == hashToCompare;
            }
            catch
            {
                return false;
            }
        }
    }
}