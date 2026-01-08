using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Core.Contracts.Results
{
    public class MemberResult
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Phải có constructor này thì mới gọi new MemberResult(id, name) được
        public MemberResult(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
