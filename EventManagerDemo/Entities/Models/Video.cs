﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models {
  public class Video {
    public string ChannelId { get; set; }
    public string VideoId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int DurationInSeconds { get; set; }
    public string DisplayFormUrl { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    public string ThumbnailUrl { get; set; }
    public int ViewCount { get; set; }
  }
}