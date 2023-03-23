using RPM_Exercise.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPM_Exercise.Models
{
    public class PetroleumDto
    {
        public DateOnly Period { get; set; }
        public string DUOArea { get; set; }
        public string AreaName { get; set; }
        public string Product { get; set; }
        public string ProductName { get; set; }
        public string Process { get; set; }
        public string Series { get; set; }
        public string SeriesDescription { get; set; }
        public double Value { get; set; }
        public string Units { get; set; }

        public PetroleumDataEntity ToEntity()
        {
            return new PetroleumDataEntity()
            {
                Period = Period,
                Price = Value
            };
        }

        public static PetroleumDto FromEntity(PetroleumDataEntity entity)
        {
            if (entity == null) return null;

            var dto = new PetroleumDto();
            dto.Period = entity.Period;
            dto.Value = entity.Price;
            return dto;
        }
    }
}
