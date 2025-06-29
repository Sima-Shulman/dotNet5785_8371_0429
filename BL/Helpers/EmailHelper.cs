using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Helpers;

public static class EmailHelper
{
    public static void SendEmail(string to, string subject, string body)
    {
        using var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("chaverimorganization@gmail.com", "pfcl duwm srwy uteb"),
            EnableSsl = true,
        };

        var mail = new MailMessage("chaverimorganization@gmail.com", to, subject, body);
        smtp.Send(mail);
    }

    public static Task SendEmailAsync(string to, string subject, string body)
    {
        return Task.Run(() => SendEmail(to, subject, body));
    }
}
