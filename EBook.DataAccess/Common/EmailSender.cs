using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EBook.DataAccess.Common
{
    public class EmailSender : IEmailSender
    {
       
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
          return Task.CompletedTask;
        }
    }
}
