using ETAG_ERP.Views;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETAG_ERP.Helpers
{
    // ✅ الكلاس الأساسي لتوليد بيانات التصنيفات
    public static class CategorySeeder
    {
        public static List<CategorySeedData> GetSeedData()
        {
            return new List<CategorySeedData>
            {
                new CategorySeedData("Water", "Avis", "Stainless", "", "", "WASTA"),
                new CategorySeedData("Water", "Avis", "Steel", "", "", "WASTE"),
                new CategorySeedData("Water", "Flange", "back up ring", "", "", "WFLBR"),
                new CategorySeedData("Water", "Flange", "Flange Blind", "", "", "WFLFB"),
                new CategorySeedData("Water", "Flange", "pump flange", "", "", "WFLPF"),
                new CategorySeedData("Water", "Flange", "Flange Slip", "", "", "WFLFS"),
                new CategorySeedData("Water", "Flange", "socket weld", "", "", "WFLSW"),
                new CategorySeedData("Water", "Valve", "butterfly valve", "", "", "WVBV"),
                new CategorySeedData("Water", "Valve", "carbon steel ball valve", "", "", "WVCS"),
                new CategorySeedData("Water", "Valve", "check valve", "", "", "WVCV"),
                new CategorySeedData("Water", "Valve", "gear box", "", "", "WVGB"),
                new CategorySeedData("Water", "Valve", "globe valve", "", "", "WVGV"),
                new CategorySeedData("Water", "Valve", "stainless steel ball valve", "", "", "WVSSBV"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK AL", "", "", "WFCLA"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK CAST", "", "", "WFCLC"),
                new CategorySeedData("Water", "Fitting", "CAM LOCK ST", "", "", "WFCLST"),
                new CategorySeedData("Water", "Fitting", "cast iron", "", "", "WFCI"),
                new CategorySeedData("pneumatic", "cylinder", "", "", "", "PNCY"),
                new CategorySeedData("pneumatic", "FRL", "", "", "", "PNFRL"),
                new CategorySeedData("pneumatic", "golbe valve", "", "", "", "PNGV"),
                new CategorySeedData("pneumatic", "HOSE", "", "", "", "PNHO"),
                new CategorySeedData("pneumatic", "VALVE", "", "", "", "PNVA"),
                new CategorySeedData("pneumatic", "metal", "el Bow", "", "", "PNMEB"),
                new CategorySeedData("pneumatic", "metal", "nipple", "", "", "PNMN"),
                new CategorySeedData("pneumatic", "metal", "straight", "", "", "PNMS"),
                new CategorySeedData("pneumatic", "metal", "TEE", "", "", "PNMTE"),
                new CategorySeedData("pneumatic", "Plastic", "bushing", "", "", "PNPB"),
                new CategorySeedData("pneumatic", "Plastic", "CROSS", "", "", "PNPC"),
                new CategorySeedData("pneumatic", "Plastic", "el Bow special", "", "", "PNPEBS"),
                new CategorySeedData("pneumatic", "Plastic", "Panel", "", "", "PNPA"),
                new CategorySeedData("pneumatic", "Plastic", "PLUG", "", "", "PNPL"),
                new CategorySeedData("pneumatic", "Plastic", "silensor", "", "", "PNPSI"),
                new CategorySeedData("pneumatic", "Plastic", "straight", "", "", "PNPST"),
                new CategorySeedData("pneumatic", "Plastic", "TEE", "", "", "PNPTE"),
                new CategorySeedData("pneumatic", "Plastic", "Throttle&check", "", "", "PNPTH"),
                new CategorySeedData("pneumatic", "Plastic", "y", "", "", "PNPY"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "ADAPTOR", "", "HFGAD"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSH", "", "HFGBU"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "BUSHING", "", "HFGBS"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "CROSS", "", "HFGC"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW", "", "HFGEB"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "EL BOW JIC", "", "HFGEBJ"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "NIPPLE", "", "HFGN"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "PLUG", "", "HFGPL"),
                new CategorySeedData("hydrulic", "fitting", "GERMAN", "TEE", "", "HFGTE"),
                new CategorySeedData("hydrulic", "accessories", "Air breather cap", "", "", "HAABC"),
                new CategorySeedData("hydrulic", "accessories", "flange", "", "", "HAFL"),
                new CategorySeedData("hydrulic", "accessories", "SPLIT FLANGE", "", "", "HASPFL"),
                new CategorySeedData("hydrulic", "accessories", "TEST HOSE", "", "", "HATH"),
                new CategorySeedData("hydrulic", "accessories", "TEST POINT", "", "", "HATP"),
                new CategorySeedData("hydrulic", "accessories", "VISUAL LEVEL", "", "", "HAVL"),
                new CategorySeedData("hydrulic", "filter", "FILTER ELEMENT", "", "", "HFFE"),
                new CategorySeedData("hydrulic", "filter", "FILTER", "", "", "HFFIL"),
                new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with glycerine", "", "HGPGWG"),
                new CategorySeedData("hydrulic", "GAUGE", "Pressure gauge", "with out glycerine", "", "HGPGWOG"),
                new CategorySeedData("hydrulic", "GAUGE", "temprture gauge", "", "", "HGTEG"),
                new CategorySeedData("hydrulic", "pump&motor", "motor", "", "", "HPMM"),
                new CategorySeedData("hydrulic", "pump&motor", "power steering", "", "", "HPMPS"),
                new CategorySeedData("hydrulic", "pump&motor", "pump", "", "", "HPMPU"),
                new CategorySeedData("hydrulic", "valve", "coil", "", "", "HVCOI"),
                new CategorySeedData("hydrulic", "valve", "control", "", "", "HVCON"),
                new CategorySeedData("hydrulic", "valve", "modular valve", "", "", "HVMV"),
                new CategorySeedData("hydrulic", "valve", "in line valve", "", "", "HVILV"),
                new CategorySeedData("SEAL", "dust seal", "METAL CASE", "", "", "SDSMC"),
                new CategorySeedData("SEAL", "dust seal", "NPR", "", "", "SDSNPR"),
                new CategorySeedData("SEAL", "hydraulic seal", "", "", "", "SHS"),
                new CategorySeedData("SEAL", "KGD", "", "", "", "SKGD"),
                new CategorySeedData("SEAL", "Mechanicul seal", "CONICAL", "", "", "SMESC"),
                new CategorySeedData("SEAL", "Mechanicul seal", "STRAIGHT", "", "", "SMESS"),
                new CategorySeedData("SEAL", "MPS", "", "", "", "SMPS"),
                new CategorySeedData("SEAL", "OMEGA", "KOMATSU", "", "", "SOMKO"),
                new CategorySeedData("SEAL", "OMEGA", "PISTON SEAL", "", "", "SOMPS"),
                new CategorySeedData("SEAL", "OMEGA", "ROD SEAL", "", "", "SOMRS"),
                new CategorySeedData("SEAL", "PACICING RING", "", "", "", "SPAR"),
                new CategorySeedData("SEAL", "pneumatic seal", "E4", "", "", "SPNSE4"),
                new CategorySeedData("SEAL", "pneumatic seal", "EU", "", "", "SPNSEU"),
                new CategorySeedData("SEAL", "pneumatic seal", "PP", "", "", "SPNSPP"),
                new CategorySeedData("SEAL", "RUBBER COUPLING", "", "", "", "SRUCO"),
                new CategorySeedData("SEAL", "shaft seal", "METAL CASE", "", "", "SSSMC"),
                new CategorySeedData("SEAL", "shaft seal", "NBR", "", "", "SSSNBR"),
                new CategorySeedData("SEAL", "x RING", "", "", "", "SXRNG"),
                new CategorySeedData("SEAL", "Oring", "VITON", "", "", "SORVI"),
                new CategorySeedData("SEAL", "Oring", "Silicone", "", "", "SORSI"),
                new CategorySeedData("SEAL", "Oring", "Teflon", "", "", "SORTE"),
                new CategorySeedData("SEAL", "Oring", "ARTELON", "", "", "SORAR"),
                new CategorySeedData("SEAL", "Oring", "oring rope", "", "", "SORORO"),
                new CategorySeedData("SEAL", "Oring", "ORING BOX", "", "", "SORORB"),
                new CategorySeedData("SEAL", "D RING", "", "", "", "SDRNG")
            };
        }
    }

    public class CategorySeedData
    {
        public string? Level1 { get; set; }
        public string? Level2 { get; set; }
        public string? Level3 { get; set; }
        public string? Level4 { get; set; }
        public string? Level5 { get; set; }
        public string? Code { get; set; }

        public CategorySeedData(string? level1, string? level2, string? level3, string? level4, string? level5, string? code)
        {
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
            Level4 = level4;
            Level5 = level5;
            Code = code;
        }
    }
}
