using AutoMapper;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityShell.Data
{
    public static class IdentityResourceUpdateMappers
    {
        static IdentityResourceUpdateMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IdentityServer4.EntityFramework.Entities.IdentityResource ToEntity(this IdentityServer4.Models.IdentityResource model, IdentityServer4.EntityFramework.Entities.IdentityResource existingEntity)
        {
            return Mapper.Map(model, existingEntity);
        }
    }
}