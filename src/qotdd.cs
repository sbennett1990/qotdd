/*
 * Copyright (c) 2020 Scott Bennett <scottb@fastmail.com>
 *
 * Permission to use, copy, modify, and distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using libcmdline;

public class Program
{
	private const string Dictionary_Path = "";
	private const string Proc_Name = "qotdd";

	public static void Main(string[] args)
	{
		bool listener_flag = false;	// Listen to network requests
		bool reader_flag = false;	// Read the dictionary file

		string readerHandle = null;

		/*
		CommandLineProcessor argsProcessor = new CommandLineProcessor();
		argsProcessor.RegisterOptionMatchHandler("L", requiresArgument: false, (sender, o) => {
			listener_flag = true;
		});
		argsProcessor.RegisterOptionMatchHandler("R", requiresArgument: false, (sender, o) => {
		*/
			reader_flag = true;
		/*
		});
		argsProcessor.RegisterOptionMatchHandler("H", requiresArgument: true, (sender, o) => {
			readerHandle = o.Argument;
		});
		argsProcessor.RegisterOptionMatchHandler(CommandLineProcessor.InvalidOptionIdentifier, (sender, o) => {
			//Usage();
			Environment.Exit(1);
		});
		argsProcessor.ProcessCommandLineArgs(args);
		*/

		if (listener_flag && reader_flag) {
			// Error!
			Environment.Exit(1);
		}

		if (listener_flag) {
			// run as the network Listener
			Listener();
		}

		if (reader_flag) {
			// run as the dictionary reader
			Reader(readerHandle);
		}

		// This is what the main app does
		ForkListener();
		ForkReader();
	}

	/// <summary>
	/// Run this process as the network listener (frontend).
	/// </summary>
	private static void Listener()
	{

	}

	/// <summary>
	/// Run this process as the dictionary reader (engine).
	/// </summary>
	private static void Reader(string readerHandle)
	{
		Reader reader = new Reader(Dictionary_Path, pipeHandleAsString: readerHandle);
		reader.Start();
	}

	/// <summary>
	/// Setup and exec the listener process.
	/// </summary>
	private static void ForkListener()
	{

	}

	/// <summary>
	/// Setup and exec the reader process.
	/// </summary>
	private static void ForkReader()
	{
		
	}
}
