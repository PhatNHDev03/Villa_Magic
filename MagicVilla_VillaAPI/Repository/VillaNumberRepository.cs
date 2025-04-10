﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _context;
         
        public VillaNumberRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<VillaNumber> Update(VillaNumber entity)
        {
            entity.updateDate = DateTime.Now;
            _context.villaNumbers.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
