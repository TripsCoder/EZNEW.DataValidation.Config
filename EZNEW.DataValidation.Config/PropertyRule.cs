using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.DataValidation.Config
{
    /// <summary>
    /// Property Validate Rule
    /// </summary>
    public class PropertyRule
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// Rules
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public List<ValidatorRule> Rules
        {
            get;set;
        }
    }
}
