using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace AutoDistrictNameStations
{
    public class Mod : IMod
    {
        public static ILog log { get; } = LogManager.GetLogger(nameof(AutoDistrictNameStations)).SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");
            
            updateSystem.UpdateBefore<StationSystem>(SystemUpdatePhase.UIUpdate);

        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
