using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace CyberLeePart2
{
    public class ActivityLogEntry
    {
        public int ActivityId { get; set; }

        public string ActivityDescription { get; set; } = "";

        public DateTime ActivityDate { get; set; }
    }
}