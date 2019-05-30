using EZNEW.Develop.DataValidation.Validators;
using EZNEW.Framework.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using System.IO;
using EZNEW.Develop.DataValidation;

namespace EZNEW.DataValidation.Config
{
    /// <summary>
    /// Validation Config
    /// </summary>
    public static class ValidationConfig
    {
        #region fields

        static Type baseExpressType = typeof(Expression);//Expression Type
        static MethodInfo lambdaMethod = null;//Create Method Object
        static List<MethodInfo> validationMethods = new List<MethodInfo>();

        #endregion

        static ValidationConfig()
        {
            var baseExpressMethods = baseExpressType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            lambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            validationMethods = typeof(ValidationManager).GetMethods().ToList();
        }

        #region Validation Config

        /// <summary>
        /// Init From Config File
        /// </summary>
        /// <param name="filePaths">config files</param>
        public static void InitFromConfigFile(params string[] filePaths)
        {
            if (filePaths == null)
            {
                return;
            }
            foreach (var file in filePaths)
            {
                try
                {
                    string jsonData = File.ReadAllText(file);
                    InitFromJson(jsonData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Init From Json
        /// </summary>
        /// <param name="json"></param>
        public static void InitFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            RuleCollection ruleCollection = JsonSerialize.JsonToObject<RuleCollection>(json);
            if (ruleCollection == null || ruleCollection.Rules == null)
            {
                return;
            }
            foreach (var typeRule in ruleCollection.Rules)
            {
                if (typeRule == null || string.IsNullOrWhiteSpace(typeRule.TypeFullName) || typeRule.Rules == null)
                {
                    continue;
                }
                Type modelType = Type.GetType(typeRule.TypeFullName);
                if (modelType == null)
                {
                    continue;
                }
                //load propertys and fields
                List<MemberInfo> memberInfoList = new List<MemberInfo>();
                memberInfoList.AddRange(modelType.GetFields(BindingFlags.Public | BindingFlags.Instance));
                memberInfoList.AddRange(modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                ParameterExpression parExp = Expression.Parameter(modelType);//parameter expression
                Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(parExp, 0);
                Type valFieldType = typeof(ValidationField<>).MakeGenericType(modelType);
                foreach (var propertyRule in typeRule.Rules)
                {
                    if (propertyRule == null || propertyRule.Rules == null)
                    {
                        return;
                    }
                    string[] propertyNameArray = propertyRule.Name.LSplit(".");
                    Expression propertyExpress = null;
                    foreach (string pname in propertyNameArray)
                    {
                        if (propertyExpress == null)
                        {
                            propertyExpress = Expression.PropertyOrField(parExp, pname);
                        }
                        else
                        {
                            propertyExpress = Expression.PropertyOrField(propertyExpress, pname);
                        }
                    }
                    Type funcType = typeof(Func<,>).MakeGenericType(modelType, typeof(object));//function type
                    var genericLambdaMethod = lambdaMethod.MakeGenericMethod(funcType);
                    var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        Expression.Convert(propertyExpress,typeof(object)),parameterArray
                    });

                    if (lambdaExpression == null)
                    {
                        continue;
                    }
                    foreach (var rule in propertyRule.Rules)
                    {
                        var fieldInstance = Activator.CreateInstance(valFieldType);
                        valFieldType.GetProperty("FieldExpression").SetValue(fieldInstance, lambdaExpression);
                        valFieldType.GetProperty("ErrorMessage").SetValue(fieldInstance, rule.ErrorMessage);
                        valFieldType.GetProperty("TipMessage").SetValue(fieldInstance, rule.TipMessage);
                        Array valFieldArray = Array.CreateInstance(valFieldType, 1);
                        valFieldArray.SetValue(fieldInstance, 0);
                        switch (rule.ValidateType)
                        {
                            case ValidatorType.Email:
                                MethodInfo emailMethod = validationMethods.FirstOrDefault(c => c.Name == "Email");
                                if (emailMethod == null)
                                {
                                    continue;
                                }
                                emailMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    valFieldArray
                                });
                                break;
                            case ValidatorType.CreditCard:
                                MethodInfo creditMethod = validationMethods.FirstOrDefault(c => c.Name == "CreditCard");
                                if (creditMethod == null)
                                {
                                    continue;
                                }
                                creditMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    valFieldArray
                                });
                                break;
                            case ValidatorType.EnumType:
                                MethodInfo enumMethod = validationMethods.FirstOrDefault(c => c.Name == "EnumType");
                                if (enumMethod == null)
                                {
                                    continue;
                                }
                                Type enumType = Type.GetType(rule.EnumType);
                                if (enumType == null)
                                {
                                    continue;
                                }
                                enumMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    enumType,valFieldArray
                                });
                                break;
                            case ValidatorType.MaxLength:
                                MethodInfo maxLengthMethod = validationMethods.FirstOrDefault(c => c.Name == "MaxLength");
                                if (maxLengthMethod == null)
                                {
                                    continue;
                                }
                                maxLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    Convert.ToInt32(rule.MaxValue),valFieldArray
                                });
                                break;
                            case ValidatorType.MinLength:
                                MethodInfo minLengthMethod = validationMethods.FirstOrDefault(c => c.Name == "MinLength");
                                if (minLengthMethod == null)
                                {
                                    continue;
                                }
                                minLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    Convert.ToInt32(rule.MinValue),valFieldArray
                                });
                                break;
                            case ValidatorType.Phone:
                                MethodInfo phoneMethod = validationMethods.FirstOrDefault(c => c.Name == "Phone");
                                if (phoneMethod == null)
                                {
                                    continue;
                                }
                                phoneMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    valFieldArray
                                });
                                break;
                            case ValidatorType.Range:
                                MethodInfo rangeMethod = validationMethods.FirstOrDefault(c => c.Name == "Range");
                                if (rangeMethod == null)
                                {
                                    continue;
                                }
                                rangeMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    rule.MinValue,rule.MaxValue,rule.LowerBoundary,rule.UpperBoundary,valFieldArray
                                });
                                break;
                            case ValidatorType.RegularExpression:
                                MethodInfo regularExpressionMethod = validationMethods.FirstOrDefault(c => c.Name == "RegularExpression");
                                if (regularExpressionMethod == null)
                                {
                                    continue;
                                }
                                regularExpressionMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    rule.Value,valFieldArray
                                });
                                break;
                            case ValidatorType.Required:
                                MethodInfo requiredMethod = validationMethods.FirstOrDefault(c => c.Name == "Required");
                                if (requiredMethod == null)
                                {
                                    continue;
                                }
                                requiredMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    valFieldArray
                                });
                                break;
                            case ValidatorType.Url:
                                MethodInfo urlMethod = validationMethods.FirstOrDefault(c => c.Name == "Url");
                                if (urlMethod == null)
                                {
                                    continue;
                                }
                                urlMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    valFieldArray
                                });
                                break;
                            case ValidatorType.StringLength:
                                MethodInfo strLengthMethod = validationMethods.FirstOrDefault(c => c.Name == "StringLength");
                                if (strLengthMethod == null)
                                {
                                    continue;
                                }
                                strLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    Convert.ToInt32(rule.MaxValue),Convert.ToInt32(rule.MinValue),valFieldArray
                                });
                                break;
                            case ValidatorType.Compare:
                                MethodInfo compareMethod = validationMethods.FirstOrDefault(c => c.Name == "SetCompareValidation");
                                if (compareMethod == null)
                                {
                                    continue;
                                }
                                object compareValue = rule.Value;
                                switch (rule.CompareTarget)
                                {
                                    case CompareObject.Field:
                                        string[] comparePropertyNameArray = compareValue.ToString().LSplit(".");
                                        Expression comparePropertyExpress = null;
                                        foreach (string pname in comparePropertyNameArray)
                                        {
                                            if (comparePropertyExpress == null)
                                            {
                                                comparePropertyExpress = Expression.PropertyOrField(parExp, pname);
                                            }
                                            else
                                            {
                                                comparePropertyExpress = Expression.PropertyOrField(comparePropertyExpress, pname);
                                            }
                                        }
                                        var compareLambdaExpression = lambdaMethod.MakeGenericMethod(funcType).Invoke(null, new object[]
                                        {
                                            Expression.Convert(comparePropertyExpress,typeof(object)),parameterArray
                                        });
                                        if (compareLambdaExpression == null)
                                        {
                                            continue;
                                        }
                                        compareValue = compareLambdaExpression;
                                        break;
                                    default:
                                        if (rule.Operator == CompareOperator.In || rule.Operator == CompareOperator.NotIn)
                                        {
                                            IEnumerable<dynamic> valueArray = compareValue.ToString().LSplit(",");
                                            compareValue = valueArray;
                                        }
                                        break;
                                }
                                compareMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
                                {
                                    rule.Operator,compareValue,fieldInstance
                                });
                                break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
