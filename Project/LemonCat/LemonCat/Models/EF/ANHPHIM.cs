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
    
    public partial class ANHPHIM
    {
        public int ID { get; set; }
        public Nullable<int> MaPhim { get; set; }
        public string Anh { get; set; }
        public string TenAnh { get; set; }
        public string KichThuoc { get; set; }
        public string NgayCapNhap { get; set; }
    
        public virtual PHIM PHIM { get; set; }
    }
}
