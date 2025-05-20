namespace MedicalPark.Servis
{
    public interface IDeleteService
    {
        Task<bool> DeleteAsync<T>(int id) where T : class;
    }
}
