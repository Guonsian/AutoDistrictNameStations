using AutoDistrictNameStations.AuxComponents;
using AutoDistrictNameStations.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Buildings;
using Game.Common;
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

        private UpdateSystem _updateSystem;
        
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
            updateSystem.UpdateBefore<StationSystem<Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<Park, FirstDistrictPark, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<FireStation, FirstDistrictFireStation, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<PoliceStation, FirstDistrictPoliceStation, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<EmergencyShelter, FirstDistrictShelter, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<Hospital, FirstDistrictHospital, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<School, FirstDistrictSchool, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<DeathcareFacility, FirstDistrictDeathcareFacility, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<GarbageFacility, FirstDistrictGarbageFacility, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<PostFacility, FirstDistrictPostFacility, Updated>>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<GenericSystem<ParkingFacility, FirstDistrictParkingFacility, Updated>>(SystemUpdatePhase.UIUpdate);


            _updateSystem = updateSystem;
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }

        public void NameAllItems()
        {
            _updateSystem.UpdateBefore<StationSystem<object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<Park, FirstDistrictPark, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<FireStation, FirstDistrictFireStation, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<PoliceStation, FirstDistrictPoliceStation, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<EmergencyShelter, FirstDistrictShelter, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<Hospital, FirstDistrictHospital, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<School, FirstDistrictSchool, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<DeathcareFacility, FirstDistrictDeathcareFacility, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<GarbageFacility, FirstDistrictGarbageFacility, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<PostFacility, FirstDistrictPostFacility, object>>(SystemUpdatePhase.UIUpdate);
            _updateSystem.UpdateBefore<GenericSystem<ParkingFacility, FirstDistrictParkingFacility, object>>(SystemUpdatePhase.UIUpdate);
        }
    }
}
