using MedicalPark.Dbcontext;

namespace MedicalPark.Servis
{
    public class DeleteService : IDeleteService
    {
        private readonly HospitalDbContext _context;

        public DeleteService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAsync<T>(int id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
