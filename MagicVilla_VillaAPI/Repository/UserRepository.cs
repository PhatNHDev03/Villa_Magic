using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
        }
        public bool IsUniqueUser(string userName)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.UserName == userName);
            return user == null ? true : false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null||isValid ==false)
            {
                return new LoginResponseDTO
                {
                    Token = "",
                    User = null
                };
            }
            //if user was founded generate JWT token
            var roles = await _userManager.GetRolesAsync(user);

            var tokenHander = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokentDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHander.CreateToken(tokentDescriptor);
            LoginResponseDTO response = new LoginResponseDTO
            {
                Token = tokenHander.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                role = roles.FirstOrDefault()
            };
            return response;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
           // Đảm bảo sử dụng DbContext trước khi các thao tác khác có thể dispose nó
ApplicationUser user = new()
{
    UserName = registerationRequestDTO.UserName,
    Email = registerationRequestDTO.UserName,
    NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
    Name = registerationRequestDTO.Name
};

try
{
    var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
    if (result.Succeeded)
    {
                    if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult()) {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }
        await _userManager.AddToRoleAsync(user, "Admin");
        // Sử dụng UserManager thay vì DbContext trực tiếp
        var userToReturn = await _userManager.FindByNameAsync(registerationRequestDTO.UserName);
        return _mapper.Map<UserDTO>(userToReturn);
    }
}
catch (Exception e)
{
    // Log exception
    Console.WriteLine(e.Message);
}

            return new UserDTO();
        }


        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

     
    }
}
