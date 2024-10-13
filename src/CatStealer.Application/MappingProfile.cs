using AutoMapper;
using CatStealer.Application.DTOs;
using CatStealer.Application.Models;
using CatStealer.Core.Entities;
using CatStealer.Core.Results;

namespace CatStealer.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatEntity, CatDto>();
            CreateMap<TagEntity, TagDto>();
            CreateMap<CatsResult, CatsResponse>();
        }
    }
}
