using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityShell.Configuration
{
    public class IDeviceCodeRepository
    {
        internal object FindByUserCodeAsync(string userCode)
        {
            throw new NotImplementedException();
        }

        internal object FindByDeviceCodeAsync(string deviceCode)
        {
            throw new NotImplementedException();
        }

        internal object RemoveByDeviceCodeAsync(string deviceCode)
        {
            throw new NotImplementedException();
        }
    }
}
