using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SmtpHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
