using UnityEngine;
using System;
namespace APP.Licence
{
    public static class Util
    {
        public static DateTime String2Time(string time)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(time, $"yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                Debug.Log($" String2Time . {dateTime} ");
            }
            return dateTime;
        }

        #region 数据持久化
        /// <summary>
        /// 取得数据
        /// </summary>
        public static string GetString(string key)
        {
            string name = Application.identifier;
            return PlayerPrefs.GetString(name);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SetString(string key, string value)
        {
            string name = Application.identifier;
            PlayerPrefs.DeleteKey(name);
            PlayerPrefs.SetString(name, value);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        public static void DeleteString(string key)
        {
            string name = Application.identifier;
            PlayerPrefs.DeleteKey(name);
        }
        /// <summary>
        /// 删除所有本地数据
        /// </summary>
        public static void DeleteAllString()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion
    }
}