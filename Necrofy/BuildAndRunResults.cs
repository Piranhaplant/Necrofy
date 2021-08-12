using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class BuildAndRunResults
    {
        public BuildResults BuildResults { get; private set; }
        public Process EmulatorProcess { get; private set; }

        public BuildAndRunResults(BuildResults buildResults, Process emulatorProcess = null) {
            BuildResults = buildResults;
            EmulatorProcess = emulatorProcess;
        }
    }
}
