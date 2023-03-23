using Microsoft.EntityFrameworkCore;
using RPM_Exercise.Entities;
using RPM_Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RPM_Exercise.Accessors
{
    public class PetroleumAccessor
    {
        private PetroleumContext context;

        public PetroleumAccessor(PetroleumContext context)
        {
            this.context = context;
        }

        public async Task<PetroleumDto> CreatePetroleumEntry(PetroleumDto newPetroleumDto)
        {
            var petroleumEntity = context.Petroleum.Where(x => x.Period == newPetroleumDto.Period).FirstOrDefault();

            if (petroleumEntity != null)
            {
                Console.WriteLine("A petroleum data log for the period: " + petroleumEntity.Period.ToString() + " already exists. Skipping.");
                return PetroleumDto.FromEntity(petroleumEntity);
            }

            PetroleumDataEntity newEntity = newPetroleumDto.ToEntity();

            context.Petroleum.Add(newEntity);

            await context.SaveChangesAsync();

            return PetroleumDto.FromEntity(newEntity);
        }

        public async Task DeletePetroleum(PetroleumDto dtoToDelete)
        {
            var entityToDelete = context.Petroleum.Where(x => x.Period == dtoToDelete.Period);

            context.Remove(entityToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<List<PetroleumDto>> GetDataAfterDays(int daysAgo)
        {

            DateOnly pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

            var entitiesToDelete = await context.Petroleum.Where(x => x.Period.CompareTo(pastDate) < 0).Select(petroleumEntity => PetroleumDto.FromEntity(petroleumEntity)).ToListAsync();

            return entitiesToDelete;
        }
    }
}
