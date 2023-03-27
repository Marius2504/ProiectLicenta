namespace ProiectLicenta.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetAll();
        T Add(T entity);
        T Update(T entity);
    }
}
