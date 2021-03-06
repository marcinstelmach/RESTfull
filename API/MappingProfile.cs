﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTO;
using Data.Model;

namespace API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year));
            CreateMap<Book, BookDto>();
            CreateMap<AuthorForCreation, Author>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.ParseExact(src.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture)));
            CreateMap<BookForCreation, Book>();
            //CreateMap<BookForUpdateDto, Book>()
            //    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
            CreateMap<BookForUpdateDto, Book>();
            CreateMap<Book, BookForUpdateDto>();
        }
    }
}
