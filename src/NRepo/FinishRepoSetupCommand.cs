﻿using System.Collections.Generic;

namespace NRepo
{
    public class FinishRepoSetupCommand
    {
        public IEnumerable<string> FilesToAdd { get; }
        public string CloneUrl { get; }

        public FinishRepoSetupCommand(IEnumerable<string> filesToAdd, string cloneUrl)
        {
            FilesToAdd = filesToAdd;
            CloneUrl = cloneUrl;
        }
    }
}