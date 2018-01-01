using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class CloudVplanLoader: IVplanLoader
    {
        public async Task<Vplan> Load()
        {
            using (var client = new HttpClient { BaseAddress = new Uri($"{App.BackendUrl}/") })
            {
                var xml = await client.GetStringAsync($"raw/vplan?version=1.2.4");
                return await VplanParser.Parse(xml);
            }
        }
    }
}
