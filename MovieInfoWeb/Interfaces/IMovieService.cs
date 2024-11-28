using System.Threading;
using System.Threading.Tasks;
using MovieInfoWeb.Models;

namespace MovieInfoWeb.Interfaces
{
    /// <summary>
    /// Interface que define operações relacionadas a filmes
    /// </summary>
    public interface IMovieService
    {
        /// <summary>
        /// Realiza busca de filmes baseado em um termo
        /// </summary>
        /// <param name="query">Termo para busca (mínimo 2 caracteres)</param>
        /// <param name="page">Página dos resultados (mínimo 1)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado paginado da busca de filmes</returns>
        Task<MovieSearchResult> SearchMoviesAsync(string query, int page = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém detalhes de um filme específico
        /// </summary>
        /// <param name="id">ID do filme</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Detalhes completos do filme</returns>
        Task<MovieDetails> GetMovieDetailsAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém filmes por gênero
        /// </summary>
        /// <param name="genreId">ID do gênero</param>
        /// <param name="page">Página dos resultados</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de filmes do gênero especificado</returns>
        Task<MovieSearchResult> GetMoviesByGenreAsync(int genreId, int page = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém lista de filmes populares
        /// </summary>
        /// <param name="page">Página dos resultados</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de filmes populares</returns>
        Task<MovieSearchResult> GetPopularMoviesAsync(int page = 1, CancellationToken cancellationToken = default);
    }
}