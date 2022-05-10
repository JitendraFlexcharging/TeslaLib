using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace TeslaLib.Models
{
    public class VehicleOptions
    {
        public VehicleOptions(string optionCodes)
        {
            ParseOptionCodes(optionCodes);
            RawOptionCodes = optionCodes;
        }

        public String RawOptionCodes { get; private set; }

        public RoofType RoofType { get; set; }

        public Region Region { get; set; }

        // Tesla doesn't return the year for this car explicitly.  They do seem to have an offset from the first year of production. (that's a guess)
        public int ModelRefreshNumber { get; set; }

        // Were using MDLS, MDLX, and MDL3, but now Model S's return identical options codes to Model 3's sometimes.  Use the VehicleConfig for this purpose.
        //public Model Model { get; set; }

        public TrimLevel TrimLevel { get; set; }

        public DriverSide DriverSide { get; set; }

        public bool IsPerformance { get; set; }

        /// <summary>
        /// Do not trust this value, because Tesla does confusing things with older Model S's starting around August 2019.  They show up returning
        /// the same goofy BT37 code as Model 3's, and they don't tell us in options that this is a Model S.  Need a secondary lookup table.
        /// </summary>
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

        public bool HasHpwc { get; set; }

        public bool HasPaintArmor { get; set; }

        public bool HasParcelShelf { get; set; }

        // 02 = NEMA 14-50
        public int AdapterTypeOrdered { get; set; }

        public bool IsPerformancePlus { get; set; }

        public bool HasLudicrous { get; set; }

        public bool HasCHAdeMOAdapter { get; set; }

        public bool AllWheelDrive { get; set; }

        public bool HasHEPAFilter { get; set; }

        public int? BatteryFirmwareLimit { get; set; }

        // Maximum amperage of the charger(s).  Some cars have dual chargers (2x40 amp chargers), or various chargers 
        // limited to 72 amps or 48 amps, etc.  Model 3 Standard Range is limited to 32 Amps AC.
        public int? ChargerLimit { get; set; }

        // Options codes change over time, with new ones showing up.  Also, there _could_ be country-specific codes.  
        // Here's a site that may help keep up with them: http://options.teslastuff.net/
        public void ParseOptionCodes(string optionCodes)
        {
            // MS01,RENA,TM00,DRLH,PF00,BT85,PBCW,RFPO,WT19,IBMB,IDPB,TR00,SU01,SC01,TP01,AU01,CH00,HP00,PA00,PS00,AD02,X020,X025,X001,X003,X007,X011,X013

            // Another car returned this as of December 2017:
            // MDLS,RENA,AF02,AU01,BC0B,BP00,BR00,BS00,BTX4,CDM0,CH05,PMSS,CW00,DA02,DCF0,DRLH,DSH7,DV4W,FG02,HP00,IDCF,IX01,LP01,ME02,MI01,PA00,PF00,PI01,PK00,PS01,PX00,QNEB,RFP2,SC01,SP00,SR01,ST01,SU00,TM00,TP03,TR00,UTAB,WTAS,WTX1,X001,X003,X007,X011,X013,X021,X025,X027,X028,X031,X037,X040,X044,YFFC,COUS

            // Another Model S as of January 2018.
            // MS04,RENA,AU01,BC0B,BP00,BR01,BS00,BTX4,CDM0,CH00,PPSB,CW00,DA02,DCF0,DRLH,DSH7,DV4W,FG02,HP00,IDHM,IX01,LP01,ME02,MI00,PA00,PF00,PI01,PK00,PS01,PX00,QNET,RFP2,SC01,SP00,SR01,SU01,TM00,TP03,TR00,UTAW,WT19,WTX1,X001,X003,X007,X011,X013,X021,X025,X027,X028,X031,X037,X040,YF00,COUS

            // A not yet delivered Model 3 (Seen March 31, 2018):
            // AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0

            // As of January 2019, an older Model S P85 returned:
            // MS03,RENA,AD02,AF00,AU00,BC0R,BS00,CH01,PPMR,CW00,DRLH,FG02,HP00,IDPB,IX01,LP01,PA00,PBT85,PF01,PK01,PS01,PX00,QYMB,RFPO,SC01,SP00,SR01,SU01,TM00,TP01,TR00,UTAW,WT1P,WTX1,X001,X003,X007,X011,X014,X019,X024,X027,X028,X031,X037,YF00,COUS

            // Model 3, built in November 2018 as of 5/13/2019.  SC04 = Pay for supercharging.
            // AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0

            // Model X
            // RENA,AD15,AF02,AH00,APF2,APH3,APPB,AU01,BC0R,BP01,BR00,BS00,BTX6,CC04,CDM0,CH04,PMNG,COUS,CW02,DRLH,DSHG,DU01,DV4W,FG02,FMP6,FR01,GLFR,HC00,HP00,IDBO,INBDS,IX00,LLP1,LP01,LT6P,ME02,MI03,PF01,PI01,PK00,PX6D,QLBS,RCX0,RFPX,S07P,SC04,SP00,SR06,ST01,SU01,TIC4,TM00,TR01,TRA1,TW01,UM01,USSB,UTSB,WTSC,X001,X003,X007,X011,X013,X021,X024,X026,X028,X031,X037,X040,X042,X043,YFFC,MDLX

            // A 2014 Model S P85 started returning the same values as a Model 3 Performance around August 2019:
            // AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0

            // In August 2019, a Model S started returning that it was a Model 3!  Here's the inconsistent, incorrectly reported Model S.
            // AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0

            // From Tim Dorr's API documentation site:
            // As of August 2019, Option Codes cannot be relied on. Vehicles now return a generic set of codes related to a Model 3.

            var options = optionCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string option in options)
            {
                try
                {
                    switch (option)
                    {
                        case "X001":
                            HasPowerLiftgate = true;
                            continue;
                        case "X002":
                            HasPowerLiftgate = false;
                            continue;
                        case "X003":
                            HasNavigation = true;
                            continue;
                        case "X004":
                            HasNavigation = false;
                            continue;
                        case "X007":
                            HasPremiumExteriorLighting = true;
                            continue;
                        case "X008":
                            HasPremiumExteriorLighting = false;
                            continue;
                        case "X011":
                            HasHomeLink = true;
                            continue;
                        case "X012":
                            HasHomeLink = false;
                            continue;
                        case "X013":
                            HasSatelliteRadio = true;
                            continue;
                        case "X014":
                            HasSatelliteRadio = false;
                            continue;
                        case "X019":
                            // Tim Dorr's documentation says this is whether we have a carbon fiber spoiler.
                            // There may be (or have been) an SLR1 option for that too...
                            HasPerformanceExterior = true;
                            continue;
                        case "X020":
                            HasPerformanceExterior = false;
                            continue;
                        case "X024":
                            HasPerformancePowertrain = true;
                            continue;
                        case "X025":
                            HasPerformancePowertrain = false;
                            continue;
                        case "X041":
                            // No Auto Presenting Door (probably Model X only)
                            continue;
                        case "X042":
                            // Has Auto Presenting Door (probably Model X only)
                            continue;
                        case "DV2W":  // 2 wheel drive
                            AllWheelDrive = false;
                            continue;
                        case "DV4W":  // 4 wheel drive
                            AllWheelDrive = true;
                            continue;
                        case "MDLX":
                            //Model = Model.X;
                            continue;
                        case "MDLS":
                            //Model = Model.S;
                            continue;
                        case "MDL3":  // Problem - my Model S thinks it's a Model 3.  Determine car type through other means.
                            //Model = Model.Three;
                            continue;
                        case "MDLY":
                            //Model = Model.Y;
                            break;
                        case "PX6D":  // Zero to 60 in 2.5 sec
                            continue;
                        case "P85D":  // Model S?  Skip for now.
                            continue;
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
                                TeslaClient.Logger.WriteLine($"Unrecognized option {option}.  A new CHAdeMO adapter type?");
                            break;

                        case "TRA":
                            // I've seen TRA1 from a Model X.  Could be a third row seating option.  Might be TR + A1,
                            // maybe.
                            break;

                        case "PBT":
                            {
                                // An older Model S started returning PBT85 as of early 2019, instead of BT.
                                string performanceBatteryPackSize = option.Substring(3);
                                BatterySize = Int32.Parse(performanceBatteryPackSize);
                            }
                            break;

                        case "LT6":
                            // Known values are LT6W for white base lower trim, and LT6P (?)
                            break;

                        case "RCX":
                            // Rear console - RCX0 for no rear console, and RCX1 for having one.
                            break;

                        case "SPT":
                            // Supposed Model 3 performance option code.  Brian's Model 3 Performance doesn't have it.
                            if (option == "SPT31")
                                IsPerformance = true;
                            break;
                    }

                    string value2 = option.Substring(2, 2);

                    switch (option.Substring(0, 2))
                    {
                        case "MS":
                            // For a Model S, we may see MS03, meaning 2014.  This is a guess though.  There is no MI00 type code with Brian's 2014 Model S.
                            ModelRefreshNumber = Int32.Parse(value2);
                            break;
                        case "MI":
                            // MI seems to be some model update offset from the introduction of that model year.
                            // People have mapped MI00 to 2015 production refresh for a Model S.  For Brian's Model 3 from 2018, I've got MI00 too.
                            // So let's not interpret this until after we've set the model type.
                            /* Tim Dorr documentation, which may not be complete or correct:
                             * MI00	2015 Production Refresh	
                               MI00	Project/Program Code M3	Base Manufacturing Intro Code
                               MI01	2016 Production Refresh	
                               MI02	2017 Production Refresh	
                               MI03	201? Production Refresh	Found on Model X ordered 11/2018 delivered 3/2019
                            */
                            ModelRefreshNumber = Int32.Parse(value2);
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
                            // A Model 3 can have BT37.  Performance Model S's also can return values like PBT85, not BT85.
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

                                    case '7':
                                        BatterySize = 75;
                                        break;

                                    case '8':
                                        BatterySize = 85;
                                        break;

                                    case '9':
                                        BatterySize = 100;
                                        break;

                                    case 'A':
                                        BatterySize = 100;  // 2020 Model X battery
                                        break;

                                    case 'B':
                                        BatterySize = 100;  // Model S Plaid 2021
                                        break;

                                    default:
                                        Console.Error.WriteLine($"Cannot parse Tesla battery type option \"{option}\".");
                                        break;
                                }
                            }
                            else if (value2 == "37")
                            {
                                // Model 3 battery is sometimes listed as BT37.
                                BatterySize = 75;
                            }
                            else if (value2 == "38")
                            {
                                // Model 3 LR built in China.  Not sure about the capacity.
                                BatterySize = 75;
                            }
                            else if (value2 == "42")
                            {
                                // M3P (and latest LR 940xxx+) 2021 model year
                                BatterySize = 82;
                            }
                            else if (value2 == "F0")
                            {
                                BatterySize = 55;  // Made in China CATL Iron Phosphate battery, Model 3 Standard+
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
                            switch (value2)
                            {
                                case "BC":
                                    RoofType = RoofType.Colored;
                                    break;
                                case "PO":
                                    RoofType = RoofType.None;
                                    break;
                                case "BK":
                                    RoofType = RoofType.Black;
                                    break;
                                case "FG":
                                case "3G":
                                    RoofType = RoofType.Glass;
                                    break;
                                case "P0":
                                    RoofType = RoofType.AllGlassPanoramic;
                                    break;
                                case "P2":
                                    RoofType = RoofType.Sunroof;
                                    break;
                                case "PX":
                                    RoofType = RoofType.ModelX;
                                    break;
                                case "FR":
                                    // Fixed glass roof, roof-rack compat, Model S 2020 onwards
                                    RoofType = RoofType.FixedGlassRoof;
                                    break;
                                default:
                                    TeslaClient.Logger.WriteLine("Unrecognized roof type: " + option);
                                    break;
                            }
                            break;

                        // Note: WT (WheelType) doesn't seem to be returned as of Sept 2019.  Moved to VehicleConfig type, basically.

                        case "ID":
                            InteriorDecor = Extensions.ToEnum<InteriorDecor>(value2);
                            continue;
                        case "IL":
                            // Interior lighting - not used currently
                            continue;
                        case "TR":
                            // Tolerate "TRA1"
                            int seatConfiguration = 0;
                            if (Int32.TryParse(value2, out seatConfiguration))
                                HasThirdRowSeating = seatConfiguration > 0;
                            break;
                        case "SU":
                            // Smart air suspension is standard on Model X, at least as of 2018.
                            // Model 3 has a SU3C option code.
                            int hasAirSuspensionNumber = 0;
                            if (Int32.TryParse(value2, out hasAirSuspensionNumber))
                                HasAirSuspension = hasAirSuspensionNumber > 0;
                            else if (value2 == "3C")
                                HasAirSuspension = true;
                            break;
                        case "SC":
                            int superchargingOption = int.Parse(value2);
                            HasSuperCharging = superchargingOption > 0;  // Note: One web site claims SC00 may mean you have supercharging...
                            if (superchargingOption == 1 || superchargingOption == 5)
                                FreeSupercharging = true;
                            else if (superchargingOption == 4)
                                PayPerUseSupercharging = true;
                            else if (superchargingOption == 6)
                                FreeSupercharging = true;  // Temporarily enabled, may be disabled soon.
                            break;
                        case "TP":
                            HasTechPackage = int.Parse(value2) > 0;
                            break;
                        case "AU":
                            // Model 3 has an audio code AU3P
                            int audioUpgrade = 0;
                            if (Int32.TryParse(value2, out audioUpgrade))
                                HasAudioUpgrade = audioUpgrade > 0;
                            else if (value2 == "3P")
                            {
                                HasAudioUpgrade = true;
                            }
                            break;
                        case "CH":
                            switch (value2)
                            {
                                case "00":
                                    ChargerLimit = 40;
                                    break;
                                case "01":
                                    ChargerLimit = 80;
                                    HasTwinChargers = true;
                                    break;
                                case "04":
                                    ChargerLimit = 72;  // Model S/X
                                    break;
                                case "05":
                                    ChargerLimit = 48;  // Model S/X
                                    break;
                                case "06":
                                    ChargerLimit = 32;  // Guess.  Not documented.
                                    break;
                                case "07":
                                    ChargerLimit = 48;  // Model 3
                                    break;
                                case "09":
                                    ChargerLimit = 48;  // EU Model S charging system.  Guessing on limit though!
                                    break;
                                case "11":
                                    ChargerLimit = 48;  // Unknown - complete guess.
                                    break;
                                case "12":
                                    ChargerLimit = 48;  // 48 Amp Combo 1 gen 3.5 charger
                                    break;
                                case "14":
                                    ChargerLimit = 40;  // This isn't documented but is showing up.  Guessing at 40.
                                    break;
                                case "15":
                                    ChargerLimit = 48;
                                    break;
                                case "16":
                                    ChargerLimit = 48;  // This isn't documented but is showing up.  Complete guess
                                    break;
                                default:
                                    Console.Error.WriteLine($"Unrecognized Tesla charger limit type.  Vehicle Option {option}");
                                    try
                                    {
                                        // Don't try writing this file on a phone.
                                        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                                            File.AppendAllText("c:\\TeslaOptionCodes.txt", "Unrecognized Tesla charger limit value: " + value2 + "\r\n");
                                    }
                                    catch { }
                                    break;
                            }
                            break;
                        case "HP":
                            HasHpwc = int.Parse(value2) > 0;
                            break;
                        case "PA":
                            HasPaintArmor = int.Parse(value2) > 0;
                            break;
                        case "PS":
                            HasParcelShelf = int.Parse(value2) > 0;
                            continue;
                        case "AD":
                            // AD15 is J1772.
                            break;
                        case "PX":
                            // PX6D handled above.  (zero to 60 in 2.5 sec)
                            IsPerformancePlus = int.Parse(value2) > 0;
                            break;
                        case "BP":
                            if (value2 == "00")
                                HasLudicrous = false;
                            else if (value2 == "01")
                                HasLudicrous = true;
                            else if (value2 == "02")
                            {
                                // "Uncorked" acceleration (non-performance)
                            }
                            else
                                Console.Error.WriteLine($"Unrecognized Tesla option {option}.  Ludicrous-like speed option?");
                            break;
                        case "AF":
                            if (value2 == "02")
                                HasHEPAFilter = true;
                            else if (value2 != "00")
                                Console.Error.WriteLine($"Unrecognized HEPA filter related option {option}");
                            break;
                        case "BR":
                            if (value2 != "00")
                            {
                                // BR01 means a range upgrade was applied.  Notes on forums suggest this was vehicles 
                                // that were software limited but now generally aren't limited, and may be able to
                                // use the full 90 kWh in their batteries.  Not sure how long this option was in effect,
                                // but seems like almost 25% of my first year vehicles had this set.
                                if (value2 == "01")
                                    break;

                                if (value2 == "03")
                                    BatteryFirmwareLimit = 60;
                                else if (value2 == "05")
                                    BatteryFirmwareLimit = 75;
                                else
                                    Console.Error.WriteLine($"Unrecognized Battery Firmware limit related option {option}");
                            }
                            break;
                        case "CO": // Country code.  Not required nor currently exposed, but nice to know this option exists.
                            switch (value2)
                            {
                                case "US":  // United States
                                case "AT":  // Austria
                                case "AU":  // Australia
                                case "BE":  // Belgium
                                case "CH":  // Switzerland
                                case "DE":  // Germany
                                case "DK":  // Denmark
                                case "ES":  // Spain
                                case "FI":  // Finland
                                case "FR":  // France
                                case "GB":  // Great Britain
                                case "HR":  // Croatia
                                case "IE":  // Ireland
                                case "IT":  // Italy
                                case "KR":  // South Korea
                                case "LU":  // Luxembourg
                                case "NL":  // Netherlands
                                case "NO":  // Norway
                                case "PT":  // Portugal
                                case "SE":  // Sweden
                                    break;

                                default:
                                    Console.Error.WriteLine($"Unrecognized Tesla country code in vehicle options \"{value2}\".");
                                    break;
                            }
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
                catch (FormatException)
                {
                    TeslaClient.Logger.WriteLine($"Cannot parse Tesla option \"{option}\".  Complete options codes: {optionCodes}");
                }
            }
        }
    }
}
 