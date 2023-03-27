using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Repositories.Interfaces;

namespace ProiectLicenta.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            this._context = context;
        } 
        public T Add(T entity)
        {
           _context.Set<T>().Add(entity);
           Save();
           return entity;
        }
   
        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public T Update(T entity)
        {
            _context.Entry(entity).State =EntityState.Modified;
            Save();
            return entity;
        }
        public async void Save()
        {
            await _context.SaveChangesAsync();
        }

       

    }
}
