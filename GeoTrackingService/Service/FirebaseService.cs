using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using GeoTrackingService;
using GeoTrackingService.Interface;

public class FirebaseService: IFirebaseService
{
    private readonly FirebaseClient _firebaseClient;
    private readonly ILogger<FirebaseService> _logger;

    public FirebaseService(string firebaseBaseUrl, ILogger<FirebaseService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrEmpty(firebaseBaseUrl))
        {
            _logger.LogError("Firebase base URL is null or empty.");
            throw new ArgumentException("Firebase base URL cannot be null or empty.", nameof(firebaseBaseUrl));
        }

        _firebaseClient = new FirebaseClient(firebaseBaseUrl);
        _logger.LogInformation("Firebase client initialized with base URL: {FirebaseBaseUrl}", firebaseBaseUrl);
    }
    public async Task<List<(string Key, RSUData Data)>> GetDataAsync()
    {
        try
        {
            var dataSnapshot = await _firebaseClient.Child("").OnceAsync<JObject>();
            // Deserialize each JObject into RsuData objects
            var rsuDataList = dataSnapshot.Select(firebaseObject =>
            {
                var key = firebaseObject.Key;
                var rsuData = firebaseObject.Object.ToObject<RSUData>();
                return (key, rsuData);
            }).ToList();


            return rsuDataList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data.");
            return null;
        }
    }

    public async Task<Incident> GetRSUDataByKeyAsync(string key)
    {
        try
        {
            // Retrieve the RSUData object corresponding to the specified key from Firebase
            var dataSnapshot = await _firebaseClient.Child(key).OnceSingleAsync<Incident>();

            // Check if the dataSnapshot is not null
            if (dataSnapshot != null)
            {
                return dataSnapshot;
            }
            else
            {
                // If the dataSnapshot is null, the RSUData with the specified key was not found
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            _logger.LogError(ex, "Error retrieving RSUData with key {Key}.", key);
            return null;
        }
    }


    public async Task<bool> UpdateRSUDataAsync(string key, Incident newData)
    {
        try
        {
            // Check if the key and new data are not null
            if (key != null && newData != null)
            {
                // Update the RSUData object at the specified key in Firebase
                await _firebaseClient.Child(key).PutAsync(newData);
                return true; // Update successful
            }
            else
            {
                // If the key or new data is null, the update cannot be performed
                _logger.LogWarning("Key or new data is null. Update failed.");
                return false;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            _logger.LogError(ex, "Error updating RSUData with key {Key}.", key);
            return false;
        }
    }

    public async Task<CarGps> GetGpsDataAsync()
    {
        try
        {
            // Retrieve the GPS data from Firebase
            var dataSnapshot = await _firebaseClient.Child("car").OnceSingleAsync<CarGps>();
            // Check if the dataSnapshot is not null
            if (dataSnapshot != null)
            {
                return dataSnapshot;
            }
            else
            {
                // If the dataSnapshot is null, the GPS data was not found
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            _logger.LogError(ex, "Error retrieving GPS data.");
            return null;
        }
    }

    public async Task<List<(string Key, Dictionary<string, object> Data)>> GetIncidentDataAsync()
    {
        try
        {
            var dataSnapshot = await _firebaseClient.Child("").OnceAsync<JObject>();
            var result = new List<(string Key, Dictionary<string, object> Data)>();

            foreach (var firebaseObject in dataSnapshot)
            {
                var key = firebaseObject.Key;
                if (key == "car") continue; // Skip 'car' child

                var incident = firebaseObject.Object.ToObject<Incident>();

                if (incident != null)
                {
                    var data = new Dictionary<string, object>
                    {
                        { "location", incident.Location },
                        { "type", GetIncidentType(incident) },
                        { "status",  incident.Status}
                    };

                    result.Add((key, data));
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving incident data.");
            return null;
        }
    }

    private string GetIncidentType(Incident incident)
    {
        return incident switch
        {
            _ when incident.Accident == 1 => "accident",
            _ when incident.CongestionRate == 1 => "congestionrate",
            _ when incident.RoadClosure == 1 => "roadclosure",
            _ => "unknown"
        };
    }
}
