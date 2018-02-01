using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Models
{
    public class EmailInput
    {
        [JsonProperty(PropertyName = "nomeDestinatario")]
        public string nomeDestinatario { get; set; }

        [JsonProperty(PropertyName = "emailDestinatario")]
        public string emailDestinatario { get; set; }

        [JsonProperty(PropertyName = "assunto")]
        public string assunto { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }
    }

    public class EmailInputCandidato : EmailInput
    {
        [JsonProperty(PropertyName = "nomeHeadhunter")]
        public string nomeHeadhunter { get; set; }

        [JsonProperty(PropertyName = "tituloVaga")]
        public string tituloVaga { get; set; }

    }

    public class EmailInputHeadhunter : EmailInput
    {
        [JsonProperty(PropertyName = "nomeCandidato")]
        public string nomeCandidato { get; set; }

        [JsonProperty(PropertyName = "emailCandidato")]
        public string emailCandidato { get; set; }

        [JsonProperty(PropertyName = "tituloVaga")]
        public string tituloVaga { get; set; }

    }
}
