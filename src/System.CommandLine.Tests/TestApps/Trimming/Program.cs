﻿using System.CommandLine;
using System.CommandLine.Invocation;

var fileArgument = new Argument<FileInfo>("file");
fileArgument.AcceptLegalFileNamesOnly();

var command = new RootCommand
{
    fileArgument
};

command.SetHandler(context =>
{
    context.Console.Write($"The file you chose was: {context.ParseResult.GetValue(fileArgument)}");
});

command.Invoke(args);
