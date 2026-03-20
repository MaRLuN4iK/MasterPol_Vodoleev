using Microsoft.EntityFrameworkCore;
using MasterPol.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterPol.Data.Database
{
    public class PartnerRepository
    {
        private readonly AppDbContext _context;

        public PartnerRepository()
        {
            _context = new AppDbContext();
        }

        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            return await _context.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.SaleHistories)
                    .ThenInclude(sh => sh.Product)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Partner> GetPartnerByIdAsync(int id)
        {
            return await _context.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.SaleHistories)
                    .ThenInclude(sh => sh.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddPartnerAsync(Partner partner)
        {
            try
            {
                partner.CreatedAt = DateTime.Now;
                partner.UpdatedAt = DateTime.Now;
                await _context.Partners.AddAsync(partner);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                partner.UpdatedAt = DateTime.Now;
                _context.Partners.Update(partner);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletePartnerAsync(int id)
        {
            try
            {
                var partner = await _context.Partners.FindAsync(id);
                if (partner != null)
                {
                    _context.Partners.Remove(partner);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PartnerType>> GetAllPartnerTypesAsync()
        {
            return await _context.PartnerTypes
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        public async Task<List<SaleHistory>> GetPartnerSaleHistoryAsync(int partnerId)
        {
            return await _context.SaleHistories
                .Include(sh => sh.Product)
                .Where(sh => sh.PartnerId == partnerId)
                .OrderByDescending(sh => sh.SaleDate)
                .ToListAsync();
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> AddSaleHistoryAsync(SaleHistory saleHistory)
        {
            try
            {
                saleHistory.CreatedAt = DateTime.Now;
                await _context.SaleHistories.AddAsync(saleHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}