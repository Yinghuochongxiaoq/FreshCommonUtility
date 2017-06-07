using System;
using FreshCommonUtility.DataConvert;

namespace FreshCommonUtility.Security
{
    /// <summary>
    /// 身份证验证帮助类
    /// </summary>
    public static class IdCardValidatorHelper
    {
        //位权值数组
        private static readonly byte[] Weight = new byte[17];
        //身份证行政区划代码部分长度
        private static byte fPart = 6;
        //算法求模关键参数
        private static byte fMode = 11;
        //旧身份证长度
        private static byte oIdLen = 15;
        //新身份证长度
        private static byte nIdLen = 18;
        //新身份证年份标记值
        private static string yearFlag = "19";
        //校验字符串
        private static string checkCode = "10X98765432";

        /// <summary>
        /// 验证身份证合理性
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        public static bool CheckIdCard(string idNumber)
        {
            if (idNumber.Length == nIdLen)
            {
                bool check = CheckIdCard18(idNumber);
                return check;
            }
            else if (idNumber.Length == oIdLen)
            {
                bool check = CheckIdCard15(idNumber);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 18位身份证号码验证
        /// </summary>
        private static bool CheckIdCard18(string idNumber)
        {
            long n;
            if (long.TryParse(idNumber.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time;
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());
            }
            int y = sum%11;
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 16位身份证号码验证
        /// </summary>
        private static bool CheckIdCard15(string idNumber)
        {
            long n;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time;
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;
        }

        /// <summary>
        /// 获取新身份证最后一位校验位
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        public static string GetIdCardCheckCode(string idCard)
        {
            int sum = 0;
            //进行加权求和计算
            for (int i = 0; i < Weight.Length; i++)
            {
                sum += int.Parse(idCard.Substring(i, 1)) * Weight[i];
            }
            //求模运算得到模值
            byte mode = (byte)(sum % fMode);
            return checkCode.Substring(mode, 1);
        }

        /// <summary>
        /// 检查身份证长度是否合法
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        public static bool CheckIdCardLen(string idCard)
        {
            if (idCard.Length == oIdLen || idCard.Length == nIdLen)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证是否是新身份证
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        public static bool IsNewIdCard(string idCard)
        {
            if (idCard.Length == nIdLen)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取身份证生日时间
        /// </summary>
        /// <param name="idCardNo">身份证号码</param>
        /// <returns>生日字符串</returns>
        public static string GetBirthday(string idCardNo)
        {
            string str;
            if (IsNewIdCard(idCardNo))
            {
                str = idCardNo.Substring(fPart, 8);
            }
            else
            {
                str = yearFlag + idCardNo.Substring(fPart, 6);
            }
            return str;
        }

        /// <summary>
        /// 根据身份证号获得性别
        /// </summary>
        /// <param name="idNumber">身份证号</param>
        /// <param name="borthday">生日</param>
        /// <param name="mastCheck">是否需要身份证号码检测，默认不检测</param>
        /// <returns>1：男；0：女；-1：身份证解析错误</returns>
        public static int GetSexByIdCard(string idNumber, out DateTime borthday, bool mastCheck = false)
        {
            //性别
            int sex = -1;
            //年
            int year = 1900;
            //月
            int month = 1;
            //日
            int day = 1;
            borthday = new DateTime(1900, 1, 1);
            if (mastCheck)
            {
                if (!CheckIdCard(idNumber))
                {
                    return -1;
                }
            }
            //解析获得性别
            if (idNumber.Length == 15)
            {
                sex = DataTypeConvertHelper.ToInt(idNumber.Substring(14, 1), 0) % 2;
                year = DataTypeConvertHelper.ToInt(idNumber.Substring(6, 2), 0) + 19 * 100;
                month = DataTypeConvertHelper.ToInt(idNumber.Substring(8, 2), 1);
                day = DataTypeConvertHelper.ToInt(idNumber.Substring(10, 2), 1);
                borthday = new DateTime(year, month, day);
            }
            else if (idNumber.Length == 18)
            {
                sex = DataTypeConvertHelper.ToInt(idNumber.Substring(16, 1), 0) % 2;
                borthday = new DateTime(year, month, day);
            }
            return sex;
        }
    }
}
