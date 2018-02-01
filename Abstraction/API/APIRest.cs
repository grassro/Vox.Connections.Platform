using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Vox.Platform.Abstraction.API;

namespace Vox.Abstraction.API
{
    public class APIRest
    {
        #region Definição da classe

        string _ENDPOINT = "";
        string _TOKEN = "";
        string _APINAME = "";
        string _CONFIG = "/Config/configAPI";
        bool _NAMED = false;

        IConfigurationRoot Configuration { get; set; }

        public APIRest(string apiName)
        {

            _APINAME = apiName;


            //recupera do config o nome do endpoint e o token
            string jsonGeneralConfig = "";
            string path = "";
            if (File.Exists($"{Directory.GetCurrentDirectory()}{_CONFIG}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"))
            {
                path = $"{Directory.GetCurrentDirectory()}{_CONFIG}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            }
            else if (File.Exists($"{Directory.GetCurrentDirectory()}{_CONFIG}.json"))
            {
                path = $"{Directory.GetCurrentDirectory()}{_CONFIG}.json";
            }
            if (path != "")
            {
                //Resgate das configuracoes iniciais
                jsonGeneralConfig = File.ReadAllText(path);
                List<JObject> initialData = JsonConvert.DeserializeObject<List<JObject>>(jsonGeneralConfig);
                //varre o JArray em busca do nome da api injetada
                foreach (JObject record in initialData)
                {
                    if (record["name"].Value<string>() == apiName)
                    {
                        _TOKEN = record["token"].Value<string>();

                        _NAMED = record["named"].Value<bool>();

                        if (!_NAMED)
                            _ENDPOINT = record["host"].Value<string>();
                        else
                            _ENDPOINT = record["host"].Value<string>() + record["name"];
                    }
                }
            }
        }

        #endregion

        #region Métodos públicos

        #region Assíncronos

        public async Task<ReadResult> GetAsync(string serviceRoute)
        {
            return await Task.Run(() => this.Get(serviceRoute));
        }

        public async Task<WriteResult> PutAsync(string serviceRoute, [Optional] JArray body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Put(serviceRoute, body, headers));
        }

        public async Task<WriteResult> PutAsync(string serviceRoute, [Optional] JObject body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Put(serviceRoute, new JArray(body), headers));
        }

