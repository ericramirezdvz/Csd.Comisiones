using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Common.Models
{
    public class EmailMessage
    {
        public List<string> To { get; set; } = new();

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; } = true;
    }
}
