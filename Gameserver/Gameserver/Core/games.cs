namespace Gameserver.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class games
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public games()
        {
            logs = new HashSet<logs>();
        }

        public int id { get; set; }

        public DateTime? created { get; set; }

        [StringLength(16)]
        public string winner { get; set; }

        [StringLength(16)]
        public string opponent { get; set; }

        [StringLength(1)]
        public string playerOneMark { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<logs> logs { get; set; }
    }
}
