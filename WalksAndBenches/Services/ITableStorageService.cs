using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalksAndBenches.Models;

namespace WalksAndBenches.Services
{
    public interface ITableStorageService
    {
        Task SaveBench(WalkToSave entityToSave);
    }
}
