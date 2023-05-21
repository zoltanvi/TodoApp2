using System.Collections.Generic;
using TodoApp2.Core.Constants;

namespace TodoApp2.Core
{
    public class ColorListProvider
    {
        public List<string> Items { get; }

        public ColorListProvider()
        {
            Items = new List<string>
            {
                GlobalConstants.ColorName.Transparent,
                "#FFFFFF",
                "#F5F5F5",
                "#E0E0E0",
                "#BDBDBD",
                "#757575",
                "#424242",
                "#212121",
                "#000000",

                //"#FFEBEE",
                "#FFCDD2",
                "#EF9A9A",
                "#E57373",
                "#EF5350",
                "#F44336",
                "#E53935",
                "#D32F2F",
                "#C62828",
                "#B71C1C",

                //"#FCE4EC",
                "#F8BBD0",
                "#F48FB1",
                "#F06292",
                "#EC407A",
                "#E91E63",
                "#D81B60",
                "#C2185B",
                "#AD1457",
                "#880E4F",

                //"#F3E5F5",
                "#E1BEE7",
                "#CE93D8",
                "#BA68C8",
                "#AB47BC",
                "#9C27B0",
                "#8E24AA",
                "#7B1FA2",
                "#6A1B9A",
                "#4A148C",

                //"#EDE7F6",
                "#D1C4E9",
                "#B39DDB",
                "#9575CD",
                "#7E57C2",
                "#673AB7",
                "#5E35B1",
                "#512DA8",
                "#4527A0",
                "#311B92",

                //"#E8EAF6",
                "#C5CAE9",
                "#9FA8DA",
                "#7986CB",
                "#5C6BC0",
                "#3F51B5",
                "#3949AB",
                "#303F9F",
                "#283593",
                "#1A237E",

                //"#E3F2FD",
                "#BBDEFB",
                "#90CAF9",
                "#64B5F6",
                "#42A5F5",
                "#2196F3",
                "#1E88E5",
                "#1976D2",
                "#1565C0",
                "#0D47A1",

                //"#E1F5FE",
                "#B3E5FC",
                "#81D4FA",
                "#4FC3F7",
                "#29B6F6",
                "#03A9F4",
                "#039BE5",
                "#0288D1",
                "#0277BD",
                "#01579B",

                //"#E0F7FA",
                "#B2EBF2",
                "#80DEEA",
                "#4DD0E1",
                "#26C6DA",
                "#00BCD4",
                "#00ACC1",
                "#0097A7",
                "#00838F",
                "#006064",

                //"#E0F2F1",
                "#B2DFDB",
                "#80CBC4",
                "#4DB6AC",
                "#26A69A",
                "#009688",
                "#00897B",
                "#00796B",
                "#00695C",
                "#004D40",

                //"#E8F5E9",
                "#C8E6C9",
                "#A5D6A7",
                "#81C784",
                "#66BB6A",
                "#4CAF50",
                "#43A047",
                "#388E3C",
                "#2E7D32",
                "#1B5E20",

                //"#F1F8E9",
                "#DCEDC8",
                "#C5E1A5",
                "#AED581",
                "#9CCC65",
                "#8BC34A",
                "#7CB342",
                "#689F38",
                "#558B2F",
                "#33691E",

                //"#F9FBE7",
                "#F0F4C3",
                "#E6EE9C",
                "#DCE775",
                "#D4E157",
                "#CDDC39",
                "#C0CA33",
                "#AFB42B",
                "#9E9D24",
                "#827717",

                //"#FFFDE7",
                "#FFF9C4",
                "#FFF59D",
                "#FFF176",
                "#FFEE58",
                "#FFEB3B",
                "#FDD835",
                "#FBC02D",
                "#F9A825",
                "#F57F17",

                //"#FFF8E1",
                "#FFECB3",
                "#FFE082",
                "#FFD54F",
                "#FFCA28",
                "#FFC107",
                "#FFB300",
                "#FFA000",
                "#FF8F00",
                "#FF6F00",

                //"#FFF3E0",
                "#FFE0B2",
                "#FFCC80",
                "#FFB74D",
                "#FFA726",
                "#FF9800",
                "#FB8C00",
                "#F57C00",
                "#EF6C00",
                "#E65100",

                //"#FBE9E7",
                "#FFCCBC",
                "#FFAB91",
                "#FF8A65",
                "#FF7043",
                "#FF5722",
                "#F4511E",
                "#E64A19",
                "#D84315",
                "#BF360C",

                //"#EFEBE9",
                "#D7CCC8",
                "#BCAAA4",
                "#A1887F",
                "#8D6E63",
                "#795548",
                "#6D4C41",
                "#5D4037",
                "#4E342E",
                "#3E2723",

                //"#ECEFF1",
                "#CFD8DC",
                "#B0BEC5",
                "#90A4AE",
                "#78909C",
                "#607D8B",
                "#546E7A",
                "#455A64",
                "#37474F",
                "#263238",


























                //GlobalConstants.ColorName.Transparent,
                
                //"#FFFFFF",
                //"#D1D4DC",
                //"#9598A1",
                //"#787B86",
                //"#5D606B",
                //"#434651",
                //"#2A2E39",
                //"#131722",
                //"#000000",

                //"#F23645",
                //"#FF9800",
                //"#FFEB3B",
                //"#4CAF50",
                //"#089981",
                //"#00BCD4",
                //"#3273DC",
                //"#673AB7",
                //"#9C27B0",
                //"#E91E63",


                //"#FCCBCD",
                //"#FFE0B2",
                //"#FFF9C4",
                //"#C8E6C9",
                //"#ACE5DC",
                //"#B2EBF2",
                //"#BBD9FB",
                //"#D1C4E9",
                //"#E1BEE7",
                //"#F8BBD0",



                //"#FAA1A4",
                //"#FFCC80",
                //"#FFF59D",
                //"#A5D6A7",
                //"#70CCBD",
                //"#80DEEA",
                //"#90BFF9",
                //"#B39DDB",
                //"#CE93D8",
                //"#F48FB1",



                //"#F77C80",
                //"#FFB74D",
                //"#FFF176",
                //"#81C784",
                //"#42BDA8",
                //"#4DD0E1",
                //"#5B9CF6",
                //"#9575CD",
                //"#BA68C8",
                //"#F06292",



                //"#F7525F",
                //"#FFA726",
                //"#FFEE58",
                //"#66BB6A",
                //"#22AB94",
                //"#26C6DA",
                //"#3179F5",
                //"#7E57C2",
                //"#AB47BC",
                //"#EC407A",




                //"#B22833",
                //"#F57C00",
                //"#FBC02D",
                //"#388E3C",
                //"#056656",
                //"#0097A7",
                //"#1848CC",
                //"#512DA8",
                //"#7B1FA2",
                //"#C2185B",



                //"#801922",
                //"#E65100",
                //"#F57F17",
                //"#1B5E20",
                //"#00332A",
                //"#006064",
                //"#0C3299",
                //"#311B92",
                //"#4A148C",
                //"#880E4F",





                //GlobalConstants.ColorName.Transparent,
                //"#FFEEEEEE",
                //"#FFB9BDC5",
                //"#FF656C73",
                //"#FF3F454B",

                //"#FF61C1EC",
                //"#FF2196F3",
                //"#FF1976D3",
                //"#FF0F47A0",
                //"#FF29349A",

                //"#FFB88DF1",
                //"#FF9A5CEC",
                //"#FF7930D8",
                //"#FF5B31B1",
                //"#FF46109C",

                //"#FFE66567",
                //"#FFFA3337",
                //"#FFBF0D0C",
                //"#FF980B16",
                //"#FF67050B",

                //"#FFF29779",
                //"#FFF57F17",
                //"#FFFF6F00",
                //"#FFE65100",
                //"#FFB5360E",

                //"#FFF8DF7B",
                //"#FFFFDC48",
                //"#FFEFBB00",
                //"#FFE09C00",
                //"#FFC58A04",

                //"#FF52E5CD",
                //"#FF1ED5B7",
                //"#FF01ABC2",
                //"#FF00838F",
                //"#FF0C6F73",

                //"#FF9CF0A1",
                //"#FF59F559",
                //"#FF1FE612",
                //"#FF06BF00",
                //"#FF249C22",
            };

        }
    }
}
