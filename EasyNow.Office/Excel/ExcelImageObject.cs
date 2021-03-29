using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    public class ExcelImageObject
    {
        public byte[] Data { get; set; }
        public ExcelImageType ImageType { get; set; }
        public ExcelAnchor[] Anchors { get; set; }

        internal PictureType GetPictureType()
        {
            switch (ImageType)
            {
                case ExcelImageType.None:
                    return PictureType.None;
                case ExcelImageType.EMF:
                    return PictureType.EMF;
                case ExcelImageType.WMF:
                    return PictureType.WMF;
                case ExcelImageType.PICT:
                    return PictureType.PICT;
                case ExcelImageType.JPEG:
                    return PictureType.JPEG;
                case ExcelImageType.PNG:
                    return PictureType.PNG;
                case ExcelImageType.DIB:
                    return PictureType.DIB;
                case ExcelImageType.GIF:
                    return PictureType.GIF;
                case ExcelImageType.TIFF:
                    return PictureType.TIFF;
                case ExcelImageType.EPS:
                    return PictureType.EPS;
                case ExcelImageType.BMP:
                    return PictureType.BMP;
                case ExcelImageType.WPG:
                    return PictureType.WPG;
                default:
                    return PictureType.Unknown;
            }
        }
    }
}