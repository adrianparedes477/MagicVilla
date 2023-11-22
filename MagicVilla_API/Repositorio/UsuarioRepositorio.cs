using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace MagicVilla_API.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private string secreteKey;
        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            secreteKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUsuarioUnico(string userName)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if(usuario == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() &&
                                                            u.Password == loginRequestDTO.Password);
            if(usuario == null)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    Usuario = null,
                };
            }
            var tokenHamdler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secreteKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHamdler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDTO = new()
            {
                Token = tokenHamdler.WriteToken(token),
                Usuario = usuario,
            };
            return loginResponseDTO;
        }

        public async Task<Usuario> Registrar(RegistroRequestDTO registroRequestDTO)
        {
            Usuario usuario = new()
            {
                UserName = registroRequestDTO.UserName,
                Password = registroRequestDTO.Password,
                Nombres = registroRequestDTO.Nombres,
                Rol = registroRequestDTO.Rol,
            };
            await _db.Usuarios.AddAsync(usuario);
            await _db.SaveChangesAsync();
            usuario.Password = "";
            return usuario;
        }
    }
}
