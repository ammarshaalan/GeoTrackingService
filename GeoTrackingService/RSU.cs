namespace GeoTrackingService
{
    public class RSUData
    {
        public string H_type { get; set; }
        public List<double> location { get; set; }
        public int? status { get; set; }
    }
    public class ResponseData
    {
        public Dictionary<string, RSUData> rsu { get; set; }
    }

}
