using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FreshCommonUtility.DataConvert;
using Microsoft.AspNetCore.Http;

namespace FreshCommonUtility.Web
{
    /// <summary>
    /// WebRequest extend
    /// </summary>
    public static class WebRequestHelper
    {
        /// <summary>
        /// 验证字符串是否为数字（正则表达式）（true = 是数字, false = 不是数字）
        /// </summary>
        /// <param name="validatedString">被验证的字符串</param>
        /// <returns>true = 是数字, false = 不是数字</returns>
        private static bool IsNumeric(string validatedString)
        {
            const string numericPattern = @"^[-]?\d+[.]?\d*$";
            return Regex.IsMatch(validatedString, numericPattern);
        }

        /// <summary>
        /// Get Querystring or Request.From params,you also can define params in method param use [FromBody]Type params string.
        /// </summary>
        /// <param name="context">request context</param>
        /// <param name="key">params key</param>
        /// <returns></returns>
        public static string GetStringFromParameters(this HttpContext context, string key)
        {
            var value = string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }
            if (context != null) value = context.Request?.Query[key];
            return value;
        }

        /// <summary>
        /// Get Querystring or Request.From params,you also can define params in method param use [FromBody]Type params int.
        /// </summary>
        /// <param name="context">request context</param>
        /// <param name="key">params key</param>
        /// <returns></returns>
        public static int GetIntFromParameters(this HttpContext context, string key)
        {
            var value = default(int);
            if (!string.IsNullOrEmpty(key))
            {
                value = (!string.IsNullOrEmpty(context.Request?.Query[key])
                         && IsNumeric(context.Request?.Query[key]))
                            ? int.Parse(context.Request?.Query[key])
                            : default(int);
            }
            return value;
        }

        /// <summary>
        /// Get Querystring params of DateTime type.
        /// </summary>
        /// <param name="context">request context</param>
        /// <param name="key">params key</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromParameters(this HttpContext context, string key)
        {
            var value = new DateTime(1900, 01, 01);
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }
            return !DateTime.TryParse(context.Request?.Query[key], out value) ? new DateTime(1900, 1, 1) : value;
        }

        /// <summary>
        /// Get collection of int.
        /// </summary>
        /// <param name="context">request context</param>
        /// <param name="key">params key</param>
        /// <param name="separator">split char</param>
        /// <returns></returns>
        public static List<int> GetListIntFromParameters(this HttpContext context,string key, char separator)
        {
            var strList = context.GetStringFromParameters(key);
            if (string.IsNullOrEmpty(strList))
            {
                return null;
            }
            var list = new List<int>();
            foreach (var item in strList.Split(separator))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                var id = DataTypeConvertHelper.ToInt(item);
                if (list.Contains(id))
                {
                    continue;
                }
                list.Add(id);
            }
            return list;
        }
    }
}
