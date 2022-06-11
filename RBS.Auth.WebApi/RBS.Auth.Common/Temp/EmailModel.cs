
using System.Collections.Generic;

namespace RBS.Auth.Common.Temp
{
    public class EmailModel
    {
        public EmailModel()
        {
            ToAddresses = new List<string>();
            FromAddresses = new List<string>();
            CcAddresses = new List<string>();
        }

        public List<string> ToAddresses { get; set; }
        public List<string> FromAddresses { get; set; }
        public List<string> CcAddresses { get; set; }

        public byte[] Attachment { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public bool IsHtml { get; set; }
    }
}
