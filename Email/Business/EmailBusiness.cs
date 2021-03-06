﻿using MailKit.Net.Smtp;
using MimeKit;
using Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Services.Business
{
    public class EmailBusiness
    {
        public Task<bool> EnviarEmail(SmtpSettings configuracoes, EmailInput parametrosEmail)
        {
            //Smtp Server 
            string SmtpServer = configuracoes.host;
            string SmtpUser = configuracoes.user;
            string SmtpPass = configuracoes.pass;
            int SmtpPortNumber = Convert.ToInt32(configuracoes.port);

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(SmtpUser, SmtpUser));
            mimeMessage.To.Add(new MailboxAddress(parametrosEmail.nomeDestinatario, parametrosEmail.emailDestinatario));
            mimeMessage.Subject = parametrosEmail.assunto;

            mimeMessage.Body = new TextPart("html")
            {
                Text = Template(parametrosEmail.nomeDestinatario, parametrosEmail.url)
            };

            using (var client = new SmtpClient())
            {

                client.Connect(SmtpServer, SmtpPortNumber, true);
                // Note: only needed if the SMTP server requires authentication 
                client.Authenticate(SmtpUser, SmtpPass);

                //Efetiva o envio do e-mail
                client.Send(mimeMessage);

                client.Disconnect(true);
            }

            return Task.FromResult(true);

        }

        private string Template(string nome, string url)
        {

            //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html  
            var pathToFile = "Email/template.html";

            var builder = new BodyBuilder();
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            string messageBody = string.Format(builder.HtmlBody, nome, url);

            return messageBody;

        }

    }
}
