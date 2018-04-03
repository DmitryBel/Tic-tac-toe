namespace Gameserver.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class logs
    {
        public int id { get; set; }

        public int? gameID { get; set; }

        public int? turn { get; set; }

        [StringLength(16)]
        public string player { get; set; }

        [StringLength(1)]
        public string mark { get; set; }

        public int? position { get; set; }

        public virtual games games { get; set; }
    }
}
