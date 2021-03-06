﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Logging
{
    /// <summary>
    /// Utility class for sanitizing logging strings.
    /// </summary>
    // Note: This file is shared between the WebJobs SDK and Script repos. Update both if changes are needed.
    internal static class Sanitizer
    {
        private const string SecretReplacement = "[Hidden Credential]";
        private static readonly char[] ValueTerminators = new char[] { '<', '"' };
        private static readonly string[] CredentialTokens = new string[] { "Token=", "DefaultEndpointsProtocol=http", "AccountKey=", "Data Source=", "Server=", "Password=", "pwd=", "&amp;sig=", "SharedAccessKey=" };

        /// <summary>
        /// Removes well-known credential strings from strings.
        /// </summary>
        /// <param name="input">The string to sanitize.</param>
        /// <returns>The sanitized string.</returns>
        internal static string Sanitize(string input)
        {
            if (input == null)
            {
                return null;
            }

            string t = input;
            foreach (var token in CredentialTokens)
            {
                int startIndex = 0;
                while (true)
                {
                    // search for the next token instance
                    startIndex = t.IndexOf(token, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (startIndex == -1)
                    {
                        break;
                    }

                    // Find the end of the secret. It most likely ends with either a double quota " or tag opening <
                    int credentialEnd = t.IndexOfAny(ValueTerminators, startIndex);

                    t = t.Substring(0, startIndex) + SecretReplacement + (credentialEnd != -1 ? t.Substring(credentialEnd) : string.Empty);
                }
            }

            return t;
        }
    }
}
