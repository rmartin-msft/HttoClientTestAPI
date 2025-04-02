using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nasa.Neo
{
  public class NeoResponse
  {
    [JsonPropertyName("links")]
    public Links Links { get; set; }

    [JsonPropertyName("page")]
    public Page Page { get; set; }

    [JsonPropertyName("near_earth_objects")]
    public List<NearEarthObject> NearEarthObjects { get; set; }
  }

  public class Links
  {
    [JsonPropertyName("next")]
    public string Next { get; set; }

    [JsonPropertyName("self")]
    public string Self { get; set; }
  }

  public class Page
  {
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("total_elements")]
    public int TotalElements { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }
  }

  public class NearEarthObject
  {
    [JsonPropertyName("links")]
    public Links Links { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("neo_reference_id")]
    public string NeoReferenceId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_limited")]
    public string NameLimited { get; set; }

    [JsonPropertyName("designation")]
    public string Designation { get; set; }

    [JsonPropertyName("nasa_jpl_url")]
    public string NasaJplUrl { get; set; }

    [JsonPropertyName("absolute_magnitude_h")]
    public double AbsoluteMagnitudeH { get; set; }

    [JsonPropertyName("estimated_diameter")]
    public EstimatedDiameter EstimatedDiameter { get; set; }

    [JsonPropertyName("is_potentially_hazardous_asteroid")]
    public bool IsPotentiallyHazardousAsteroid { get; set; }

    [JsonPropertyName("close_approach_data")]
    public List<CloseApproachData> CloseApproachData { get; set; }

    [JsonPropertyName("orbital_data")]
    public OrbitalData OrbitalData { get; set; }

    [JsonPropertyName("is_sentry_object")]
    public bool IsSentryObject { get; set; }
  }

  public class EstimatedDiameter
  {
    [JsonPropertyName("kilometers")]
    public DiameterUnit Kilometers { get; set; }

    [JsonPropertyName("meters")]
    public DiameterUnit Meters { get; set; }

    [JsonPropertyName("miles")]
    public DiameterUnit Miles { get; set; }

    [JsonPropertyName("feet")]
    public DiameterUnit Feet { get; set; }
  }

  public class DiameterUnit
  {
    [JsonPropertyName("estimated_diameter_max")]
    public double EstimatedDiameterMax { get; set; }
  }

  public class CloseApproachData
  {
    [JsonPropertyName("orbiting_body")]
    public string OrbitingBody { get; set; }
  }

  public class OrbitalData
  {
    [JsonPropertyName("orbit_id")]
    public string OrbitId { get; set; }

    [JsonPropertyName("orbit_determination_date")]
    public string OrbitDeterminationDate { get; set; }

    [JsonPropertyName("first_observation_date")]
    public string FirstObservationDate { get; set; }

    [JsonPropertyName("last_observation_date")]
    public string LastObservationDate { get; set; }

    [JsonPropertyName("data_arc_in_days")]
    public int DataArcInDays { get; set; }

    [JsonPropertyName("observations_used")]
    public int ObservationsUsed { get; set; }

    [JsonPropertyName("orbit_uncertainty")]
    public string OrbitUncertainty { get; set; }

    [JsonPropertyName("minimum_orbit_intersection")]
    public string MinimumOrbitIntersection { get; set; }

    [JsonPropertyName("jupiter_tisserand_invariant")]
    public string JupiterTisserandInvariant { get; set; }

    [JsonPropertyName("epoch_osculation")]
    public string EpochOsculation { get; set; }

    [JsonPropertyName("eccentricity")]
    public string Eccentricity { get; set; }

    [JsonPropertyName("semi_major_axis")]
    public string SemiMajorAxis { get; set; }

    [JsonPropertyName("inclination")]
    public string Inclination { get; set; }

    [JsonPropertyName("ascending_node_longitude")]
    public string AscendingNodeLongitude { get; set; }

    [JsonPropertyName("orbital_period")]
    public string OrbitalPeriod { get; set; }

    [JsonPropertyName("perihelion_distance")]
    public string PerihelionDistance { get; set; }

    [JsonPropertyName("perihelion_argument")]
    public string PerihelionArgument { get; set; }

    [JsonPropertyName("aphelion_distance")]
    public string AphelionDistance { get; set; }

    [JsonPropertyName("perihelion_time")]
    public string PerihelionTime { get; set; }

    [JsonPropertyName("mean_anomaly")]
    public string MeanAnomaly { get; set; }

    [JsonPropertyName("mean_motion")]
    public string MeanMotion { get; set; }

    [JsonPropertyName("equinox")]
    public string Equinox { get; set; }
  }
}