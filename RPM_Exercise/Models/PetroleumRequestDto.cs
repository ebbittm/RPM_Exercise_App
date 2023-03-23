using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPM_Exercise.Models
{
    public class PetroleumRequestDto
    {
        public string Total { get; set; }
        public string DateFormat { get; set; }
        public string Frequency { get; set; }
        public List<PetroleumDto> Data { get; set; }

        public PetroleumRequestDto()
        {

        }
    }
}
