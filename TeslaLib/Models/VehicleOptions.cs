using System;
using System.Linq;

namespace TeslaLib.Models
{
    public class VehicleOptions
    {
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

        public bool HasHpwc { get; set; }

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

            // A not yet delivered Model 3 (Seen March 31, 2018):
            // AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0

            var options = optionCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

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
                            // A Model 3 can have BT37.  Is that 70 kWh?  37 kW?
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
						    }
                            break;
                        case "WT":
                            switch (value2)
                            {
                                case "19":
                                    WheelType = WheelType.Base19;
                                    break;
                                case "21":
                                    WheelType = WheelType.Silver21;
                                    break;
                                case "SP":
                                    WheelType = WheelType.Charcoal21;
                                    break;
                                case "SG":
                                    WheelType = WheelType.CharcoalPerformance21;
                                    break;
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
                            if (superchargingOption == 5)
                                FreeSupercharging = true;
                            else if (superchargingOption == 4)
                                PayPerUseSupercharging = true;
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
                            HasTwinChargers = int.Parse(value2) > 0;
                            break;
                        case "HP":
                            HasHpwc = int.Parse(value2) > 0;
                            break;
                        case "PA":
                            HasPaintArmor = int.Parse(value2) > 0;
                            break;
                        case "PS":
                            HasParcelShelf = int.Parse(value2) > 0;
                            break;
                        case "AD":
                            // AD15 is J1772.
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
                catch(FormatException)
                {
                    Console.WriteLine($"Cannot parse option \"{option}\".  Complete options codes: {optionCodes}");
                }
            }
        }
    }
}