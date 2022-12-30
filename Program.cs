using System.Net;
using Geocoding;
using Geocoding.Google;
using Google.Api.Gax.Grpc;
using Google.Maps.Routing.V2;
using Google.Type;
using Grpc.Core;

namespace GoogleMapsApi
{



    internal class Program
    {

        public const string ApiUrl = "https://routes.googleapis.com";
        static void Main(string[] args)
        {
            LatLng kaunas = new LatLng() { Latitude = 54.900880, Longitude = 23.908480 };
            LatLng vilnius = new LatLng() { Latitude = 54.684514, Longitude = 25.277152 };
            Waypoint origin = new Waypoint()
            {
                Location = new Google.Maps.Routing.V2.Location
                {
                    LatLng = kaunas,
                    Heading = 0
                },
            };
            Waypoint destination = new Waypoint()
            {
                Location = new Google.Maps.Routing.V2.Location
                {
                    LatLng = vilnius,
                    Heading = 0
                },
            };



            RoutesClient client = RoutesClient.Create();
            CallSettings callSettings = CallSettings.FromHeader("X-Goog-FieldMask", "*");
            ComputeRoutesRequest request = new ComputeRoutesRequest
            {
                Origin = origin,
                Destination = destination,
                TravelMode = RouteTravelMode.Drive,

            };
            var response = client.ComputeRoutes(request, callSettings);

            IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "Cant Push On Github" };
            var originAddresses = geocoder.ReverseGeocode(new Geocoding.Location(origin.Location.LatLng.Latitude, origin.Location.LatLng.Longitude));
            var destinationAddresses = geocoder.ReverseGeocode(new Geocoding.Location(destination.Location.LatLng.Latitude, destination.Location.LatLng.Longitude));

            Console.WriteLine($"From {originAddresses.First().FormattedAddress}, coords: {origin.Location.LatLng}");
            Console.WriteLine($"To {destinationAddresses.First().FormattedAddress}, coords: {destination.Location.LatLng}");

            Console.WriteLine("The distance is: " + response.Routes[0].DistanceMeters / 1000 + "km");
            Console.WriteLine("duration in hours: " + (float)response.Routes[0].Duration.Seconds / 3600f);
            Console.WriteLine("---------------Json--------------");        
            Console.WriteLine(response);

            int i = 1;
            foreach (var leg in response.Routes[0].Legs)
            {
                //response.Routes[0].
                foreach (var step in leg.Steps)
                {
                    Console.WriteLine($"-----------step {i}----------");
                    Console.WriteLine($"---distance: {step.DistanceMeters}");
                    Console.WriteLine($"---Polyline: {step.Polyline.EncodedPolyline}");
                    Console.WriteLine($"---Navigation: {step?.NavigationInstruction}");
                    //Console.WriteLine($"---nav instructions: {step.NavigationInstruction.Instructions}");
                    Console.WriteLine("-------------------------------");

                    i++;
                }
                i = 1;
            }
            //Console.WriteLine(   response.Routes[0].CalculateSize());
        }
    }
}