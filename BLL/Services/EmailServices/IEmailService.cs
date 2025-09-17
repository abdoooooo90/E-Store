namespace BLL.Services.EmailServices
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string resetLink);
        Task<bool> SendWelcomeEmailAsync(string email, string fullName);
        Task<bool> SendOrderConfirmationEmailAsync(string email, string orderNumber, decimal totalAmount);
    }
}
