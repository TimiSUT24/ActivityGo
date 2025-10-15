using Application.Category.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Category.Mapper
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Domain.Models.Category, CategoryReadDto>();
            CreateMap<CategoryCreateDto, Domain.Models.Category>()
                .ForMember(d => d.IsActive, opt => opt.MapFrom(_ => true));
            CreateMap<CategoryUpdateDto, Domain.Models.Category>()
                .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember is not null));
        }
    }
}
