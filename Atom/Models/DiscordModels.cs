using System.Collections.Generic;

namespace Atom.Models
{
    public class GuildData
    {
        public string Name { get; set; } = string.Empty;
        public ulong Id { get; set; }
        public List<RoleData> Roles { get; set; } = new();
        public List<CategoryData> Categories { get; set; } = new();
        public List<ChannelData> ChannelsWithoutCategory { get; set; } = new();
    }

    public class RoleData
    {
        public string Name { get; set; } = string.Empty;
        public uint Color { get; set; }
        public ulong Permissions { get; set; }
        public bool IsHoisted { get; set; }
        public bool IsMentionable { get; set; }
        public int Position { get; set; }
    }

    public class CategoryData
    {
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public List<ChannelData> Channels { get; set; } = new();
    }

    public class ChannelData
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Position { get; set; }
        public string? Topic { get; set; }
        public bool IsNsfw { get; set; }
    }
}
