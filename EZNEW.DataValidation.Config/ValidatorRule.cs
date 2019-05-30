using EZNEW.DataValidation;
using EZNEW.Develop.DataValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.DataValidation.Config
{
    /// <summary>
    /// Validator Rule
    /// </summary>
    public class ValidatorRule
    {
        #region Propertys

        /// <summary>
        /// Validate Type
        /// </summary>
        [JsonProperty(PropertyName = "vtype")]
        public ValidatorType ValidateType
        {
            get; set;
        }

        /// <summary>
        /// Operator
        /// </summary>
        [JsonProperty(PropertyName = "operator")]
        public CompareOperator Operator
        {
            get; set;
        }

        /// <summary>
        /// Value
        /// </summary>
        [JsonProperty(PropertyName = "val")]
        public dynamic Value
        {
            get; set;
        }

        /// <summary>
        /// Enum Type
        /// </summary>
        [JsonProperty(PropertyName = "enumType")]
        public string EnumType
        {
            get; set;
        }

        /// <summary>
        /// Max Value
        /// </summary>
        [JsonProperty(PropertyName = "maxValue")]
        public dynamic MaxValue
        {
            get; set;
        }

        /// <summary>
        /// Min Value
        /// </summary>
        [JsonProperty(PropertyName = "minValue")]
        public dynamic MinValue
        {
            get; set;
        }

        /// <summary>
        /// Lower Boundary
        /// </summary>
        [JsonProperty(PropertyName = "lowerBoundary")]
        public RangeBoundary LowerBoundary
        {
            get; set;
        }

        /// <summary>
        /// Upper Boundary
        /// </summary>
        [JsonProperty(PropertyName = "upperBoundary")]
        public RangeBoundary UpperBoundary
        {
            get; set;
        }

        /// <summary>
        /// Error Message
        /// </summary>
        [JsonProperty(PropertyName = "errorMsg")]
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Tip Message
        /// </summary>
        [JsonProperty(PropertyName = "tipMsg")]
        public bool TipMessage
        {
            get; set;
        }

        /// <summary>
        /// Compare Target
        /// </summary>
        [JsonProperty(PropertyName = "compareType")]
        public CompareObject CompareTarget
        {
            get; set;
        }

        #endregion
    }

    /// <summary>
    /// Compare Object
    /// </summary>
    public enum CompareObject
    {
        Field,
        Value
    }
}
