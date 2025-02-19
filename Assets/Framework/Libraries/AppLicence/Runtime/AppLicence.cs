using System;
using UnityEngine;
namespace APP.Licence
{
    public class AppLicence : MonoBehaviour
    {
        [Header("证书有效期")]
        [SerializeField]
        private string validTime = DateTime.Now.ToString("yyyy-mm-ss");
        [Header("证书有效次数")]
        [SerializeField]
        private int maxUseCount = 1000;
        private Action<bool> action = null;

        // private void Awake()
        // {
        //     Initialize((bl) =>
        //     {
        //         Log($" 鉴权是否通过 {bl} ");
        //     });
        // }
        public void Initialize(Action<bool> action)
        {
            this.action = action;
            DateTime time = Util.String2Time(validTime);
            DateTime now = DateTime.Now;
            TimeSpan delta = time - now;

            if (Math.Abs(delta.TotalDays) > 365f)
            {
                int number = 0;

                string str = Util.GetString($"xyLicence");
                if (int.TryParse(str, out number))
                {
                    number++;
                }
                Util.SetString($"xyLicence", $"{number}");
                action?.Invoke(number < maxUseCount);
            }
            else
            {
                action?.Invoke(delta.TotalDays > 0);
            }
        }
    }
}