using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalInspection.Models
{
    public class ChecklistItem
    {
        public string Name { get; set; }
        //public IList<Guid> ParentIds { get; set; }
        public enum Condition {NEEDS_SERVICE, SHOULD_WATCH, ALL_GOOD }
        public Guid Id { get; set; }
    }
}