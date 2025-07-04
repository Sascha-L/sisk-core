﻿// The Sisk Framework source code
// Copyright (c) 2024- PROJECT PRINCIPIUM and all Sisk contributors
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   MimeHelper.cs
// Repository:  https://github.com/sisk-http/core

using Sisk.Core.Internal;

namespace Sisk.Core.Helpers;

/// <summary>
/// Provides useful helper methods for resolving mime-types from common formats.
/// </summary>
public static class MimeHelper {
    static readonly int inlineContentTypesSize = MimeTypeList.InlineMimeTypes.Length;

    /// <summary>
    /// Gets or sets the <see cref="MimeHelper"/> default fallback mime-type.
    /// </summary>
    /// <remarks>
    /// This property is not used by the HTTP server itself, only this helper class.
    /// </remarks>
    public static string DefaultMimeType { get; set; } = "application/octet-stream";

    /// <summary>
    /// Gets the content mime-type from the specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension, file path or the extension, with or without the dot.</param>
    /// <param name="fallback">Optional. The default mime-type when the file best mime-type is not found. If this argument is null, <see cref="DefaultMimeType"/> is used.</param>
    /// <returns>The best matched mime-type, or the default if no mime-type was matched with the specified extension.</returns>
    public static string GetMimeType ( string fileExtension, string? fallback = null ) {
        string extension;
        if (fileExtension.StartsWith ( '.' )) {
            extension = fileExtension.Substring ( 1 );
        }
        else if (fileExtension.Contains ( '/' ) || fileExtension.Contains ( '\\' )) {
            extension = Path.GetExtension ( fileExtension ).TrimStart ( '.' );
        }
        else {
            extension = fileExtension;
        }
        return MimeTypeList.ResolveMimeType ( extension.ToLowerInvariant () ) ?? fallback ?? DefaultMimeType;
    }

    /// <summary>
    /// Gets an boolean indicating if the specified file is an well-known plain text file.
    /// </summary>
    /// <param name="fileExtension">The file extension, with or without the initial dot.</param>
    public static bool IsPlainTextFile ( string? fileExtension ) {
        if (fileExtension is null)
            return false;
        return PlainTextFileExtensions.IsTextFile ( fileExtension.TrimStart ( '.' ) );
    }

    /// <summary>
    /// Gets a value indicating whether the specified MIME type is a plain text MIME type.
    /// </summary>
    /// <param name="mimeType">The MIME type to check.</param>
    /// <returns><see langword="true"/> if the MIME type is a plain text MIME type; otherwise, <see langword="false"/>.</returns>
    public static bool IsPlainTextMimeType ( string? mimeType ) {
        if (mimeType is null)
            return false;
        return
            mimeType.StartsWith ( "text/", StringComparison.OrdinalIgnoreCase ) ||
            mimeType.StartsWith ( "message/", StringComparison.OrdinalIgnoreCase ) ||
            PlainTextFileMimeTypes.PlainTextMimeTypes.Contains ( mimeType, StringComparer.OrdinalIgnoreCase );
    }

    /// <summary>
    /// Determines whether the specified mime-type is considered an inline content type 
    /// that can be displayed directly in most browsers.
    /// </summary>
    /// <param name="mimeType">The mime-type to evaluate.</param>
    /// <returns>
    /// <see langword="true"></see> if the content type is an inline content type; otherwise, <see langword="false"></see>.
    /// </returns>
    public static bool IsBrowserKnownInlineMimeType ( string mimeType ) {
        for (int i = 0; i < inlineContentTypesSize; i++) {
            if (mimeType.Contains ( MimeTypeList.InlineMimeTypes [ i ], StringComparison.OrdinalIgnoreCase )) {
                return true;
            }
        }
        return false;
    }
}
