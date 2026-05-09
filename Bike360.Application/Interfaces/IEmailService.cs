using System.Threading.Tasks;

namespace Bike360.Application.Interfaces
{
   
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}