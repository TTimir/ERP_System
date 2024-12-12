using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace CloudERP_System.HelperCls
{
    public class FileHelpers
    {
        public static bool UploadPhoto(HttpPostedFileBase file, string folder, string name)
        {
            // Validate inputs
            if (file == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(folder))
            {
                return false;
            }

            try
            {
                string path = string.Empty;

                if (file != null)
                {
                    path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                    file.SaveAs(path);
                    // Read file into a byte array
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                return true; // Returning the file path upon success
            }
            catch
            {
                return false;
            }
        }
    }
}