using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace TestKeyRedis
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }


        private static async void PublishhMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // mã token truy cập
            //var ACCESS_TOKEN = "9Vmhl2svWoMIYmnPX0JC9TGCI30gurS9MEv58drDpvt";
            var ACCESS_TOKEN = "EcZUak6XwGUK3nupGYMWpDO1mvg1ryJQoDazmzhBhHe";

            using (var client = new HttpClient())
            {
                // tin nhắn sẽ được thông báo
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                 {
                     { "message", "Các cặp key dataPromotion có giá trị khác nhau\n" + message },
                 });

                // thêm mã token vào header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

                // Thực hiện gửi thông báo
                var result = await client.PostAsync("https://notify-api.line.me/api/notify", content);
            }
        }

        public static async void CheckDataKeyPromotion(string strStoreIds, string strStoreExpiredIds, string strCateIds = "8686,10298,10798,7147,7149,7148,7091,2488,7143,2515,8679,7160,3185")
        {
            // mã token truy cập
            var ACCESS_TOKEN = "TMi_ovpW516il-s32KrBLaXnjEAojL95GCGAqk4AZqgSpL08E89q2HS2_VfrrhcD-karPIQCnxj8SC9vjjfE0eGx9QiSMxt6DO1xGnQfZkH4fGhhFLUqI9Z--vRhyJcT7Cp8N6kRhUiesQdi7oCPe5iqML2EPTJFFipPreW9cAP5edd6W2aJXWqRx1PwuanEEk7OE_cz5weI-4prqdcwCHS4O1uQQjJ26q4NV8YylIYglY7etTCPTSoukV_E3Q62Yzuc7BDdLmZA7OcmMETGNw";

            List<string> result = new List<string>();
            List<Tuple<int, string>> listStoreIds = new List<Tuple<int, string>>();
            var tmp = strStoreIds.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var tmp1 = strStoreExpiredIds.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            foreach (var i in tmp)
            {
                listStoreIds.Add(new Tuple<int, string>(i, "NormalStore"));
            }
            foreach (var i in tmp1)
            {
                listStoreIds.Add(new Tuple<int, string>(i, "ExpiredStore"));
            }
            foreach (var store in listStoreIds)
            {
                var storeId = store.Item1;
                var storeType = store.Item2;
                using (var client = new HttpClient())
                {
                    // thêm mã token vào header
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

                    var url = "https://staging2.bachhoaxanh.com/webapi/api/test/TestListDataKeyRedisPromotion?storeId=" + storeId + "&strCateIds=" + strCateIds;
                    if (storeType == "ExpiredStore")
                    {
                        url += "&isExpiredStore=true";
                    }
                    using (var response = client.GetAsync(url).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var JsonString = await response.Content.ReadAsStringAsync();
                            var cust = JsonConvert.DeserializeObject<List<string>>(JsonString);
                            if (cust != null)
                            {
                                result.AddRange(cust);
                            }
                        }
                    }
                }
            }
            if (result != null && result.Any())
            {
                PublishhMessage(string.Join("\n", result.Distinct().ToArray()));
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Worker.CheckDataKeyPromotion("6885,6973,9422,6463", "6886,6974,6613");
                await Task.Delay(7200000, stoppingToken);
            }
        }
    }
}