using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Core.Contracts.Results
{
    public class MovieCreateResult
    {
        public int Id { get; set; }
        public bool Success { get; set; }
    }
}
