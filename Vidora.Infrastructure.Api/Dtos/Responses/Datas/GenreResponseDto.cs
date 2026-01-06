using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas
{
    public record GenreResponseDto
    {
        public bool Success { get; set; }

        public List<GenreDto> Data { get; set; } = new();
    }

    public record GenreDto
    {
        [JsonPropertyName("genreId")]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
