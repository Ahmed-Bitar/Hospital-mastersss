using Microsoft.AspNetCore.Mvc;

namespace MedicalPark.Servis
{
    public interface IMailService
    {
        Task<IActionResult> SendingTask(int id);
    }
}