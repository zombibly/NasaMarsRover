using Common.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasaMarsRover.Apis
{
    [Header("nasa-mars-rovers", "RestEase")]
    public interface INasaMarsRoversApi
    {
        [Query("api_key")]
        string ApiKey { get; set; }

        [Get("{rover}/photos")]
        Task<List<Photo>> GetPhotos([Path] string rover, [Query("earth_date", Format = "YYYY-MM-DD")] DateTime earthDate);
    }
}
