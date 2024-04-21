﻿using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace Persistence.Repositories
{
    internal sealed class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;

        public EmailRepository(IConfiguration config) => _config = config;


        public async Task SendAsync(string[] to, string subject, string body, CancellationToken cancellation = default)
        {
            var settings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();
            var emails = to.Select(x => new MailboxAddress(x, x));
            
            message.From.Add(new MailboxAddress(settings["SenderName"], settings["DefaultFromEmail"]));
            message.To.AddRange(emails);
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            var port = int.Parse(settings["Port"]);
            client.Connect(settings["Host"], port, false, cancellation);
            var result = await client.SendAsync(message, cancellation);
            client.Disconnect(true, cancellation);
        }
    }
}
