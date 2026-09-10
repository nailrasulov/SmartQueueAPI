using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Application.DTO_s
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }   
        public string Status { get; set; } = default!;
        public int? QueuePosition { get; set; }
    }
}
