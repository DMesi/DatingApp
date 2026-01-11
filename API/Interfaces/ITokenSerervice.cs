using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenSerervice
{
string CreateToken(AppUser user);

}
