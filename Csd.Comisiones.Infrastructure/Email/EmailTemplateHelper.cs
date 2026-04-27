using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Infrastructure.Email
{
    public static class EmailTemplateHelper
    {
        public static string LoadTemplate(string templateName)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Templates",
                templateName);

            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static string Replace(string template, Dictionary<string, string> values)
        {
            foreach (var item in values)
            {
                template = template.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return template;
        }
    }
}
