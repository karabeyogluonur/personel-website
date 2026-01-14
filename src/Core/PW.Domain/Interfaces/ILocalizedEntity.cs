namespace PW.Domain.Interfaces;

public interface ILocalizedEntity<TTranslation> where TTranslation : class
{
   ICollection<TTranslation> Translations { get; set; }
}
