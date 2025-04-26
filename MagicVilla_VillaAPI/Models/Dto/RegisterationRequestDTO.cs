using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class RegisterationRequestDTO
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string  Password { get; set; }
        [AllowNull]
        public string Role { get; set; }
    }
}
