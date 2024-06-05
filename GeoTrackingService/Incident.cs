namespace GeoTrackingService
{
    public class Incident
    {
        public int accident { get; set; }
        public int congestionRate { get; set; }
        public List<double>? location { get; set; }
        public int roadClosure { get; set; }
        public int? status { get; set; }
    }
}
