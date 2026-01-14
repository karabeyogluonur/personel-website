using PW.Application.Common.Interfaces;
using PW.Domain.Interfaces;

namespace PW.Application.Common.Extensions;

public static class TranslationExtensions
{
   public static void SyncTranslations<TTranslation, TDto>(
       this ICollection<TTranslation> databaseTranslations,
       IEnumerable<TDto> translationDtos,
       Func<TDto, bool> isEmptyPredicate,
       Action<TTranslation, TDto> mapAction)

       where TTranslation : class, IEntityTranslation, new()
       where TDto : ITranslationDto
   {
      if (translationDtos == null) return;

      foreach (TDto translationDto in translationDtos)
      {
         TTranslation? existingTranslation = databaseTranslations.FirstOrDefault(translation => translation.LanguageId == translationDto.LanguageId);

         bool isEmpty = isEmptyPredicate(translationDto);

         if (isEmpty)
         {
            if (existingTranslation != null)
               databaseTranslations.Remove(existingTranslation);
         }
         else
         {
            if (existingTranslation != null)
               mapAction(existingTranslation, translationDto);
            else
            {
               TTranslation newTranslation = new TTranslation
               {
                  LanguageId = translationDto.LanguageId
               };

               mapAction(newTranslation, translationDto);
               databaseTranslations.Add(newTranslation);
            }
         }
      }
   }
}
