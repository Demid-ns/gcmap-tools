using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Services
{
    public class EmailService
    {
        //Метод для отправки е-mail на подтверждение аккаунта
        public async Task SendEmailAsync(string email, string subject, string message, IConfiguration configuration)
        {
            //Считываем SMTP-конфигурацию
            string senderEmail = configuration["SMTP:Email"];
            string server = configuration["SMTP:Server"];
            int port = int.Parse(configuration["SMTP:Port"]);
            string password = configuration["SMTP:Password"];

            //Создаем е-mail письмо
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация", email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            //Отправляем письмо с smtp-клиента указанной конфигурации
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(server, port);

                await client.AuthenticateAsync(senderEmail, password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
