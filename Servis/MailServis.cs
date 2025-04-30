using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EmailVerificationService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _FromEmail;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;


    public EmailVerificationService(IConfiguration configuration)
    {  

        _smtpServer = configuration["EmailSettings:SmtpServer"];
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
        _FromEmail = configuration["EmailSettings:FromEmail"];
        _smtpPassword = configuration["EmailSettings:SmtpPassword"];
        _enableSsl = bool.TryParse(configuration["EmailSettings:EnableSsl"], out var ssl) && ssl;

    }

    public async Task<bool> SendVerificationEmail(string emailAddress, string verificationCode)
    {
        try
        {
            var fromAddress = new MailAddress(_FromEmail, "Verification Service");
            var toAddress = new MailAddress(emailAddress);
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Your Verification Code",
                Body = $"Your verification code is: {verificationCode}. It is valid for 90 seconds.",
            };

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_FromEmail, _smtpPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}
