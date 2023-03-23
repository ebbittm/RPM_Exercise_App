using Microsoft.Extensions.Configuration;
using RPM_Exercise.Accessors;
using RPM_Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPM_Exercise.Managers
{
    public class PetroleumDataManager
    {
        private readonly IConfiguration _configuration;
        private PetroleumAccessor _accessor;

        public PetroleumDataManager(IConfiguration config, PetroleumAccessor accessor)
        {
            _configuration = config;
            _accessor = accessor;
        }

        public async Task<List<PetroleumDto>> CreatePetroleumData(List<PetroleumDto> dtos)
        {
            var parameterDaysAgo = _configuration.GetSection("Parameters").GetSection("NDaysAgo").Value;
            int nDaysAgo = 365;
            int.TryParse(parameterDaysAgo, out nDaysAgo);
            List<PetroleumDto> newData = new List<PetroleumDto>();
            DateOnly pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-nDaysAgo));

            foreach (PetroleumDto dto in dtos)
            {
                if (dto.Period.CompareTo(pastDate) >= 0)
                {
                    newData.Add(await CreatePetroleumData(dto));
                }
            }

            return newData;
        }

        public async Task<PetroleumDto> CreatePetroleumData(PetroleumDto newDto)
        {
            PetroleumDto createdPetroleum = await _accessor.CreatePetroleumEntry(newDto);

            return createdPetroleum;
        }

        public async Task DeleteOldPetroleumData()
        {
            var parameterDaysAgo = _configuration.GetSection("Parameters").GetSection("NDaysAgo").Value;
            int nDaysAgo = 365;
            int.TryParse(parameterDaysAgo, out nDaysAgo);

            var petroleumDtosToDelete = await _accessor.GetDataAfterDays(nDaysAgo);

            foreach(PetroleumDto dto in petroleumDtosToDelete)
            {
                await _accessor.DeletePetroleum(dto);
            }
        }
    }
}
