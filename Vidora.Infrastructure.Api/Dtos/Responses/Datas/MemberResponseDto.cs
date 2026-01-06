using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas
{
    public record MemberResponseDto
    {
        public bool Success { get; set; }
        public List<MemberDto> Data { get; set; } = new();
    }

    public record MemberDto
    {
        [JsonPropertyName("memberId")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
