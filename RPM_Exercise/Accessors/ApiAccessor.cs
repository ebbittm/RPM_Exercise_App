using RPM_Exercise.Clients;
using RPM_Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPM_Exercise.Accessors
{
    public class ApiAccessor
    {
        private readonly IApiClient apiClient;
        private readonly string petroleumDataPath = "petroleum/pri/gnd/data/?frequency=weekly&data[0]=value&facets[series][]=EMD_EPD2D_PTE_NUS_DPG&sort[0][column]=period&sort[0][direction]=desc&offset=0&length=5000&api_key=EthXWE6eUTrBEJ1uTpNCqbL4NjghRxaC2R5tw1b2";

        public ApiAccessor()
        {
        }

        public ApiAccessor(IApiClient _apiClient)
        {
            apiClient = _apiClient;
        }

        public async Task<PetroleumDto> GetPetroleumData()
        {
            return await apiClient.Get<PetroleumDto>(petroleumDataPath);
        }
    }
}