        public async Task<WriteResult> PostAsync(string serviceRoute, [Optional] JArray body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Post(serviceRoute, body, headers));
        }

        public async Task<WriteResult> PostAsync(string serviceRoute, [Optional] JObject body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Post(serviceRoute, body, headers));
        }

        public async Task<WriteResult> DeleteAsync(string serviceRoute, [Optional] JArray body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Delete(serviceRoute, headers));
        }

        public async Task<WriteResult> DeleteAsync(string serviceRoute, [Optional] JObject body, [Optional]Dictionary<string, string> headers)
        {
            return await Task.Run(() => this.Delete(serviceRoute, headers));
        }

        #endregion 

        #region Síncronos

        /// <summary>
        /// Faz o GET admitindo que o resultado é um JSON ou Jsonarray
        /// </summary>
        /// <param name="serviceRoute">Rota com os parametros para o servico</param>
        /// <returns>Retorna o tipo de dado declarado pelo cliente</returns>
        public ReadResult Get(string serviceRoute, [Optional] Dictionary<string, string> header)
        {
            serviceRoute = serviceRoute.Replace(Environment.NewLine, string.Empty)
                                        .Replace("\"", "'");

            //Chama o serviço de acordo com a rota informada  
            HttpWebRequest request = null;//(HttpWebRequest)WebRequest.Create(_SOURCE);
            HttpWebResponse response = null;

            object result = new object();
            ReadResult readResult;

            //Create Request
            try
            {
                request = (HttpWebRequest)WebRequest.Create(_ENDPOINT + serviceRoute);
                request.Method = "GET";
                request.ContentType = "application/json; encoding='utf-8'";

                if (header != null)
                    foreach (var item in header)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }

                //Get Response
                response = (HttpWebResponse)request.GetResponse();

                string content = response.GetResponseStream().ToString();

                string responseText;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseText = reader.ReadToEnd();
                }

                //Now load the JSON Document
                try
                {
                    JObject aux = JsonConvert.DeserializeObject<JObject>(responseText);
                    readResult = new ReadResult(aux["result"].Value<bool>(), aux["businessMessage"].Value<string>(), aux["content"].Value<JArray>());
                    return readResult;
                }
                catch
                {
                    JObject aux = JsonConvert.DeserializeObject<JObject>(responseText);
                    return readResult = new ReadResult(true, aux["businessMessage"].Value<string>(), aux["content"].Value<JObject>());
                }

            }
            //Verifica erros, loga-os e retorna uma mensagem para o chamador
            catch (Exception ex)
            {
                //TODO: logar no sistema fleury
                throw new Exception(ex.Message);
            }

            //Caso bem sucedido, retorna o dado


        }

        public WriteResult Post(string serviceRoute, [Optional] JArray body, [Optional]Dictionary<string, string> header)
        {
            return Send("POST", serviceRoute, body, header);
        }

        public WriteResult Post(string serviceRoute, [Optional] JObject body, [Optional]Dictionary<string, string> header)
        {
            return Send("POST", serviceRoute, body, header);
        }

        public WriteResult Put(string serviceRoute, [Optional] JArray body, [Optional]Dictionary<string, string> header)
        {
            return Send("PUT", serviceRoute, body, header);
        }

        public object WriteResult(string serviceRoute, [Optional] JObject body, [Optional]Dictionary<string, string> header)
        {
            return Send("PUT", serviceRoute, body, header);
        }

        public WriteResult Delete(string serviceRoute, [Optional]Dictionary<string, string> headers)
        {
            serviceRoute = serviceRoute.Replace(Environment.NewLine, string.Empty)
                                        .Replace("\"", "'");
          
            //Chama o serviço de acordo com a rota informada  
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(_ENDPOINT + serviceRoute);
                request.Method = "DELETE";
                request.ContentType = "application/json";

                //Get Response
                response = (HttpWebResponse)request.GetResponse();
                string responseText;
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseText = reader.ReadToEnd();
                }

                return new WriteResult(true, responseText);
            }
            catch (Exception ex)
            {
                //TODO Logar log corporativo
                Console.WriteLine(ex.Message);
                return new WriteResult(true, "");
            }
        }

        #endregion

        #endregion

        #region Métodos privados

        /// <summary>
        /// Faz um operação de POST ou PUT
        /// </summary>
        private WriteResult Send(string operation, string serviceRoute, [Optional] Object body, Dictionary<string, string> headers)
        {

            serviceRoute = serviceRoute.Replace(Environment.NewLine, string.Empty)
                                        .Replace("\"", "'");
           
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    if (body == null)
                        body = "";

                    client.Headers["Content-Type"] = "application/json";
                    byte[] responsebytes = client.UploadData(_ENDPOINT + serviceRoute, operation, Encoding.Default.GetBytes(body.ToString()));

                    return new WriteResult(true, Encoding.UTF8.GetString(responsebytes));
                }
            }
            catch (Exception ex)
            {
                //TODO Logar log corporativo
                Console.WriteLine(ex.Message);
                return new WriteResult(false, new JArray("ex:" + ex.Message));
            }
        }

        public bool CheckBodyIntegrity(JObject request, JObject requestCheck, Dictionary<string, string> headers)
        {
            bool isTheRequisition = true;

            //Verifica se existe body esperado no projeto da requisição
            if (requestCheck != null)
            {
                bool bodyOk = CompareJsonIntegrity(request["body"].Value<JObject>(), JObject.FromObject(requestCheck));

                if (!bodyOk)
                {
                    isTheRequisition = false;
                }
            }

            return isTheRequisition;
        }

        private bool CompareHeaderIntegrity(Dictionary<string, string> requisitionHeaders, JObject checkHeaders)
        {
            bool headerOk = true;
            foreach (JProperty header in checkHeaders.Properties())
            {
                if (!requisitionHeaders.ContainsKey(header.Name))
                    headerOk = false;
            }
            return headerOk;
        }

        private bool CompareJsonIntegrity(JObject requisitionBody, JObject bodyToCheck)
        {
            bool bodyOk = true;
            if (requisitionBody != null)
            {

                foreach (var check in bodyToCheck)
                {
                    string result = "";
                    if (requisitionBody[check.Key] != null)
                    {
                        if (result == null && result != "")
                        {
                            bodyOk = false;
                        }
                    }
                }
            }
            return bodyOk;
        }

        #endregion
    }
}
