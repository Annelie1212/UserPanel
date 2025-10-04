using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UserPanelWPF.Models;

namespace UserPanelWPF.Services
{
    public static class VDClientService
    {
        //We get the status of the VD using a POST (not a simple GET this time!).
        public static async Task<VDFetchStatusResponse> FetchStatusVDAsync()
        {
            //Fake, sätt senare till att ändras dynamiskt beroende på inloggad user.
            var vDFetchStatusRequest = new VDFetchStatusRequest()
            {
                VibrationDetectorId = 12,
                UserId = 1, // This should be dynamically set based on the logged-in user
                UserPanelActionDate = DateTime.Now
            };

            using var http = new HttpClient();

            var fetch_result = await http.PostAsJsonAsync("https://localhost:7034/api/VibrationDetectorsSync", vDFetchStatusRequest);

            if (fetch_result.IsSuccessStatusCode)
            {
                var responseContent = await fetch_result.Content.ReadFromJsonAsync<VDFetchStatusResponse>();
                if (responseContent != null && responseContent.RequestSuccessful)
                {
                    VDFetchStatusResponse response = new VDFetchStatusResponse();
                    response.VibrationDetectorId = responseContent.VibrationDetectorId;
                    response.DeviceName = responseContent.DeviceName;
                    response.Location = responseContent.Location;
                    response.AlarmArmed = responseContent.AlarmArmed;
                    response.AlarmTriggered = responseContent.AlarmTriggered;
                    response.VibrationLevel = responseContent.VibrationLevel;
                    response.VibrationLevelThreshold = responseContent.VibrationLevelThreshold;
                    response.RequestSuccessful = responseContent.RequestSuccessful;
                    response.ErrorMessage = responseContent.ErrorMessage;


                    //string totalStatusResponseString = $"Id:{responseContent.VibrationDetectorId}, Name:{responseContent.DeviceName}, Location:{responseContent.Location}, " +
                    //    $"AlarmArmed:{responseContent.AlarmArmed}, AlarmTriggered:{responseContent.AlarmTriggered}, " +
                    //    $"VibrationLevel:{responseContent.VibrationLevel}, VibrationLevelThreshold:{responseContent.VibrationLevelThreshold}";
                    //return $"Vibration Detector updated successfully with status:{totalStatusResponseString}";
                    return response;
                }
                else
                {
                    //return $"Error: {responseContent?.ErrorMessage ?? "Unknown error"}";
                    return new VDFetchStatusResponse()
                    {
                        RequestSuccessful = false,
                        ErrorMessage = responseContent?.ErrorMessage ?? "null error"
                    };
                }
            }
            else
            {
                //return $"HTTP Error: {fetch_result.StatusCode}";
                return new VDFetchStatusResponse()
                {
                    RequestSuccessful = false,
                    ErrorMessage = $"HTTP Error: {fetch_result.StatusCode}"
                };
            }
        }

        public async static Task<string> SetVDAsync()
        {

            var changeValueRequest = new VDChangeValueRequest()
            {
                VibrationDetectorId = 1,
                UserPanelAction = "SetVibrationDetector",
                NewValue = 10,
                UserId = 1, // This should be dynamically set based on the logged-in user
                UserPanelActionDate = DateTime.Now
            };
            using var http = new HttpClient();

            var post_result = await http.PostAsJsonAsync("https://localhost:7034/api/VibrationDetectors", changeValueRequest);
            //var get_result = await http.GetFromJsonAsync<VDChangeValueResponse>("https://localhost:7170/api/products");

           
            if (post_result.IsSuccessStatusCode)
            {
                var responseContent = await post_result.Content.ReadFromJsonAsync<VDChangeValueResponse>();
                if (responseContent != null && responseContent.RequestSuccessful)
                {
                    return $"Vibration Detector updated successfully.from id:{responseContent.VibrationDetectorId}";
                }
                else
                {
                    return $"Error: {responseContent?.ErrorMessage ?? "Unknown error"}";
                }
            }
            else
            {
                return $"HTTP Error: {post_result.StatusCode}";
            }
                                              
        }
    }
}
