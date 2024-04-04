


using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Environment.Registry;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.MajorVersion10
{
    public class RegistryProviderDefault : IRegistry
    {
        public RegistryProviderDefault()
        {
            // NOP
        }
        public object GetValue(string keyName, string valueName, object defaultValue)
        {
            return Microsoft.Win32.Registry.GetValue(keyName, valueName, defaultValue);
           
        }       
    }
}
