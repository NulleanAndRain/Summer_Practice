using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LibraryDAL {
	static class ImgReader {
		public static BitmapImage GetImage(Stream stream) {
			if (stream.Length == 0) return null;
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = stream;
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();
			bitmap.Freeze();
			return bitmap;
		}

		public static byte[] GetBytes(BitmapImage img) {
			if (img == null) return new byte[0];
			using (MemoryStream ms = new MemoryStream()) {
				return File.ReadAllBytes(img.UriSource.LocalPath);
			}
		}
	}
}
