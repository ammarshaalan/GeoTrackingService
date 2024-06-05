using GeoTrackingService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GeoTrackingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;

        public DataController(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var data = await _firebaseService.GetDataAsync();

                var response = data.Select(tuple => new Dictionary<string, object>
                {
                    { "key", tuple.Key },
                    { "data", tuple.Data }
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Failed to retrieve data");
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptData(string key)
        {
            try
            {
                // Retrieve the RSUData object corresponding to the specified key
                var rsuData = await _firebaseService.GetRSUDataByKeyAsync(key);

                // If the RSUData object is found, update its status to 1 (accepted)
                if (rsuData != null)
                {
                    if(rsuData.status != 2)
                    {
                        rsuData.status = 2; // Update status to accepted
                        await _firebaseService.UpdateRSUDataAsync(key, rsuData); // Update data in Firebase
                        return Ok("Data accepted successfully");

                    }
                    else
                    {
                        return Ok("Data is active already");
                    }
                }
                else
                {
                    return NotFound("RSUData not found");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Failed to accept data");
            }
        }

        [HttpPost("decline")]
        public async Task<IActionResult> DeclineData(string key)
        {
            try
            {
                // Retrieve the RSUData object corresponding to the specified key
                var rsuData = await _firebaseService.GetRSUDataByKeyAsync(key);

                // If the RSUData object is found, update its status to 0 (declined)
                if (rsuData != null)
                {
                    if(rsuData.status != 1)
                    {
                        rsuData.status = 1; // Update status to declined
                        await _firebaseService.UpdateRSUDataAsync(key, rsuData); // Update data in Firebase
                        return Ok("Data declined successfully");

                    }
                    else
                    {
                        return Ok("Data is rejected already");
                    }
                }
                else
                {
                    return NotFound("RSUData not found");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Failed to decline data");
            }
        }

        [HttpGet("GetIncidentData")]
        public async Task<IActionResult> GetIncidentData()
        {
            try
            {
                var data = await _firebaseService.GetIncidentDataAsync();

                if (data == null)
                {
                    return StatusCode(500, "Failed to retrieve data");
                }

                var response = data.Select(tuple => new Dictionary<string, object>
            {
                { "key", tuple.Key },
                { "location", tuple.Data["location"] },
                { "type", tuple.Data["type"] },
                { "status", tuple.Data["status"] }

            }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "Failed to retrieve data");
            }
        }

        [HttpGet("GetCarGps")]
        public async Task<IActionResult> GetCarGps()
        {
            try
            {
                var carGps = await _firebaseService.GetGpsDataAsync();

                if (carGps == null)
                {
                    return NotFound("No GPS data found.");
                }

                return Ok(carGps);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, $"Failed to retrieve data: {ex.Message}");
            }
        }

    }
}
