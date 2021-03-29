using System.Drawing;

namespace EasyNow.Office.Excel
{
    public class ExcelAnchor
    {
        public int LeftX { get; set; }
        public int TopY { get; set; }
        public int RightX { get; set; }
        public int BottomY { get; set; }
        public Point LeftTopCell { get; set; }
        public Point RightBottomCell { get; set; }
    }
}