using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;

namespace CyberLeePart2
{
    public static class DatabaseConfig
    {
        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("CYBERLEE_DB_CONNECTION")
            ?? throw new InvalidOperationException(
                "Database connection settings are missing.");
    }
}

    
    

