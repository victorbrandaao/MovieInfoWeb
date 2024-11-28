using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieInfoWeb.Interfaces;
using MovieInfoWeb.Models;
using System.Threading;
using System.Threading.Tasks;
using MovieInfoWeb.Exceptions;

namespace MovieInfoWeb.Controllers
{
    /// <summary>
    /// Controlador responsável por operações relacionadas a filmes
    /// </summary>
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Exibe a página inicial
        /// </summary>
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var popularMovies = await _movieService.GetPopularMoviesAsync(page: 1, cancellationToken);
                return View(popularMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página inicial");
                return View("Error", new ErrorViewModel { Message = "Erro ao carregar filmes populares" });
            }
        }

        /// <summary>
        /// Realiza busca de filmes
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Search(string query, int page = 1, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Termo de busca é obrigatório");

            try
            {
                var result = await _movieService.SearchMoviesAsync(query, page, cancellationToken);
                return View("SearchResults", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca: {Query}", query);
                return View("Error", new ErrorViewModel { Message = "Erro ao realizar busca" });
            }
        }

        /// <summary>
        /// Exibe detalhes de um filme
        /// </summary>
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            try
            {
                var movie = await _movieService.GetMovieDetailsAsync(id, cancellationToken);
                return View(movie);
            }
            catch (MovieNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do filme: {Id}", id);
                return View("Error", new ErrorViewModel { Message = "Erro ao carregar detalhes do filme" });
            }
        }

        /// <summary>
        /// Lista filmes por gênero
        /// </summary>
        public async Task<IActionResult> ByGenre(int genreId, int page = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _movieService.GetMoviesByGenreAsync(genreId, page, cancellationToken);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar filmes por gênero: {GenreId}", genreId);
                return View("Error", new ErrorViewModel { Message = "Erro ao carregar filmes do gênero" });
            }
        }

        /// <summary>
        /// Lista filmes populares
        /// </summary>
        public async Task<IActionResult> Popular(int page = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _movieService.GetPopularMoviesAsync(page, cancellationToken);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar filmes populares");
                return View("Error", new ErrorViewModel { Message = "Erro ao carregar filmes populares" });
            }
        }
    }
}