using AutoDistrictNameStations.AuxComponents;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

namespace AutoDistrictNameStations
{
    [FileLocation("AutoDistrictNameStations")]
    [SettingsUIShowGroupName(new [] {section, transportSection, deathscareSection, hospitalSection, parkSection, policeSection, schoolSection, shelterSection, firestationSection, garbageSection, postSection, parkingSection})]
    public class ModOptions(IMod mod) : ModSetting(mod)
    {
     
        // Define sections 
        public const string section = "mainSection";
        public const string transportSection = "transportSection";
        public const string deathscareSection = "deathscareSection";
        public const string hospitalSection = "hospitalSection";
        public const string parkSection = "parkSection";
        public const string policeSection = "policeSection";
        public const string schoolSection = "schoolSection";
        public const string shelterSection = "shelterSection";
        public const string firestationSection = "firestationSection";
        public const string garbageSection = "garbageSection";
        public const string postSection = "postSection"; 
        public const string parkingSection = "parkingSection";
        // Main Section
        [SettingsUIDropdown(typeof(FormatNameOptionsProvider), nameof(FormatNameOptionsProvider.GetOptions))]
        [SettingsUISection(section)]
        public string stationFormat { get; set; } = "";
        
        // Transport stations
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(transportSection)] public bool changeStations { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(transportSection)]
        public int stationsApplyTo { get; set; } = -1;
        
        // Deaths care settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(deathscareSection)] 
        public bool changeDeathsCare { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(deathscareSection)]
        public int deathsCareApplyTo { get; set; } = -1;

        // Hospital settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(hospitalSection)]
        public bool changeHospital { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(hospitalSection)]
        public int hospitalApplyTo { get; set; } = -1;
        
        // Park settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(parkSection)]
        public bool changePark { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(parkSection)]
        public int parkApplyTo { get; set; } = -1;

        // Police settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(policeSection)]
        public bool changePolice { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(policeSection)]
        public int policeApplyTo { get; set; } = -1;

        // School settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(schoolSection)]
        public bool changeSchool { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(schoolSection)]
        public int schoolApplyTo { get; set; } = -1;

        // Shelter settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(shelterSection)]
        public bool changeShelter { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(shelterSection)]
        public int shelterApplyTo { get; set; } = -1;
        
        
        // Firestation settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(firestationSection)]
        public bool changeFirestation { get; set; }

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(firestationSection)]
        public int firestationApplayTo { get; set; } = -1;
        
        // Garbage settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(garbageSection)] 
        public bool changeGarbage { get; set; } = true;

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(garbageSection)]
        public int garbageApplyTo { get; set; } = 0;
        
        // Post settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(postSection)] 
        public bool changePost { get; set; } = true;

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(postSection)]
        public int postApplyTo { get; set; } = 0;

        // Post settings
        [SettingsUIDisplayName("Enable")]
        [SettingsUISection(parkingSection)] 
        public bool changeParking { get; set; } = true;

        [SettingsUIDisplayName("ApplyLabel")]
        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(parkingSection)]
        public int parkingApplyTo { get; set; } = 0;
        
        public override void SetDefaults()
        {
            stationFormat = "{district} {station}";
            changeStations = true;
            stationsApplyTo = 0;
            changeDeathsCare = true;
            deathsCareApplyTo = 0;
            changeHospital = true;
            hospitalApplyTo = 0;
            changePark = true;
            parkApplyTo = 0;
            changePolice = true;
            policeApplyTo = 0;
            changeSchool = true;
            schoolApplyTo = 0;
            changeShelter = true;
            shelterApplyTo = 0;
            changeFirestation = true;
            firestationApplayTo = 0;
            changeGarbage = true;
            postApplyTo = 0;
            changeParking = true;
            parkingApplyTo = 0;

        }
        
        public int GetApplyTo<T>()
        {
            if (typeof(T) == typeof(FirstDistrictStation))
            {
                return stationsApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictDeathcareFacility))
            {
                return deathsCareApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictPoliceStation))
            {
                return policeApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictSchool))
            {
                return schoolApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictShelter))
            {
                return shelterApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictPark))
            {
                return parkApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictFireStation))
            {
                return firestationApplayTo;
            }
            if (typeof(T) == typeof(FirstDistrictGarbageFacility))
            {
                return garbageApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictPostFacility))
            {
                return postApplyTo;
            }
            if (typeof(T) == typeof(FirstDistrictParkingFacility))
            {
                return parkingApplyTo;
            }
            

            return 0;
        }
        
        public bool GetChangeAllowed<T>()
        {
            log.Info(typeof(T));
            if (typeof(T) == typeof(FirstDistrictStation))
            {
                return changeStations;
            }
            if (typeof(T) == typeof(FirstDistrictDeathcareFacility))
            {
                return changeDeathsCare;
            }
            if (typeof(T) == typeof(FirstDistrictPoliceStation))
            {
                return changePolice;
            }
            if (typeof(T) == typeof(FirstDistrictSchool))
            {
                return changeSchool;
            }
            if (typeof(T) == typeof(FirstDistrictShelter))
            {
                return changeShelter;
            }
            if (typeof(T) == typeof(FirstDistrictPark))
            {
                return changePark;
            }
            if (typeof(T) == typeof(FirstDistrictFireStation))
            {
                return changeFirestation;
            }
            if (typeof(T) == typeof(FirstDistrictGarbageFacility))
            {
                return changeGarbage;
            }
            if (typeof(T) == typeof(FirstDistrictPostFacility))
            {
                return changePost;
            }
            if (typeof(T) == typeof(FirstDistrictParkingFacility))
            {
                return changeParking;
            }

            return true;
        }
    }
    public static class FormatNameOptionsProvider
    {
        public static DropdownItem<string>[] GetOptions()
        {
            return new[]
            {
                new DropdownItem<string> { value = "{district} {station}", displayName = "{district} {station}" },
                new DropdownItem<string> { value = "{district} | {station}", displayName = "{district} | {station}" },
                new DropdownItem<string> { value = "{district}'s {station}", displayName = "{district}'s {station}" }
            };
        }
    }
    
    public static class ApplyToWhatElements
    {
        public static DropdownItem<int>[] GetOptions()
        {
            return new[]
            {
                new DropdownItem<int> { value = 0, displayName = "District name then streetname" },
                new DropdownItem<int> { value = 1, displayName = "Always district name" },
                new DropdownItem<int> { value = 2, displayName = "Always street name" }
            };
        }
    }
    

}