using AutoMapper;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityShell.IdentityStore
{
    public static class ApiResourceUpdateMappers
    {
        static ApiResourceUpdateMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IdentityServer4.EntityFramework.Entities.ApiResource ToEntity(this IdentityServer4.Models.ApiResource model, IdentityServer4.EntityFramework.Entities.ApiResource existingEntity)
        {
            return Mapper.Map(model, existingEntity);
        }
    }
}