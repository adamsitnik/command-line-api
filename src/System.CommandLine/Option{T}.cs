// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Completions;
using System.CommandLine.Parsing;
using System.Linq;

namespace System.CommandLine
{
    /// <summary>
    /// A symbol defining a named parameter and a value for that parameter. 
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> that the option's arguments are expected to be parsed as.</typeparam>
    public class Option<T> : Argument<T>
    {
        private readonly IdentifierSymbol _id;

        /// <inheritdoc/>
        public Option(string name, string? description = null) : base(null, description)
        {
            _id = new IdentifierSymbol(name);
        }

        /// <inheritdoc/>
        public Option(string[] aliases, string? description = null) : base(null, description)
        {
            _id = new IdentifierSymbol(aliases);
        }

        /// <inheritdoc/>
        public Option(
            string name,
            Func<ArgumentResult, T> parseArgument,
            bool isDefault = false,
            string? description = null) 
            : base(name, parseArgument, isDefault, description)
        {
            _id = new IdentifierSymbol(name);
        }

        /// <inheritdoc/>
        public Option(
            string[] aliases,
            Func<ArgumentResult, T> parseArgument,
            bool isDefault = false,
            string? description = null) 
            : base(null, parseArgument, isDefault, description)
        {
            _id = new IdentifierSymbol(aliases);
        }

        /// <inheritdoc/>
        public Option(
            string name,
            Func<T> defaultValueFactory,
            string? description = null) 
            : base(name, defaultValueFactory, description)
        {
            _id = new IdentifierSymbol(name);
        }

        /// <inheritdoc/>
        public Option(
            string[] aliases,
            Func<T> defaultValueFactory,
            string? description = null)
            : base(null!, defaultValueFactory, description)
        {
            _id = new IdentifierSymbol(aliases);
        }

        /// <inheritdoc/>
        public override string Name
        { 
            get => _id.Name ?? DefaultName;
            set => _id.Name = value;
        }

        /// <summary>
        /// Gets a value that indicates whether multiple argument tokens are allowed for each option identifier token.
        /// </summary>
        /// <example>
        /// If set to <see langword="true"/>, the following command line is valid for passing multiple arguments:
        /// <code>
        /// > --opt 1 2 3
        /// </code>
        /// The following is equivalent and is always valid:
        /// <code>
        /// > --opt 1 --opt 2 --opt 3
        /// </code>
        /// </example>
        public bool AllowMultipleArgumentsPerToken { get; set; }

        /// <summary>
        /// Indicates whether the option is required when its parent command is invoked.
        /// </summary>
        /// <remarks>When an option is required and its parent command is invoked without it, an error results.</remarks>
        public bool IsRequired { get; set; }

        internal virtual bool IsGreedy => typeof(T) != typeof(bool) && Arity.MinimumNumberOfValues > 0;

        /// <summary>
        /// Global options are applied to the command and recursively to subcommands.
        /// They do not apply to parent commands.
        /// </summary>
        internal bool IsGlobal { get; set; }

        internal bool DisallowBinding { get; init; }

        private protected override string DefaultName => _id.GetLongestAlias();

        /// <inheritdoc />
        public override IEnumerable<CompletionItem> GetCompletions(CompletionContext context)
        {
            List<CompletionItem>? completions = null;

            foreach (var completion in base.GetCompletions(context))
            {
                if (completion.Label.ContainsCaseInsensitive(context.WordToComplete))
                {
                    (completions ??= new List<CompletionItem>()).Add(completion);
                }
            }

            if (completions is null)
            {
                return Array.Empty<CompletionItem>();
            }

            return completions
                   .OrderBy(item => item.SortText.IndexOfCaseInsensitive(context.WordToComplete))
                   .ThenBy(symbol => symbol.Label, StringComparer.OrdinalIgnoreCase);
        }
    }
}