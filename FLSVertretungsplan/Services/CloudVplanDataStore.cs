using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        HttpClient client;
        Vplan vplan;

        public CloudVplanDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            vplan = new Vplan {
                Changes = new List<Change>()
            };
        }

        public async Task<Vplan> GetVplanAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                var xml = await client.GetStringAsync($"raw/vplan?version=1.2.4");
                vplan = await Task.Run(() => VplanParser.Parse(xml));
            }
            return vplan;
        }

    }

}
