using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _SgtsModUpdater
{
	class FileWriter : TextWriter
	{
		string FileName = "Updater.log";
		public FileStream? FileStream = null;
		public StreamWriter? Writer = null;
		bool initialized = false;

		public FileWriter(string? filename = null)
		{
			if (filename != null && File.Exists(filename))
				FileName = filename;
			if (File.Exists(FileName))
			{
				if (IsFileLocked(new(FileName)))
					return;
				File.Delete(FileName);
			}
			FileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			Writer = new(FileStream);
			Writer.AutoFlush = true;
			initialized = true;
		}
		public override void Close()
		{
			FileStream?.Dispose();
			Writer?.Dispose();
			base.Close();
		}
		public override void Write(char value)
		{
			base.Write(value);
			if (!initialized)
				return;
			Writer?.Write(value);
		}

		public override Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}
		protected virtual bool IsFileLocked(FileInfo file)
		{
			try
			{
				using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
				{
					stream.Close();
				}
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}

			//file is not locked
			return false;
		}
	}
}
