using AltaGasTest.Data.Entities;

namespace AltaGasTest.Data.Repository
{
    public interface ICanadianCityRepository
    {
        Task<IEnumerable<CanadianCity>> GetAllAsync();
    }
}
