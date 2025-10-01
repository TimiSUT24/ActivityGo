using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Läsning
        Task<TEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();

        // Skrivning (Dessa metoder spårar bara ändringar, de sparar inte)
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        // Lägg till en metod för att hitta baserat på uttryck
        Task<IEnumerable<TEntity>> FindAsync(
            System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
    }
}
