using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenSerervice
{
Task<string> CreateToken(AppUser user);
string GenerateRefreshToken();

}
