using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RestClient2
{
    internal class Program
    {
        private static string AntrianUrl = "https://api-api-apicast-production.apps.ocp4dvlp.bpjs-kesehatan.go.id/bestamo_dev/mobile/v1/sso/pegawai/logout/12345678";
        private static string VclaimUrl = "https://api-api-apicast-production.apps.ocp4dvlp.bpjs-kesehatan.go.id/vclaim-rest-dev";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            /* OUTPUT FILE SETUP */
            string path = @"output.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }

            string outDate, outAccount, outApiType, outUserKey, outResult, outMessage;

            foreach (string row in System.IO.File.ReadLines(@"input.txt"))
            {
                outDate = string.Empty;
                outAccount = string.Empty;
                outApiType = string.Empty;
                outUserKey = string.Empty;
                outResult = string.Empty;
                outMessage = string.Empty;

                string[] rowContent = row.Split('|');

                if ((rowContent != null) && (rowContent.Length == 4) &&
                    (rowContent[1] != string.Empty) && (rowContent[2] != string.Empty) && (rowContent[3] != string.Empty))
                {
                    outDate = DateTime.Now.ToString();
                    outAccount = rowContent[1];
                    outApiType = rowContent[2];
                    outUserKey = rowContent[3];

                    Task<string> response = ConsumeApi(outApiType, outUserKey);
                    outResult = response.Result.ToString();

                    outMessage = outDate + "|" + outAccount + "|" + outApiType + "|" + outUserKey + "|" + outResult;
                    Console.WriteLine(outMessage);
                    log.Info(outMessage);

                    /* WRITE TO OUTPUT FILE */
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(outMessage);
                    }
                }
                else
                {
                    /* WRITE TO OUTPUT FILE WITHOUT PROCESS */
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(row);
                    }
                }
            }

            Console.WriteLine("FINISH!");
            log.Info("FINISH!");
            Console.ReadLine();
        }

        private static async Task<String> ConsumeApi(string apiType, string userKey)
        {
            HttpClient _client = new HttpClient();

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.TryAddWithoutValidation("user_key", userKey);

            try
            {
                string apiUri = apiType.Equals("ANTRIAN") ? AntrianUrl : VclaimUrl;
                HttpResponseMessage response = await _client.GetAsync(apiUri);

                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode.ToString();
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
