using AutoMapper;
using OWP.AssignmentUploadNotification.Models;

namespace OWP.AssignmentUploadNotification
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ChangedValue, ChangedValue>();
        }
    }
}