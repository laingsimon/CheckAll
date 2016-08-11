using System;
using System.Diagnostics;
using System.IO;

namespace CheckAll
{
	class Program
	{
		static void Main(string[] args)
		{
			var messageWriter = new MessageWriter();

			try
			{
				messageWriter.WriteLine("Git directory: {0}", Environment.CurrentDirectory);

				var git = new Git(
						new DirectoryInfo(Environment.CurrentDirectory));
				var application = new Application(
					messageWriter,
					git,
					new FileProcessor(messageWriter, git));

				var request = new Request(args);
				application.Execute(request);
			}
			catch (Exception exc)
			{
				messageWriter.WriteError(exc.ToString());
			}

			if (Debugger.IsAttached)
				Console.ReadKey();
		}
	}
}
