using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Infrastructure.Email
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }

        public string SenderName { get; set; }
        public string SenderEmail { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string BaseUrl { get; set; }
    }
}
