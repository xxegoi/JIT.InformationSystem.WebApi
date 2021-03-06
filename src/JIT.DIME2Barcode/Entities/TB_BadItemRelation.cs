﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JIT.DIME2Barcode.Entities
{
    public partial class TB_BadItemRelation
    {
        [Key]
        public int FID { get; set; }
        public int? FItemID { get; set; }
        public int FOperID { get; set; }
        public string FNumber { get; set; }
        public string FName { get; set; }
        public bool? FDeleted { get; set; }
        public string FRemark { get; set; }
    }
}