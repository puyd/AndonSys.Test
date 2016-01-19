using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace AndonSys.Common
{
    public class ImgPack
    {
        Image img;
        int unit;

        public ImgPack(string FileName, int Unit)
        {
            img = new Bitmap(FileName);
            unit = Unit;
        }

        ~ImgPack()
        {
            img.Dispose();
        }

        public void Draw(Graphics g, Rectangle dest, int line, int index)
        {
            Rectangle rc = new Rectangle(index*unit+1,line*unit+1,unit-2,unit-2);

            g.DrawImage(img, dest, rc,GraphicsUnit.Pixel);
        }
    }
}
