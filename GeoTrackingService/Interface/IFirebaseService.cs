using GeoTrackingService;

namespace GeoTrackingService.Interface
{
    public interface IFirebaseService
    {
        Task<List<(string Key, RSUData Data)>> GetDataAsync();
        Task<Incident> GetRSUDataByKeyAsync(string key);
        Task<bool> UpdateRSUDataAsync(string key, Incident newData);
        Task<CarGps> GetGpsDataAsync();
        Task<List<(string Key, Dictionary<string, object> Data)>> GetIncidentDataAsync();
    }
}
