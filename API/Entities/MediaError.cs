﻿using System;
using API.Entities.Interfaces;

namespace API.Entities;

/// <summary>
/// Represents an issue with a file during a file scan that the user should be made aware of
/// </summary>
public class MediaError : IEntityDate
{
    public int Id { get; set; }
    /// <summary>
    /// Format Type (RAR, ZIP, 7Zip, Epub, PDF)
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Full Filepath to the file that has some issue
    /// </summary>
    public string FilePath { get; set; }
    /// <summary>
    /// A type of Error or Unsupported Media
    /// </summary>
    public string MediaErrorType { get; set; } = "Error";
    /// <summary>
    /// Information from the Exception
    /// </summary>
    public string Comment { get; set; }
    /// <summary>
    /// Was the file imported or not
    /// </summary>
    public bool Imported { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}
