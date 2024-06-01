using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WebApplication1;

public class FirebaseService
{
    private readonly FirebaseClient _firebaseClient;

    public FirebaseService(string firebaseBaseUrl)
    {
        _firebaseClient = new FirebaseClient(firebaseBaseUrl);
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
            // Handle exceptions
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
            Console.WriteLine($"Error retrieving RSUData with key {key}: {ex.Message}");
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
                Console.WriteLine("Key or new data is null. Update failed.");
                return false;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error updating RSUData with key {key}: {ex.Message}");
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
            Console.WriteLine($"Error retrieving GPS data: {ex.Message}");
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
            // Handle exceptions
            return null;
        }
    }

    private string GetIncidentType(Incident incident)
    {
        if (incident.Accident == 1)
        {
            return "accident";
        }
        else if (incident.CongestionRate == 1)
        {
            return "congestionrate";
        }
        else if (incident.RoadClosure == 1)
        {
            return "roadclosure";
        }
        return "unknown";
    }
}
