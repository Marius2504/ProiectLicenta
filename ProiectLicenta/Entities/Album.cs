﻿using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int ArtistId { get; set; }
        [JsonIgnore]
        public Artist Artist { get; set; }
        [JsonIgnore]
        public List<Song> Songs { get; set; }
    }
}