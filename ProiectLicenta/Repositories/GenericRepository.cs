using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.Repositories.Interfaces;

namespace ProiectLicenta.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            this._context = context;
        } 
        public async Task<T> Add(T entity)
        {
           await _context.Set<T>().AddAsync(entity);
           Save();
           return entity;
        }
        public async Task<T?> Get(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
   
        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T?> Update(T entity)
        {
            _context.Entry(entity).State =EntityState.Modified;
            Save();
            return entity;
        }
        public async Task<T?> Delete(int id)
        {
            var obj =await _context.Set<T>().FindAsync(id);
            if(obj != null)
            {
                _context.Set<T>().Remove(obj);
            }
            Save();
            return obj;
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

       

    }
}
