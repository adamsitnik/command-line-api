﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Diagnostics;

namespace System.CommandLine
{
    /// <summary>
    /// A symbol, such as an option or command, having one or more fixed names in a command line interface.
    /// </summary>
    internal sealed class IdentifierSymbol
    {
        private readonly HashSet<string> _aliases = new(StringComparer.Ordinal);
        private string? _specifiedName;

        internal IdentifierSymbol(string name, bool removePrefix = true) 
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _specifiedName = removePrefix ? name.RemovePrefix() : name;
        }

        internal IdentifierSymbol(string[] aliases)
        {
            if (aliases is null)
            {
                throw new ArgumentNullException(nameof(aliases));
            }

            if (aliases.Length == 0)
            {
                throw new ArgumentException("An option must have at least one alias.", nameof(aliases));
            }

            for (var i = 0; i < aliases.Length; i++)
            {
                AddAlias(aliases[i]);
            }
        }

        /// <summary>
        /// Gets the set of strings that can be used on the command line to specify the symbol.
        /// </summary>
        internal IReadOnlyCollection<string> Aliases => _aliases;

        internal string? Name
        {
            get => _specifiedName;
            set
            {
                if (_specifiedName is null || !string.Equals(_specifiedName, value, StringComparison.Ordinal))
                {
                    AddAlias(value!);

                    if (_specifiedName is { })
                    {
                        RemoveAlias(_specifiedName);
                    }

                    _specifiedName = value;
                }
            }
        }

        /// <summary>
        /// Adds an <see href="/dotnet/standard/commandline/syntax#aliases">alias</see>.
        /// </summary>
        /// <param name="alias">The alias to add.</param>
        /// <remarks>
        /// You can add multiple aliases for a symbol.
        /// </remarks>
        internal void AddAlias(string alias)
        {
            ThrowIfAliasIsInvalid(alias);

            _aliases.Add(alias);
        }

        internal void RemoveAlias(string alias) => _aliases.Remove(alias);

        /// <summary>
        /// Determines whether the specified alias has already been defined.
        /// </summary>
        /// <param name="alias">The alias to search for.</param>
        /// <returns><see langword="true" /> if the alias has already been defined; otherwise <see langword="false" />.</returns>
        internal bool HasAlias(string alias) => _aliases.Contains(alias);

        internal string GetLongestAlias()
        {
            string max = "";
            foreach (string alias in _aliases)
            {
                if (alias.Length > max.Length)
                {
                    max = alias;
                }
            }
            return max.RemovePrefix();
        }

        [DebuggerStepThrough]
        private void ThrowIfAliasIsInvalid(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentException("An alias cannot be null, empty, or consist entirely of whitespace.");
            }

            for (var i = 0; i < alias.Length; i++)
            {
                if (char.IsWhiteSpace(alias[i]))
                {
                    throw new ArgumentException($"Alias cannot contain whitespace: \"{alias}\"", nameof(alias));
                }
            }
        }
    }
}