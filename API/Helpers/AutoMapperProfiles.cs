﻿using API.Data.Entities;
using API.DTOs;
using API.Extensions;
using AutoMapper;
using System.Linq;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ReverseMap();

            CreateMap<Photo, PhotoDto>().ReverseMap();
            CreateMap<MemberUpdateDto, AppUser>().ReverseMap();
            CreateMap<RegisterDto, AppUser>().ReverseMap();
        }
    }
}
