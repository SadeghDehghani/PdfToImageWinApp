using Docnet.Core.Models;
using Docnet.Core;

using System;

using System.Drawing.Imaging;
using System.IO;

using System.Windows.Forms;
using System.Drawing;

namespace PdfToImage
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            OpenFileDialog openPdf = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "انتخاب فایل PDF"
            };

            if (openPdf.ShowDialog() == DialogResult.OK)
            {
                string pdfPath = openPdf.FileName;

                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "انتخاب پوشه خروجی"
                };

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputFolder = folderDialog.SelectedPath;
                    ConvertPdfToImages(pdfPath, outputFolder);
                }
            }

        }

        private void ConvertPdfToImages(string pdfPath, string outputFolder)
        {
            try
            {
                using (var docReader = DocLib.Instance.GetDocReader(File.ReadAllBytes(pdfPath), new PageDimensions(1080, 1920)))
                {
                    int pageCount = docReader.GetPageCount();

                    for (int i = 0; i < pageCount; i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            int width = pageReader.GetPageWidth();
                            int height = pageReader.GetPageHeight();
                            byte[] rawBytes = pageReader.GetImage();

                            // ساخت یک تصویر با زمینه سفید
                            using (var finalBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                            {
                                using (Graphics g = Graphics.FromImage(finalBitmap))
                                {
                                    g.Clear(Color.White); // 👈 زمینه سفید

                                    // تبدیل rawBytes به Bitmap موقت
                                    using (var rawBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                                    {
                                        var bmpData = rawBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, rawBitmap.PixelFormat);
                                        System.Runtime.InteropServices.Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);
                                        rawBitmap.UnlockBits(bmpData);

                                        // رسم تصویر PDF روی زمینه سفید
                                        g.DrawImage(rawBitmap, 0, 0);
                                    }
                                }

                                string outputPath = Path.Combine(outputFolder, $"Page_{i + 1}.png");
                                finalBitmap.Save(outputPath, ImageFormat.Png);
                                listBoxLogs.Items.Add($"✅ صفحه {i + 1} ذخیره شد: {outputPath}");
                            }
                        }
                    }

                    MessageBox.Show("🎉 همه صفحات با موفقیت ذخیره شدند.", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطا: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConvertPdfToImagesxxxx(string pdfPath, string outputFolder)
        {
            try
            {
                using (var docReader = DocLib.Instance.GetDocReader(File.ReadAllBytes(pdfPath), new PageDimensions(1080, 1920)))
                {
                    int pageCount = docReader.GetPageCount();

                    for (int i = 0; i < pageCount; i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            var width = pageReader.GetPageWidth();
                            var height = pageReader.GetPageHeight();

                            var bytes = pageReader.GetImage(); // returns raw BGRA byte[]

                            using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                            {
                                var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                                System.Runtime.InteropServices.Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);

                                bmp.UnlockBits(bmpData);

                                string outputPath = Path.Combine(outputFolder, $"Page_{i + 1}.png");

                                bmp.Save(outputPath, ImageFormat.Png);

                                listBoxLogs.Items.Add($"✅ صفحه {i + 1} ذخیره شد: {outputPath}");
                            }
                        }
                    }

                    MessageBox.Show("🎉 همه صفحات با موفقیت تبدیل شدند.", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطا: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






    }
}
