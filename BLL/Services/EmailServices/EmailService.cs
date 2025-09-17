using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace BLL.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Furni Store", "noreply@furnistore.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "إعادة تعيين كلمة المرور - Furni Store";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = CreatePasswordResetEmailTemplate(resetLink)
                };
                message.Body = bodyBuilder.ToMessageBody();

                // Try to send email using SMTP
                try
                {
                    using var client = new SmtpClient();
                    
                    // For development, we'll use a fake SMTP or log the email
                    // In production, configure these settings in appsettings.json
                    var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                    var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                    var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
                    var smtpPass = _configuration["EmailSettings:SmtpPassword"] ?? "";

                    if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                    {
                        // Development mode - just log the email
                        _logger.LogInformation($"=== EMAIL WOULD BE SENT ===");
                        _logger.LogInformation($"To: {email}");
                        _logger.LogInformation($"Subject: {message.Subject}");
                        _logger.LogInformation($"Reset Link: {resetLink}");
                        _logger.LogInformation($"=== END EMAIL ===");
                        return true;
                    }

                    await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    _logger.LogInformation($"Password reset email sent successfully to: {email}");
                    return true;
                }
                catch (Exception smtpEx)
                {
                    _logger.LogWarning(smtpEx, $"SMTP failed, falling back to logging for: {email}");
                    
                    // Fallback - log the email details
                    _logger.LogInformation($"=== EMAIL FALLBACK ===");
                    _logger.LogInformation($"To: {email}");
                    _logger.LogInformation($"Reset Link: {resetLink}");
                    _logger.LogInformation($"=== END EMAIL ===");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {email}");
                return false;
            }
        }

        private string CreatePasswordResetEmailTemplate(string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html dir='rtl' lang='ar'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>إعادة تعيين كلمة المرور</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .content {{ padding: 30px; }}
        .button {{ display: inline-block; background-color: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #6c757d; font-size: 14px; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🛋️ Furni Store</h1>
            <h2>إعادة تعيين كلمة المرور</h2>
        </div>
        <div class='content'>
            <h3>مرحباً!</h3>
            <p>لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك في Furni Store.</p>
            <p>للمتابعة، يرجى النقر على الزر أدناه:</p>
            
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>إعادة تعيين كلمة المرور</a>
            </div>
            
            <div class='warning'>
                <strong>⚠️ تنبيه أمني:</strong>
                <ul>
                    <li>هذا الرابط صالح لمدة ساعة واحدة فقط</li>
                    <li>إذا لم تطلب إعادة تعيين كلمة المرور، يرجى تجاهل هذه الرسالة</li>
                    <li>لا تشارك هذا الرابط مع أي شخص آخر</li>
                </ul>
            </div>
            
            <p>إذا لم تتمكن من النقر على الزر، يمكنك نسخ الرابط التالي ولصقه في متصفحك:</p>
            <p style='word-break: break-all; background-color: #f8f9fa; padding: 10px; border-radius: 5px; font-family: monospace;'>{resetLink}</p>
        </div>
        <div class='footer'>
            <p>شكراً لاختيارك Furni Store</p>
            <p>فريق خدمة العملاء</p>
        </div>
    </div>
</body>
</html>";
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string fullName)
        {
            try
            {
                // TODO: Implement actual email sending logic
                _logger.LogInformation($"Welcome email would be sent to: {email} for user: {fullName}");
                
                // Simulate email sending delay
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {email}");
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string email, string orderNumber, decimal totalAmount)
        {
            try
            {
                // TODO: Implement actual email sending logic
                _logger.LogInformation($"Order confirmation email would be sent to: {email}");
                _logger.LogInformation($"Order: {orderNumber}, Total: {totalAmount:C}");
                
                // Simulate email sending delay
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send order confirmation email to {email}");
                return false;
            }
        }
    }
}
