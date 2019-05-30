using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.DataValidation.Config
{
    /// <summary>
    /// Type Validate Rule
    /// </summary>
    public class TypeRule
    {
        /// <summary>
        /// Type Full Name
        /// </summary>
        [JsonProperty(PropertyName = "typeName")]
        public string TypeFullName
        {
            get;set;
        }

        /// <summary>
        /// Rules
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public List<PropertyRule> Rules
        {
            get;set;
        }
    }
}
