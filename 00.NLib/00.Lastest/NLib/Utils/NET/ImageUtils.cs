#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- Image Utils class updated
  - Change debug code back to using MethodBase instead of StackFrame.

======================================================================================================================
Update 2015-01-12
=================
- Image Utils class updated
  - Change GetImageStream to GetImage.
  - Add GetImage from Embeded Resource.

======================================================================================================================
Update 2013-08-19
=================
- Image Utils class ported and updated
  - Port Image Utils class from TML project.
  - Update code by used new debug framework.
  - Add GetImageStream from System.Drawing.Image.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using NLib;

#endregion

namespace NLib.Utils
{
    /// <summary>
    /// Image Utils helper class.
    /// </summary>
    public class ImageUtils
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        static ImageUtils()
        {
        }

        #endregion

        #region Public Methods

        #region GetImage(s)

        /// <summary>
        /// Gets Image from file in byte array.
        /// </summary>
        /// <param name="fileName">The target file to load.</param>
        /// <returns>Returns image byte array.</returns>
        public static byte[] GetImage(string fileName)
        {
            byte[] results = null;
            System.IO.FileStream fs = null;
            System.IO.BinaryReader reader = null;
            long size = 0L;
            MethodBase med = MethodBase.GetCurrentMethod();

            #region Check File is exists

            if (!System.IO.File.Exists(fileName))
            {
                "File Name : {0} not found.".Info(med, fileName);
            }
            else
            {
                "File Name : {0} found.".Info(med, fileName);
            }

            #endregion

            #region Open Image File

            try
            {
                fs = new System.IO.FileStream(fileName,
                    System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
#if THROW_EXCEPTION
                throw ex;
#else
                "Cannot open image file.".Err(med);
                ex.Err(med);
#endif
            }

            // Checks if cannot open file return null.
            if (null == fs)
                return results;

            #endregion

            #region Check File Size

            try
            {
                size = fs.Length;
                fs.Position = 0;
            }
            catch (Exception ex)
            {
                size = 0L;
#if THROW_EXCEPTION
                throw ex;
#else
                "Detected error during access FileStream.Length or Set FileStream Position.".Err(med);
                ex.Err(med);
#endif
            }
            // Check if file size is less than or equal to zero, File stream is null
            // Return null if required information is not prepated.
            if (size <= 0L || null == fs)
                return results;

            #endregion

            #region Read data into buffers

            bool isReaderCreated = false;

            try
            {
                reader = new System.IO.BinaryReader(fs);
                isReaderCreated = true;
            }
            catch (Exception ex)
            {
                isReaderCreated = false;
#if THROW_EXCEPTION
                throw ex;
#else
                "Detected error during create bynary reader.".Err(med);
                ex.Err(med);
#endif
            }

            if (isReaderCreated && null != reader)
            {
                try
                {
                    results = reader.ReadBytes((int)size);
                }
                catch (Exception ex)
                {
                    results = null;
#if THROW_EXCEPTION
                    throw ex;
#else
                    "Detected error during read data from bynary reader.".Err(med);
                    ex.Err(med);
#endif
                }
            }

            #endregion

            #region Release Binary Reader

            if (null != reader)
            {
                try { reader.Close(); }
                catch { } // no need to keep exception to log.
                try { reader.Dispose(); }
                catch { } // no need to keep exception to log.
            }
            // release reference
            reader = null;

            #endregion

            #region Release FileStream resources

            if (null != fs)
            {
                try { fs.Close(); }
                catch { } // no need to keep exception to log.
            }
            if (null != fs)
            {
                try { fs.Dispose(); }
                catch { } // no need to keep exception to log.
            }
            // Release reference.
            fs = null;

            #endregion

            return results;
        }
        /// <summary>
        /// Gets Image from System.Drawing.Image in byte array.
        /// </summary>
        /// <param name="img">The instance of System.Drawing.Image.</param>
        /// <returns>Returns image byte array.</returns>
        public static byte[] GetImage(System.Drawing.Image img)
        {
            byte[] results = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == img)
            {
                "Parameter Image is null.".Err(med);
                return results;
            }

            try
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0; // reset position.
                results = ms.GetBuffer();
                if (null != ms)
                {
                    ms.Dispose();
                }
            }
            catch (Exception ex)
            {
                results = null;
                "Detected error during build BitmapSource from MemoryStream.".Err(med);
                ex.Err(med);
            }
            finally
            {
                if (null != img)
                {
                    img.Dispose();
                }
                img = null;
            }

