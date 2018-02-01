using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vox.Platform.Abstraction.API
{
    public class WriteResult
    {
        public bool result { get; }
        public string businessMessage { get; } = "";
        public object content { get; }

        public WriteResult(bool result, string businessMessage)
        {
            this.result = result;
            this.businessMessage = businessMessage;
        }

        public WriteResult(bool result, string businessMessage, JObject content)
        {
            this.result = result;
            this.businessMessage = businessMessage;
            this.content = content;
        }
        public WriteResult(bool result, string businessMessage, JArray content)
        {
            this.result = result;
            this.businessMessage = businessMessage;
            this.content = content;
        }
        public WriteResult(bool result, JObject content)
        {
            this.result = result;
            this.content = content;
        }
        public WriteResult(bool result, JArray content)
        {
            this.result = result;
            this.content = content;
        }

    }
}
