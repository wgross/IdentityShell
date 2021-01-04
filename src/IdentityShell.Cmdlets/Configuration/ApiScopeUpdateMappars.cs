using AutoMapper;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityShell.Cmdlets.Configuration
{
    public static class ApiScopeUpdateMappars
    {
        static ApiScopeUpdateMappars()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IdentityServer4.EntityFramework.Entities.ApiScope ToEntity(this IdentityServer4.Models.ApiScope model, IdentityServer4.EntityFramework.Entities.ApiScope existingEntity)
        {
            return Mapper.Map(model, existingEntity);
        }
    }
}