using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Utilities;

namespace MvcForum.Core
{
    public class FeatureManager :IFeatureManager
    {

        public bool IsEnabled(string feature)
        {
            bool.TryParse(ConfigurationManager.AppSettings[feature], out var enabled);
            return enabled;
        }
        
    }
}
