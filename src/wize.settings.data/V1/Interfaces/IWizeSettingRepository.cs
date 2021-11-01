using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using wize.settings.data.V1.Models;

namespace wize.settings.data.V1.Interfaces
{
    public interface IWizeSettingRepository
    {
        Task<List<WizeSetting>> GetByTypeAsync(string name);
        Task PutAsync(List<WizeSetting> model);

    }
}
