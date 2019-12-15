using Business.Apis.Models;
using RestEase;
using System;
using System.Threading.Tasks;

namespace NasaMarsRover.Apis
{
    [Header("nasa-mars-rovers", "RestEase")]
    public interface INasaMarsRoversApi
    {
        [Query("api_key")]
        string ApiKey { get; set; }

        [Get("rovers/{rover}/photos")]
        Task<PhotoResponse> GetPhotos([Path] string rover, [Query("earth_date", Format = "yyyy-MM-dd")] DateTime earthDate);
    }
}
