using System.ComponentModel.DataAnnotations;

namespace ConnectFour.Models
{
    public record PlayerRequest
    {
        public string Id { get; init; }
        [Required]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public bool IsIngame { get; init; }
    }
}
