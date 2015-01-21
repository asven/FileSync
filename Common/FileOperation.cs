using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Common
{
    public class FileOperation
    {
        public Enumerations.Enumeration.ActionType FileActionType { get; set; }

        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}
