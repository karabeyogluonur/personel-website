using PW.Application.Interfaces.Localization;
using PW.Domain.Common;
using PW.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PW.Application.Common.Extensions
{
    public static class LocalizationExtensions
    {
        public static async Task<IList<T>> ToLocalizedListAsync<T>(
            this IQueryable<T> source,
            ILocalizationService localizationService,
            int languageId)
            where T : BaseEntity, ILocalizedEntity
        {
            var entities = await source.ToListAsync();

            if (languageId == 0 || !entities.Any())
                return entities;

            string groupName = typeof(T).Name;
            var entityIds = entities.Select(e => e.Id).ToList();

            var allTranslations = await localizationService.GetTranslationsForListAsync(entityIds, groupName, languageId);

            foreach (var entity in entities)
            {
                var entityTranslations = allTranslations.Where(x => x.EntityId == entity.Id);

                foreach (var translation in entityTranslations)
                {
                    var prop = typeof(T).GetProperty(translation.LocaleKey);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(entity, translation.LocaleValue);
                    }
                }
            }

            return entities;
        }
    }
}
