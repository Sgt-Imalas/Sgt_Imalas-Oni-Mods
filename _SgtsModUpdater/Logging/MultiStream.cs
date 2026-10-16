using System.IO;
using System.Text;

namespace _SgtsModUpdater
{
    class MultiStream : TextWriter
	{
		List<TextWriter> writers = new List<TextWriter>();

		public MultiStream(TextWriter original)
		{
			writers.Add(original);
		}
		public void AddWriter(TextWriter writer)
		{
			if (writers.Contains(writer))
				return;
			writers.Add(writer);
		}

		public override void Write(char value)
		{
			base.Write(value);
			foreach(var writer in  writers) 
				writer.Write(value);
		}

		public override Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}
	}
}
