using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Mapper
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
                CreateMap<DTO.LoginDto, Domain.Models.User>()
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        }
    }
}
