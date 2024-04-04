using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class LogTypes
    {
        public LogTypes(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static LogTypes AUTH_TOKEN { get { return new LogTypes("AUTH_TOKEN"); } }
        public static LogTypes USER_SESSION { get { return new LogTypes("USER_SESSION"); } }
        public static LogTypes COMPANY_SETTINGS { get { return new LogTypes("COMPANY_SETTINGS"); } }
        public static LogTypes USER_COMPANY_INSTANCE { get { return new LogTypes("USER_COMPANY_INSTANCE"); } }
        public static LogTypes COMPANY_CONFIGURATION { get { return new LogTypes("COMPANY_CONFIGURATION"); } }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
