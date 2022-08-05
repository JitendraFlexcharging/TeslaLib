using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TeslaLib.Models;

namespace TeslaLibTests
{
    [TestClass]
    public class VehicleOptionsTests
    {
        [TestMethod]
        public void NullOptionsCodes()
        {
            // One driver has two Tesla's.  The second one hand back null for options codes.

            // Real options codes
            string optionsCodes = "AD15,AF00,APFB,APH4,AU3D,BC3B,BT35,RNG0,CDM0,CH05,COUS,DRLH,DV2W,FC01,FG30,FM3S,GLFR,HL31,HM30,ID3W,IL31,LTSB,MDL3,MR30,PMNG,PC30,RENA,RF3G,RS3H,S3PB,SA3P,SC04,STCP,SU3C,T3MA,TM00,TW00,UT3P,W38B,WR00,ZINV,MI01,PL30,SLR0,ST30,BG30,I36M,USSB,AUF2,RSF0,ILF0,FGF0,CPF0,P3WS,HP30,PT00";
            VehicleOptions options = new VehicleOptions(optionsCodes);
            GC.KeepAlive(options);

            // null options codes
            VehicleOptions nullOptions = new VehicleOptions(null);
            GC.KeepAlive(nullOptions);
        }
    }
}
