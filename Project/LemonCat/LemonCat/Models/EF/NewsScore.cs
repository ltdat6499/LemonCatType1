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
    
    public partial class NewsScore
    {
        public int ID { get; set; }
        public Nullable<int> MaTK { get; set; }
        public Nullable<int> NewsID { get; set; }
        public Nullable<bool> Score { get; set; }
    
        public virtual NEW NEW { get; set; }
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
