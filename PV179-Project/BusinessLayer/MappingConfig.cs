using AutoMapper;
using BusinessLayer.DataTransferObjects;
using BusinessLayer.DataTransferObjects.QueryDTOs;
using BusinessLayer.Services.Implementations;
using DataAccessLayer.DataClasses;
using Infrastructure.Query;

namespace BusinessLayer
{
    public class MappingConfig
    {
        public static void ConfigureMap(IMapperConfigurationExpression config)
        {
            config.CreateMap<Trip, TripDto>().ReverseMap();
            config.CreateMap<User, UserDto>().ReverseMap();
            config.CreateMap<Review, ReviewDto>().ReverseMap();
            config.CreateMap<UserTrip, UserTripDto>().ReverseMap();
            config.CreateMap<Location, LocationDto>().ReverseMap();
            config.CreateMap<Challenge, ChallengeDto>().ReverseMap();
            config.CreateMap<TripLocation, TripLocationDto>().ReverseMap();
            config.CreateMap<UserReviewVote, UserReviewVoteDto>().ReverseMap();
        }
    }
}