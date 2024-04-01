using AutoDistrictNameStations.AuxComponents;
using AutoDistrictNameStations.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Game;
using Game.Buildings;
using Game.Modding;
using Game.SceneFlow;
using Game.UI;
using Unity.Entities;

namespace AutoDistrictNameStations
{
    public class Mod : IMod
    {
        public static ILog log { get; } = LogManager.GetLogger(nameof(AutoDistrictNameStations)).SetShowsErrorsInUI(false);
        public static ModOptions ModCustomOptions{ get; set; }
        public static NameSystem GameNameSystem { get; private set; }
        
        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            ModCustomOptions = new ModOptions(this);
            ModCustomOptions.RegisterInOptionsUI();
            
            AssetDatabase.global.LoadSettings("AutoDistrictNameSettings", ModCustomOptions, new ModOptions(this));

            if (ModCustomOptions.stationsApplyTo == -1)
            {
                ModCustomOptions.SetDefaults();
            }
            
            if(ModCustomOptions.stationFormat != null && !ModCustomOptions.stationFormat.Contains("{district}"))
            {   
                ModCustomOptions.stationFormat = "{district} {station}";
            }
            
            GameNameSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Game.UI.NameSystem>();
            
            // In UI.Update as we want to use the Game.UI.NameSystem 
            updateSystem.UpdateBefore<StationSystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<Park, FirstDistrictPark>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<FireStation, FirstDistrictFireStation>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<PoliceStation, FirstDistrictPoliceStation>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<EmergencyShelter, FirstDistrictShelter>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<Hospital, FirstDistrictHospital>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<School, FirstDistrictSchool>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<DeathcareFacility, FirstDistrictDeathcareFacility>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<GarbageFacility, FirstDistrictGarbageFacility>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<PostFacility, FirstDistrictPostFacility>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<ParkingFacility, FirstDistrictParkingFacility>>(SystemUpdatePhase.UIUpdate);

        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
