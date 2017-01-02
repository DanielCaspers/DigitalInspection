using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalInspection.Models
{
    public class Checklist
    {
        public string Name { get; set; }
        public IList<ChecklistItem> Items { get; set; }
        public Guid Id { get; set; }

    }
}