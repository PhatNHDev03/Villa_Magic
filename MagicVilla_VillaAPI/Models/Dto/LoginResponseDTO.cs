namespace MagicVilla_VillaAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string role { get; set; }
        public string Token { get; set; }
    }
}
