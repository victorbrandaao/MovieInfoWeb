using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Representa um filme e suas informações básicas
/// </summary>
public class Movie
{
    /// <summary>
    /// ID único do filme
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Título do filme
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Sinopse do filme
    /// </summary>
    [StringLength(2000)]
    public string Overview { get; init; } = string.Empty;

    /// <summary>
    /// Data de lançamento do filme
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; init; }

    /// <summary>
    /// Caminho para o poster do filme
    /// </summary>
    [Url]
    public string PosterPath { get; init; } = string.Empty;

    /// <summary>
    /// Nota média do filme
    /// </summary>
    [Range(0, 10)]
    public decimal Rating { get; set; }

    /// <summary>
    /// Gêneros do filme
    /// </summary>
    public IReadOnlyCollection<string> Genres { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}