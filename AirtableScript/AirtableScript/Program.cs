using AirtableApiClient;
using Geocoding.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirtableScript
{
    class Handler
    {
        readonly static string googleApiKey = "XXX";

        readonly static string baseId = "appIlXTISIivP7QnW";
        readonly static string appKey = "keyqxHaTyUVBnqKfW";
        readonly static string tableName = "tbllU935iLZg4LI02";
        readonly static string recordId = "recU9DNhmZn7Y5MIH";
        readonly static string addressFieldName = "Address";
        readonly static string coordsFiledName = "Coordinates";

        public static async Task Main()
        {
            try
            {
                var record = await GetRecord(recordId);
                var coords = await GetCoordsFromAddress(record.Fields[addressFieldName].ToString());
                var upd = await UpdateField(coordsFiledName, coords, recordId);
                Console.WriteLine("\n-------> Updated: " + upd.Record.Fields[coordsFiledName]);
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n-------> ERROR: " + ex.Message);
            }          

        }

        async static Task<AirtableRecord> GetRecord(string recordId)
        {
            using (var ab = new AirtableBase(appKey, baseId))
            {
                var res = await ab.RetrieveRecord(tableName, recordId);

                if (!res.Success)
                {
                    throw new Exception(message: "GetRecord: " + (res.AirtableApiError is AirtableApiException ? res.AirtableApiError.ErrorMessage : "Unknown Error"));
                }
                return res.Record;
            }
        }

        static async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateField(string fieldName, string fieldValue, string recordId)
        {
            using (var ab = new AirtableBase(appKey, baseId))
            {
                var fields = new Fields();
                fields.AddField(fieldName, fieldValue);

                var res = await ab.UpdateRecord(tableName, fields, recordId);

                if (!res.Success)
                {
                    throw new Exception(message: "UpdateField: " + (res.AirtableApiError is AirtableApiException ? res.AirtableApiError.ErrorMessage : "Unknown Error"));
                }
                return res;
            }
        }

        static async Task<string> GetCoordsFromAddress(string address)
        {
            try
            {
                var geocoder = new GoogleGeocoder() { ApiKey = googleApiKey };
                var res = await geocoder.GeocodeAsync(address);
                var coords = res.First().Coordinates;
                return coords.Latitude + ", " + coords.Longitude;
            }
            catch (Exception ex)
            {
                throw new Exception(message: "GetCoordsFromAddress: " + ex.Message);
            }
        }
    }
}
