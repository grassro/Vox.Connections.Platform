﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Services.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Services.Business;
using Microsoft.ApplicationInsights;

namespace Services.Controllers
{
    [Route("/email")]
    public class EmailController : Controller
    {
        #region Atributos
        private readonly IOptions<SmtpSettings> _smtpSettings;
        #endregion

        #region Construtor
        public EmailController(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Método responsável pelo envio de e-mails
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// 
        [HttpPost("/email")]
        [SwaggerOperation("EnviarEmail")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> EnviarEmail([FromBody]JObject email)
        {
            var sw = Stopwatch.StartNew();

            //Verifica se os parametros foram informados
            if (email == null)
            {
                return BadRequest();
            }

            try
            {
                EmailInput emailInput = JsonConvert.DeserializeObject<EmailInput>(Convert.ToString(email));

                bool response = await new EmailBusiness().EnviarEmail(_smtpSettings.Value, emailInput);

                //Verifica se houve retorno 
                if (response)
                {
                    sw.Stop();

                    return Ok(response);
                }
                else
                {
                    sw.Stop();

                    return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                //Instrumentar AppInsights
                sw.Stop();
                var telemetry = new TelemetryClient();

                var properties = new Dictionary<string, string>
                    {
                        { "Controller", "EmailController" },
                        { "Method", "EnviarEmail" },
                        { "Parameters", email.ToString() }
                    };

                var measurements = new Dictionary<string, double>
                    {
                        { "ExecutionTime", sw.ElapsedMilliseconds}
                    };

                telemetry.TrackException(ex, properties, measurements);

                //Retorna Internal Server Error
                return StatusCode(500);
            }
        }
        #endregion
    }
}