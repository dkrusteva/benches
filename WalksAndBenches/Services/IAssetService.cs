using System.Collections.Generic;
using System.Threading.Tasks;
using WalksAndBenches.Models;

namespace WalksAndBenches.Services
{
    public interface IAssetService
    {
        Task SaveWalkAsync(WalkModel walk);

        Task<List<WalkToDisplay>> GetWalksToDisplayAsync();
    }
}
