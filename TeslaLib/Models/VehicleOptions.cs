using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TeslaLib.Models
{
    public class VehicleOptions
    {

        public VehicleOptions()
        {

        }

        public VehicleOptions(string optionCodes)
        {
            ParseOptionCodes(optionCodes);
        }

        public RoofType RoofType { get; set; }

        public Region Region { get; set; }

        public int YearModel { get; set; }

        public Model Model { get; set; }

        public TrimLevel TrimLevel { get; set; }

        public DriverSide DriverSide { get; set; }

        public bool IsPerformance { get; set; }

        public int BatterySize { get; set; }

        public TeslaColor Color { get; set; }

        public WheelType WheelType { get; set; }

        public InteriorDecor InteriorDecor { get; set; }

        public bool HasPowerLiftgate { get; set; }

        public bool HasNavigation { get; set; }

        public bool HasPremiumExteriorLighting { get; set; }

        public bool HasHomeLink { get; set; }

        public bool HasSatelliteRadio { get; set; }
        
        public bool HasPerformanceExterior { get; set; }
        
        public bool HasPerformancePowertrain { get; set; }

        public bool HasThirdRowSeating { get; set; }

        public bool HasAirSuspension { get; set; }

        public bool HasSuperCharging { get; set; }

        public bool FreeSupercharging { get; set; }

        public bool PayPerUseSupercharging { get; set; }

        public bool HasTechPackage { get; set; }

        public bool HasAudioUpgrade { get; set; }

        // Note: Around 2016 Tesla replaced their twin chargers option with one 72 kW charger.
        public bool HasTwinChargers { get; set; }

        public bool HasHPWC { get; set; }

        public bool HasPaintArmor { get; set; }

        public bool HasParcelShelf { get; set; }

        // 02 = NEMA 14-50
        public int AdapterTypeOrdered { get; set; }

        public bool IsPerformancePlus { get; set; }

        public bool HasLudicrous { get; set; }

        public bool HasCHAdeMOAdapter { get; set; }

        public bool AllWheelDrive { get; set; }

        // Options codes change over time, with new ones showing up.  Also, there _could_ be country-specific codes.  
        // Here's a site that may help keep up with them: http://options.teslastuff.net/
        public void ParseOptionCodes(string optionCodes)
        {
            // MS01,RENA,TM00,DRLH,PF00,BT85,PBCW,RFPO,WT19,IBMB,IDPB,TR00,SU01,SC01,TP01,AU01,CH00,HP00,PA00,PS00,AD02,X020,X025,X001,X003,X007,X011,X013

            // Another car returned this as of December 2017:
            // MDLS,RENA,AF02,AU01,BC0B,BP00,BR00,BS00,BTX4,CDM0,CH05,PMSS,CW00,DA02,DCF0,DRLH,DSH7,DV4W,FG02,HP00,IDCF,IX01,LP01,ME02,MI01,PA00,PF00,PI01,PK00,PS01,PX00,QNEB,RFP2,SC01,SP00,SR01,ST01,SU00,TM00,TP03,TR00,UTAB,WTAS,WTX1,X001,X003,X007,X011,X013,X021,X025,X027,X028,X031,X037,X040,X044,YFFC,COUS

            // Another Model S as of January 2018.
            // MS04,RENA,AU01,BC0B,BP00,BR01,BS00,BTX4,CDM0,CH00,PPSB,CW00,DA02,DCF0,DRLH,DSH7,DV4W,FG02,HP00,IDHM,IX01,LP01,ME02,MI00,PA00,PF00,PI01,PK00,PS01,PX00,QNET,RFP2,SC01,SP00,SR01,SU01,TM00,TP03,TR00,UTAW,WT19,WTX1,X001,X003,X007,X011,X013,X021,X025,X027,X028,X031,X037,X040,YF00,COUS

            List<string> options = optionCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string option in options)
            {
                try
                {
                    switch (option)
                    {
                        case "X001":
                            HasPowerLiftgate = true;
                            break;
                        case "X002":
                            HasPowerLiftgate = false;
                            break;
                        case "X003":
                            HasNavigation = true;
                            break;
                        case "X004":
                            HasNavigation = false;
                            break;
                        case "X007":
                            HasPremiumExteriorLighting = true;
                            break;
                        case "X008":
                            HasPremiumExteriorLighting = false;
                            break;
                        case "X011":
                            HasHomeLink = true;
                            break;
                        case "X012":
                            HasHomeLink = false;
                            break;
                        case "X013":
                            HasSatelliteRadio = true;
                            break;
                        case "X014":
                            HasSatelliteRadio = false;
                            break;
                        case "X019":
                            HasPerformanceExterior = true;
                            break;
                        case "X020":
                            HasPerformanceExterior = false;
                            break;
                        case "X024":
                            HasPerformancePowertrain = true;
                            break;
                        case "X025":
                            HasPerformancePowertrain = false;
                            break;
                        case "DV4W":
                            AllWheelDrive = true;
                            break;
                        case "MDLX":
                            Model = Model.X;
                            break;
                        case "MDLS":
                            Model = Model.S;
                            break;
                        case "MDL3":  // I don't know this is the right option code, but it seems probable.
                            Model = Model.Three;
                            break;
                    }


                    string value1 = option.Substring(3, 1);
                    switch (option.Substring(0, 3))
                    {
                        case "CDM":
                            if (value1 == "0")
                                HasCHAdeMOAdapter = false;
                            else if (value1 == "1")
                                HasCHAdeMOAdapter = true;
                            else
                                Console.Error.WriteLine($"Unrecognized option {option}.  A new CHAdeMO adapter type?");
                            break;

                        case "TRA":
                            // I've seen TRA1 from a Model X.  Could be a third row seating option.  Might be TR + A1,
                            // maybe.
                            break;
                    }

                    string value2 = option.Substring(2, 2);

                    switch (option.Substring(0, 2))
                    {
                        case "MS":
                            YearModel = int.Parse(value2);
                            Model = Model.S;
                            break;
                        case "RE":
                            Region = Extensions.ToEnum<Region>(value2);
                            break;
                        case "TM":
                            TrimLevel = Extensions.ToEnum<TrimLevel>(value2);
                            break;
                        case "DR":
                            DriverSide = Extensions.ToEnum<DriverSide>(value2);
                            break;
                        case "PF":
                            IsPerformance = int.Parse(value2) > 0;
                            break;
                        case "BT":
                            if (value2[0] == 'X')
                            {
                                switch (value2[1])
                                {
                                    case '4':
                                        BatterySize = 90;  // 90 kWh
                                        break;

                                    case '5':
                                        BatterySize = 75;  // 75 kWh
                                        break;

                                    case '6':
                                        BatterySize = 100; // 100 kWh
                                        break;

                                    default:
                                        Console.Error.WriteLine($"Cannot parse battery type option \"{option}\".");
                                        break;
                                }
                            }
                            else
                            {
                                int batterySize = 0;
                                if (Int32.TryParse(value2, out batterySize))
                                    BatterySize = batterySize;
                                else
                                    Console.Error.WriteLine($"Cannot parse battery type option \"{option}\".  Complete option codes: {optionCodes}");
                            }
                            break;
                        case "RF":
                            if (value2 == "BC")
                            {
                                RoofType = Models.RoofType.COLORED;
                            }
                            else if (value2 == "PO")
                            {
                                RoofType = Models.RoofType.NONE;
                            }
                            else if (value2 == "BK")
                            {
                                RoofType = Models.RoofType.BLACK;
                            }
                            break;
                        case "WT":
                            if (value2 == "19")
                            {
                                WheelType = Models.WheelType.BASE_19;
                            }
                            else if (value2 == "21")
                            {
                                WheelType = Models.WheelType.SILVER_21;
                            }
                            else if (value2 == "SP")
                            {
                                WheelType = Models.WheelType.CHARCOAL_21;
                            }
                            else if (value2 == "SG")
                            {
                                WheelType = Models.WheelType.CHARCOAL_PERFORMANCE_21;
                            }
                            break;
                        case "ID":
                            InteriorDecor = Extensions.ToEnum<InteriorDecor>(value2);
                            break;
                        case "TR":
                            // Tolerate "TRA1"
                            int seatConfiguration = 0;
                            if (Int32.TryParse(value2, out seatConfiguration))
                                HasThirdRowSeating = seatConfiguration > 0;
                            break;
                        case "SU":
                            HasAirSuspension = int.Parse(value2) > 0;
                            break;
                        case "SC":
                            int superchargingOption = int.Parse(value2);
                            HasSuperCharging = superchargingOption > 0;
                            if (superchargingOption == 5)
                                FreeSupercharging = true;
                            else if (superchargingOption == 4)
                                PayPerUseSupercharging = true;
                            break;
                        case "TP":
                            HasTechPackage = int.Parse(value2) > 0;
                            break;
                        case "AU":
                            HasAudioUpgrade = int.Parse(value2) > 0;
                            break;
                        case "CH":
                            HasTwinChargers = int.Parse(value2) > 0;
                            break;
                        case "HP":
                            HasHPWC = int.Parse(value2) > 0;
                            break;
                        case "PA":
                            HasPaintArmor = int.Parse(value2) > 0;
                            break;
                        case "PS":
                            HasParcelShelf = int.Parse(value2) > 0;
                            break;
                        case "AD":
                            break;
                        case "PX":
                            IsPerformancePlus = int.Parse(value2) > 0;
                            break;
                        case "BP":
                            if (value2 == "00")
                                HasLudicrous = false;
                            else if (value2 == "01")
                                HasLudicrous = true;
                            else
                                Console.Error.WriteLine($"Unrecognized option {option}.  Ludicrous-like speed option?");
                            break;
                    }

                    string value3 = option.Substring(1, 3);
                    switch (option.Substring(0, 1))
                    {
                        case "P":
                            Color = Extensions.ToEnum<TeslaColor>(value3);
                            break;
                        case "I":
                            break;
                    }
                }
                catch(FormatException e)
                {
                    Console.WriteLine($"Cannot parse option \"{option}\"");
                }
            }
        }
    }

    public enum Region
    {
        [EnumMember(Value = "NA")]
        USA,

        [EnumMember(Value = "NC")]
        CANADA
    }

    public enum TrimLevel
    {
        [EnumMember(Value = "00")]
        STANDARD,
        
        //[EnumMember(Value = "01")]
        //PERFORMANCE,

        [EnumMember(Value = "02")]
        SIGNATURE_PERFORMANCE
    }

    // Model S paint codes:  https://teslamotorsclub.com/tmc/threads/model-s-paint-codes.25297/
    public enum TeslaColor
    {
        [EnumMember(Value = "BSB")]
        BLACK,

        [EnumMember(Value = "BCW")]
        WHITE,

        [EnumMember(Value = "MSS")]
        SILVER,  // Metallic silver

        [EnumMember(Value = "MTG")]
        METALLIC_DOLPHIN_GREY,

        [EnumMember(Value = "MAB")]
        METALLIC_BROWN,

        [EnumMember(Value = "MBL")]
        METALLIC_BLACK,  // Obsidian Black

        [EnumMember(Value = "MMB")]
        METALLIC_BLUE,

        [EnumMember(Value = "MNG")]
        STEEL_GREY,

        [EnumMember(Value = "MSG")]
        METALLIC_GREEN,

        [EnumMember(Value = "PSW")]
        PEARL_WHITE,
        
        [EnumMember(Value = "PMR")]
        MULTICOAT_RED,
        //Red = MULTICOAT_RED,

        // Not clear whether this exists.
        //[EnumMember(Value = "MMR")]
        //MULTICOAT_RED_2,

        [EnumMember(Value = "PSB")]
        DEEP_BLUE_METALLIC,  // Originally called Ocean Blue

        [EnumMember(Value = "PSR")]
        SIGNATURE_RED,  // "Sunset Red"

        [EnumMember(Value = "PTI")]
        TITANIUM,  // Titanium metallic
    }

    public enum InteriorDecor
    {
        [EnumMember(Value = "CF")]
        CARBON_FIBER,

        [EnumMember(Value = "LW")]
        LACEWOOD,

        [EnumMember(Value = "OM")]
        OBECHE_WOOD_MATTE,

        [EnumMember(Value = "OG")]
        OBECHE_WOOD_GLOSS,

        [EnumMember(Value = "PB")]
        PIANO_BLACK,
    }

    public enum DriverSide
    {

        [EnumMember(Value = "LH")]
        LEFT_HAND_DRIVE,

        [EnumMember(Value = "RH")]
        RIGHT_HAND_DRIVE,
    }

    public enum Model
    {
        Unknown,
        S,
        X,
        Three
    }
}