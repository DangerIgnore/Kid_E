using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Web;

namespace com.baidu.ai
{
    public static class AccessToken
    {
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 返回token示例
        public static String TOKEN = "";

        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "m2H9NHUKSuVTzk4PgzYYd8aC";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "dWSugjsSzlk7SFNVedA94TPlYcPrjMcX";

        public static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            return result;
        }
        public static String GetJosn(string json, string key)
        {
            JObject obj = JObject.Parse(json);
            foreach (var x in obj)
            {
                if (x.Key.Equals(key))
                {
                    return x.Value.ToString();
                }
            }
            return "";
        }
    }

    public static class DiscernPic
    {
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "m2H9NHUKSuVTzk4PgzYYd8aC";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "dWSugjsSzlk7SFNVedA94TPlYcPrjMcX";
        public static JObject discernNumber(byte[] Pic)
        {
            var client = new Baidu.Aip.Ocr.Ocr(clientId, clientSecret);

            // 通用文字识别
            JObject result = client.Numbers(Pic);
            return result;
        }

        public static JObject handWriteNumber(byte[] Pic)
        {
            var client = new Baidu.Aip.Ocr.Ocr(clientId, clientSecret);

            // 通用文字识别
            JObject result = client.Handwriting(Pic);
            return result;
        }
    }
}
