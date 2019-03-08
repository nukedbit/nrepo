﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace NRepo.Services
{
    public interface ITemplateFilesService
    {
        Task<List<string>> DownloadAsync();
    }
}