            return results;
        }
        /// <summary>
        /// Get Image from embeded resource in byte array.
        /// </summary>
        /// <param name="assembly">The resource's assembly.</param>
        /// <param name="resourceName">
        /// The full resource name. The resource should set to Embeded Resource.
        /// </param>
        /// <returns>Returns image byte array.</returns>
        public static byte[] GetImage(Assembly assembly, string resourceName)
        {
            if (null == assembly)
                return null;

            byte[] buffers = null;
            Stream stream = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            try
            {
                stream = assembly.GetManifestResourceStream(resourceName);
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
            if (null != stream)
            {
                buffers = new byte[stream.Length];
                try
                {
                    stream.Read(buffers, 0, (int)stream.Length);
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                }
            }
            if (null != stream)
            {
                try
                {
                    stream.Close();
                    stream.Dispose();
                }
                catch { }

            }
            stream = null;

            return buffers;
        }

        #endregion

        #region GetBitmapSource(s)

        /// <summary>
        /// Gets Bitmap Source from byte array.
        /// </summary>
        /// <param name="buffers">The image byte array.</param>
        /// <returns>
        /// Returns BitmapSource instance for binding with Image control.
        /// </returns>
        public static BitmapSource GetBitmapSource(byte[] buffers)
        {
            BitmapImage img = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == buffers || buffers.Length <= 0)
            {
                // buffer is null.
                "Parameter 'buffers' is null or empty.".Err(med);
                return img;
            }

            using (System.IO.MemoryStream mem =
                new System.IO.MemoryStream(buffers, 0, buffers.Length, false, true))
            {
                try
                {
                    img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = mem;
                    img.EndInit();
                    img.Freeze();
                }
                catch (Exception ex)
                {
                    img = null;
#if THROW_EXCEPTION
                    throw ex;
#else
                    ex.Err(med);
#endif
                }
            }

            return img;
        }
        /// <summary>
        /// Gets bitmap source from Uri.
        /// </summary>
        /// <param name="uri">The source Uri.</param>
        /// <returns>
        /// Returns BitmapSource instance for binding with Image control.
        /// </returns>
        public static BitmapSource GetBitmapSource(Uri uri)
        {
            BitmapImage img = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == uri)
            {
                "Parameter 'uri' is null.".Err(med);
                return img;
            }

            try
            {
                img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                img.UriSource = uri;
                img.EndInit();
            }
            catch (Exception ex)
            {
                img = null; // reset image.
#if THROW_EXCEPTION
                throw ex;
#else
                "Detected error during load bitmap source from Uri.".Err(med);
                object dump = new object();
                dump.Dump(med, new { uri.AbsoluteUri });
                dump.Dump(med, new { uri.AbsolutePath });
                dump.Dump(med, new { uri.Authority });
                ex.Err(med);
                return img;
#endif
            }

            return img;
        }

        #endregion

        #region GrayScale

        /// <summary>
        /// Convert image to Gray Scale.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <returns>Returns Gray Scale BitmapSource.</returns>
        public static BitmapSource GrayScale(BitmapSource image)
        {
            FormatConvertedBitmap img = null;
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == image)
            {
                "Parameter 'image' is null.".Err(med);
                return img;
            }

            try
            {
                img = new FormatConvertedBitmap();
                img.BeginInit();
                img.Source = image;
                img.DestinationFormat = PixelFormats.Gray8;
                img.EndInit();
                img.Freeze();
            }
            catch (Exception ex)
            {
                img = null;
#if THROW_EXCEPTION
                throw ex;
#else
                "Detected error during convert bitmap source to gray scale.".Err(med);
                ex.Err(med);
#endif
            }

            return img;
        }

        #endregion

        #endregion
    }
}
