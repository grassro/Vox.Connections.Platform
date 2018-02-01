using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vox.Abstraction.API
{
    public class ReadResult
    {

        public bool result { get; }
        public string businessMessage { get; } = "";
        public object content { get; }

        public ReadResult(bool result, string businessMessage)
        {
            this.result = result;
            this.businessMessage = businessMessage;
        }

        public ReadResult(bool result, string businessMessage, JObject content)
        {
            this.result = result;
            this.businessMessage = businessMessage;
            this.content = content;
        }
        public ReadResult(bool result, string businessMessage, JArray content)
        {
            this.result = result;
            this.businessMessage = businessMessage;
            this.content = content;
        }
        public ReadResult(bool result, JObject content)
        {
            this.result = result;
            this.content = content;
        }
        public ReadResult(bool result, JArray content)
        {
            this.result = result;
            this.content = content;
        }
    }
}
