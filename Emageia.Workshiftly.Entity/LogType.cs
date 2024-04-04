using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public enum LogType
    {
        
        [Description("AUTH_TOKEN")]
        AUTH_TOKEN ,

        [Description("USER_SESSION")]
        USER_SESSION,

        [Description("COMPANY_SETTINGS")]
        COMPANY_SETTINGS,

        [Description("USER_COMPANY_INSTANCE")]
        USER_COMPANY_INSTANCE,

        [Description("COMPANY_CONFIGURATION")]
        COMPANY_CONFIGURATION, 
        
        [Description("DEVICE_SETTINGS")]
        DEVICE_SETTINGS,

        [Description("LOGGED_IN_DEVICE_SETTINGS")]
        LOGGED_IN_DEVICE_SETTINGS,



    }

    


    
}
