using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Statistics.DTO;
using Domain.Reporting;

namespace Application.Statistics.Mapper
{
    public class StatisticsProfile : Profile
    {
        public StatisticsProfile()
        {
            // Domain.Reporting -> Application DTOs
            CreateMap<CountBucket, BookingsPerBucketDto>()
                .ForMember(d => d.Bucket, m => m.MapFrom(s => s.Bucket))
                .ForMember(d => d.Count,  m => m.MapFrom(s => s.Count));

            CreateMap<RevenueBucket, RevenuePerBucketDto>()
                .ForMember(d => d.Bucket,  m => m.MapFrom(s => s.Bucket))
                .ForMember(d => d.Revenue, m => m.MapFrom(s => s.Revenue));

            CreateMap<TopItem, TopItemDto>()
                .ForMember(d => d.Id,    m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Name,  m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Count, m => m.MapFrom(s => s.Count));
        }
    }
}
