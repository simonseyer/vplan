using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class CloudVplanLoader: IVplanLoader
    {
        HttpClient client;

        public CloudVplanLoader()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri($"{App.BackendUrl}/")
            };
        }

        public async Task<Vplan> Load()
        {
            var xml = await client.GetStringAsync($"raw/vplan?version=1.2.4");
            return await VplanParser.Parse(xml);
        }
    }
}
