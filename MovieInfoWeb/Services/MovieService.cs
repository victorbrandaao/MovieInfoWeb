using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieInfoWeb.Interfaces;
using MovieInfoWeb.Models;
using MovieInfoWeb.Exceptions;

namespace MovieInfoWeb.Services
{
    /// <summary>
    /// Implementação do serviço de filmes usando a API TMDB
    /// </summary>
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<MovieService> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
        private readonly JsonSerializerOptions _jsonOptions;

        public MovieService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<MovieService> logger,
            IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = configuration["TMDB:ApiKey"] ?? throw new ArgumentException("API Key não configurada", nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private async Task<T> GetFromApiAsync<T>(string endpoint, CancellationToken cancellationToken) where T : class
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            return result ?? throw new MovieServiceException("Resposta inválida da API");
        }

        public async Task<MovieSearchResult> SearchMoviesAsync(string query, int page = 1, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query não pode ser vazia", nameof(query));
            if (query.Length < 2)
                throw new ArgumentException("Query deve ter pelo menos 2 caracteres", nameof(query));
            if (page < 1)
                throw new ArgumentException("Página deve ser maior que zero", nameof(page));

            var cacheKey = $"search_{query.ToLower()}_{page}";
            if (_cache.TryGetValue(cacheKey, out MovieSearchResult? cachedResult))
            {
                _logger.LogInformation("Cache hit para busca: {Query} (página {Page})", query, page);
                return cachedResult!;
            }

            try
            {
                var url = $"search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}&language=pt-BR";
                var result = await GetFromApiAsync<MovieSearchResult>(url, cancellationToken);
                
                _cache.Set(cacheKey, result, CacheDuration);
                return result;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao buscar filmes: {Query}", query);
                throw new MovieServiceException("Erro ao buscar filmes", ex);
            }
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("ID deve ser maior que zero", nameof(id));

            var cacheKey = $"movie_{id}";
            if (_cache.TryGetValue(cacheKey, out MovieDetails? cachedMovie))
            {
                _logger.LogInformation("Cache hit para filme: {Id}", id);
                return cachedMovie!;
            }

            try
            {
                var url = $"movie/{id}?api_key={_apiKey}&language=pt-BR";
                var movie = await GetFromApiAsync<MovieDetails>(url, cancellationToken);
                
                _cache.Set(cacheKey, movie, CacheDuration);
                return movie;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new MovieNotFoundException(id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do filme: {Id}", id);
                throw new MovieServiceException($"Erro ao buscar detalhes do filme {id}", ex);
            }
        }

        public async Task<MovieSearchResult> GetMoviesByGenreAsync(int genreId, int page = 1, CancellationToken cancellationToken = default)
        {
            if (genreId <= 0)
                throw new ArgumentException("ID do gênero deve ser maior que zero", nameof(genreId));
            if (page < 1)
                throw new ArgumentException("Página deve ser maior que zero", nameof(page));

            var cacheKey = $"genre_{genreId}_{page}";
            if (_cache.TryGetValue(cacheKey, out MovieSearchResult? cachedResult))
            {
                _logger.LogInformation("Cache hit para gênero: {GenreId} (página {Page})", genreId, page);
                return cachedResult!;
            }

            try
            {
                var url = $"discover/movie?api_key={_apiKey}&with_genres={genreId}&page={page}&language=pt-BR";
                var result = await GetFromApiAsync<MovieSearchResult>(url, cancellationToken);
                
                _cache.Set(cacheKey, result, CacheDuration);
                return result;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao buscar filmes por gênero: {GenreId}", genreId);
                throw new MovieServiceException("Erro ao buscar filmes por gênero", ex);
            }
        }

        public async Task<MovieSearchResult> GetPopularMoviesAsync(int page = 1, CancellationToken cancellationToken = default)
        {
            if (page < 1)
                throw new ArgumentException("Página deve ser maior que zero", nameof(page));

            var cacheKey = $"popular_{page}";
            if (_cache.TryGetValue(cacheKey, out MovieSearchResult? cachedResult))
            {
                _logger.LogInformation("Cache hit para filmes populares (página {Page})", page);
                return cachedResult!;
            }

            try
            {
                var url = $"movie/popular?api_key={_apiKey}&page={page}&language=pt-BR";
                var result = await GetFromApiAsync<MovieSearchResult>(url, cancellationToken);
                
                _cache.Set(cacheKey, result, CacheDuration);
                return result;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro ao buscar filmes populares");
                throw new MovieServiceException("Erro ao buscar filmes populares", ex);
            }
        }
    }
}