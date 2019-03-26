﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace XWorkUp.AspNetCoreMvc.Models
{
    public class PieRepository: IPieRepository
    {
        private readonly ApplicationDbContext _appDbContext;

        public PieRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Pie> Pies
        {
            get
            {
                return _appDbContext.Pies.Include(c => c.Category);
            }
        }

        public IEnumerable<Pie> PiesOfTheWeek
        {
            get
            {
                return _appDbContext.Pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);
            }
        }

        public Pie GetPieById(int pieId)
        {
            return _appDbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
        }

        public void UpdatePie(Pie pie)
        {
            _appDbContext.Pies.Update(pie);
            _appDbContext.SaveChanges();
        }

        public void CreatePie(Pie pie)
        {
            _appDbContext.Pies.Add(pie);
            _appDbContext.SaveChanges();
        }
    }
}
