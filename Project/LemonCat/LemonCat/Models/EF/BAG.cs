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
    
    public partial class BAG
    {
        public int ID { get; set; }
        public Nullable<int> MaTK { get; set; }
        public Nullable<int> MaDVD { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> Gia { get; set; }
    
        public virtual DVD DVD { get; set; }
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
