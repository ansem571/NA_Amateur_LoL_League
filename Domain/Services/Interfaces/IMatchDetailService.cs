﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IMatchDetailService
    {
        Task<bool> SendFileData(MatchSubmissionView view);
    }
}