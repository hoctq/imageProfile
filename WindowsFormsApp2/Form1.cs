using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
//using AForge;
//using AForge.Imaging;
//using AForge.Math.Geometry;
//using Point = System.Drawing.Point;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        string path;
        Bitmap imageLast;
        public Form1()
        {
            InitializeComponent();
            
        }

        private static Bitmap resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            //if (nPercentH < nPercentW)
            //    nPercent = nPercentH;
            //else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        private static Bitmap cropImage(Image img, Rectangle cropArea)
        {
            try
            {
                Bitmap bmpImage = new Bitmap(img);
                Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
                return bmpCrop;
            }
            catch (System.OutOfMemoryException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private Bitmap setFinalSizeImage(Bitmap img) {
            Bitmap bitmap = new Bitmap(640, 640);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            g.DrawImage(img, new Point(0, 0));
            g.Dispose();
            return bitmap;
        }

        private Bitmap rotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }

        private void btnHandle_Click(object sender, EventArgs e)
        {
            if (path == null || path.Length == 0) {
                MessageBox.Show("Please choose image before");
                return;
            }
            Image img = Image.FromFile(path);
            Size size = new Size(640, 640);
            Rectangle rect = new Rectangle(0, 210, 640, 440);

            Bitmap image = resizeImage(img, size);
            Bitmap imageCrop = cropImage(image, rect);

            Bitmap imageFinalSize = setFinalSizeImage(imageCrop);

            imageLast = rotateImage(imageFinalSize, 270);
            pictureBox1.Image = imageLast;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.ShowDialog();
            
            path = openFileDialog1.FileName;
            if (path != null || path.Length > 0) {
                btnHandle.Enabled = true;
                pictureBox1.Image = Image.FromFile(path);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Images|*.png;*.jpeg;*.jpg";
            saveFileDialog1.InitialDirectory = @"C:\";
            ImageFormat format = ImageFormat.Png;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(saveFileDialog1.FileName);
                string pathSave = saveFileDialog1.FileName;
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                }
                imageLast.Save(saveFileDialog1.FileName, format);
                path = "";
                imageLast = null;
                btnHandle.Enabled = false;
            }
        }

    }
}
