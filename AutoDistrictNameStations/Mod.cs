using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace AutoDistrictNameStations
{
    public class Mod : IMod
    {
        public static ILog log { get; } = LogManager.GetLogger(nameof(AutoDistrictNameStations)).SetShowsErrorsInUI(false);
        public static ModOptions modOptions{ get; set; }
        
        
        
        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            modOptions = new ModOptions();
            AssetDatabase.global.LoadSettings("Mod Options Section", modOptions, new ModOptions());
            if (modOptions.stationFormat is null || !modOptions.stationFormat.Contains("{district}"))
            {
                modOptions.stationFormat = "{district} {station}";
            }

            
            updateSystem.UpdateBefore<StationSystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
