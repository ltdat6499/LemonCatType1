//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LemonCat.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class GENRE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GENRE()
        {
            this.GENREPHIMs = new HashSet<GENREPHIM>();
        }
    
        public int MaGenre { get; set; }
        public string TenGenre { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GENREPHIM> GENREPHIMs { get; set; }
    }
}
