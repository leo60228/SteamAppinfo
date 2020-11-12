using System.Threading.Tasks;
using SteamKit2;

namespace SteamAppinfo {
    public interface IGetProductInfo {
        Task<SteamApps.PICSProductInfoCallback.PICSProductInfo> ProductInfo(uint app);
    }
}
