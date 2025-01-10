﻿using Core.Entities;
using Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Abstract;

public interface INewsRepository : IBaseRepository<News>
{
    Task<News> GetNewsByTitle(string title);
    Task<List<News>> GetLastThreeNews();
}
