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
    
    public partial class SUATCHIEU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUATCHIEU()
        {
            this.ORDERSEATs = new HashSet<ORDERSEAT>();
        }
    
        public int ID { get; set; }
        public Nullable<int> IDRap { get; set; }
        public Nullable<int> IDPhim { get; set; }
        public string Time { get; set; }
        public string DanhSachGheTrong { get; set; }
        public Nullable<int> Price { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDERSEAT> ORDERSEATs { get; set; }
    }
}
