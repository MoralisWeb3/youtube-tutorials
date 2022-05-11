using System;
using System.Collections.Generic;
using System.Text;

namespace Moralis.WebGL.Platform.Objects
{
    /// <summary>
    /// Represents a distance between two MoralisGeoPoints.
    /// </summary>
    public struct MoralisGeoDistance
    {
        private const double EarthMeanRadiusKilometers = 6371.0;
        private const double EarthMeanRadiusMiles = 3958.8;

        /// <summary>
        /// Creates a ParseGeoDistance.
        /// </summary>
        /// <param name="radians">The distance in radians.</param>
        public MoralisGeoDistance(double radians)
          : this() => Radians = radians;

        /// <summary>
        /// Gets the distance in radians.
        /// </summary>
        public double Radians { get; private set; }

        /// <summary>
        /// Gets the distance in miles.
        /// </summary>
        public double Miles => Radians * EarthMeanRadiusMiles;

        /// <summary>
        /// Gets the distance in kilometers.
        /// </summary>
        public double Kilometers => Radians * EarthMeanRadiusKilometers;

        /// <summary>
        /// Gets a MoralisGeoDistance from a number of miles.
        /// </summary>
        /// <param name="miles">The number of miles.</param>
        /// <returns>A MoralisGeoDistance for the given number of miles.</returns>
        public static MoralisGeoDistance FromMiles(double miles) => new MoralisGeoDistance(miles / EarthMeanRadiusMiles);

        /// <summary>
        /// Gets a MoralisGeoDistance from a number of kilometers.
        /// </summary>
        /// <param name="kilometers">The number of kilometers.</param>
        /// <returns>A MoralisGeoDistance for the given number of kilometers.</returns>
        public static MoralisGeoDistance FromKilometers(double kilometers) => new MoralisGeoDistance(kilometers / EarthMeanRadiusKilometers);

        /// <summary>
        /// Gets a MoralisGeoDistance from a number of radians.
        /// </summary>
        /// <param name="radians">The number of radians.</param>
        /// <returns>A MoralisGeoDistance for the given number of radians.</returns>
        public static MoralisGeoDistance FromRadians(double radians) => new MoralisGeoDistance(radians);
    }
}
