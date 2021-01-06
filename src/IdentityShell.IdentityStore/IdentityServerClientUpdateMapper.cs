using AutoMapper;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityShell.IdentityStore
{
    public static class ClientUpdateMappers
    {
        static ClientUpdateMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IdentityServer4.EntityFramework.Entities.Client ToEntity(this IdentityServer4.Models.Client model, IdentityServer4.EntityFramework.Entities.Client existingEntity)
        {
            return Mapper.Map(model, existingEntity);
        }
    }
}