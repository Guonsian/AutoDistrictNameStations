using AutoDistrictNameStations.AuxComponents;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

namespace AutoDistrictNameStations
{
    [FileLocation("AutoDistrictNameStations")]
    [SettingsUIShowGroupName(new [] {section, transportSection, deathscareSection, hospitalSection, parkSection, policeSection, schoolSection, shelterSection, firestationSection})]
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
        
        // Main Section
        [SettingsUIDropdown(typeof(FormatNameOptionsProvider), nameof(FormatNameOptionsProvider.GetOptions))]
        [SettingsUISection(section)]
        public string stationFormat { get; set; } = "";
        
        // Transport stations
        [SettingsUISection(transportSection)] public bool changeStations { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(transportSection)]
        public int stationsApplyTo { get; set; } = -1;
        
        // Deaths care settings
        [SettingsUISection(deathscareSection)] public bool changeDeathsCare { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(deathscareSection)]
        public int deathsCareApplyTo { get; set; } = -1;

        // Hospital settings
        [SettingsUISection(hospitalSection)]
        public bool changeHospital { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(hospitalSection)]
        public int hospitalApplyTo { get; set; } = -1;
        
        // Park settings
        [SettingsUISection(parkSection)]
        public bool changePark { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(parkSection)]
        public int parkApplyTo { get; set; } = -1;

        // Police settings
        [SettingsUISection(policeSection)]
        public bool changePolice { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(policeSection)]
        public int policeApplyTo { get; set; } = -1;

        // School settings
        [SettingsUISection(schoolSection)]
        public bool changeSchool { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(schoolSection)]
        public int schoolApplyTo { get; set; } = -1;

        // Shelter settings
        [SettingsUISection(shelterSection)]
        public bool changeShelter { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(shelterSection)]
        public int shelterApplyTo { get; set; } = -1;
        
        
        // Firestation settings
        [SettingsUISection(firestationSection)]
        public bool changeFirestation { get; set; }

        [SettingsUIDropdown(typeof(ApplyToWhatElements), nameof(ApplyToWhatElements.GetOptions))]
        [SettingsUISection(firestationSection)]
        public int firestationApplayTo { get; set; } = -1;
        
        
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