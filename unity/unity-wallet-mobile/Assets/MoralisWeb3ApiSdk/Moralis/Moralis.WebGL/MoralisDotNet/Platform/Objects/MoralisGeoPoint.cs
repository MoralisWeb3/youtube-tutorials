using System;
using System.Collections.Generic;
using System.Text;

namespace Moralis.WebGL.Platform.Objects
{
    public class MoralisGeoPoint
    {
        /// <summary>
        /// Constructs a MoralisGeoPoint with the specified latitude and longitude.
        /// </summary>
        /// <param name="latitude">The point's latitude.</param>
        /// <param name="longitude">The point's longitude.</param>
        public MoralisGeoPoint(double latitude, double longitude)
        //  : this()
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        private double latitude;
        /// <summary>
        /// Gets or sets the latitude of the GeoPoint. Valid range is [-90, 90].
        /// Extremes should not be used.
        /// </summary>
        public double Latitude
        {
            get => latitude;
            set
            {
                if (value > 90 || value < -90)
                {
                    throw new ArgumentOutOfRangeException("value",
                      "Latitude must be within the range [-90, 90]");
                }
                latitude = value;
            }
        }

        private double longitude;
        /// <summary>
        /// Gets or sets the longitude. Valid range is [-180, 180].
        /// Extremes should not be used.
        /// </summary>
        public double Longitude
        {
            get => longitude;
            set
            {
                if (value > 180 || value < -180)
                {
                    throw new ArgumentOutOfRangeException("value",
                      "Longitude must be within the range [-180, 180]");
                }
                longitude = value;
            }
        }

        /// <summary>
        /// Get the distance in radians between this point and another GeoPoint. This is the smallest angular
        /// distance between the two points.
        /// </summary>
        /// <param name="point">GeoPoint describing the other point being measured against.</param>
        /// <returns>The distance in between the two points.</returns>
        public MoralisGeoDistance DistanceTo(MoralisGeoPoint point)
        {
            double d2r = Math.PI / 180; // radian conversion factor
            double lat1rad = Latitude * d2r;
            double long1rad = longitude * d2r;
            double lat2rad = point.Latitude * d2r;
            double long2rad = point.Longitude * d2r;
            double deltaLat = lat1rad - lat2rad;
            double deltaLong = long1rad - long2rad;
            double sinDeltaLatDiv2 = Math.Sin(deltaLat / 2);
            double sinDeltaLongDiv2 = Math.Sin(deltaLong / 2);
            // Square of half the straight line chord distance between both points.
            // [0.0, 1.0]
            double a = sinDeltaLatDiv2 * sinDeltaLatDiv2 +
              Math.Cos(lat1rad) * Math.Cos(lat2rad) * sinDeltaLongDiv2 * sinDeltaLongDiv2;
            a = Math.Min(1.0, a);
            return new MoralisGeoDistance(2 * Math.Asin(Math.Sqrt(a)));
        }

        //IDictionary<string, object> IJsonConvertible.ConvertToJSON() => new Dictionary<string, object> {
        //    {"__type", "GeoPoint"},
        //    {nameof(latitude), Latitude},
        //    {nameof(longitude), Longitude}
        //  };
    }
}
