using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    public class ProcessDetails
    {
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string State { get; set; }
        public string User { get; set; }
        public string Memory { get; set; }
        public string Description { get; set; }
    }
}
