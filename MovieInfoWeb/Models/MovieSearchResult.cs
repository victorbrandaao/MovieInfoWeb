using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieInfoWeb.Models
{
    public class MovieSearchResult
    {
        [JsonPropertyName("page")]
        public int Page { get; init; }

        [JsonPropertyName("results")]
        public List<Movie> Results { get; init; } = new();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; init; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; init; }

        public string SearchQuery { get; set; } = string.Empty;

        public bool HasNextPage => Page < TotalPages;
    }
}