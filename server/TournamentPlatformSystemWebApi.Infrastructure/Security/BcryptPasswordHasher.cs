// using BCrypt.Net;

// namespace TournamentPlatformSystemWebApi.Infrastructure.Security
// {
//     public class BcryptPasswordHasher : IPasswordHasher
//     {
//         private readonly int _workFactor;

//         public BcryptPasswordHasher(int workFactor = 12)
//         {
//             _workFactor = workFactor;
//         }

//         public string HashPassword(string password)
//         {
//             return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
//         }

//         public bool VerifyPassword(string password, string hash)
//         {
//             return BCrypt.Net.BCrypt.Verify(password, hash);
//         }
//     }
// }
