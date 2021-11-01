using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using wize.common.tenancy.Interfaces;

namespace wize.settings.data.V1.Models
{
    public class WizeSetting : ITenantModel
    {
        [Key, Column(Order = 0)]
        public string Name { get; set; }
        public string Value { get; set; }
        [Key, Column(Order = 1)]
        public string Type { get; set; }
    }
}
