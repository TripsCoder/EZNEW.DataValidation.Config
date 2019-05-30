using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EZNEW.DataValidation.Config
{
    /// <summary>
    /// Type Rule Collection
    /// </summary>
    public class RuleCollection
    {
        /// <summary>
        /// Rules
        /// </summary>
        [JsonProperty(PropertyName ="rules")]
        public List<TypeRule> Rules
        {
            get;set;
        }
    }
}
