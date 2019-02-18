using System;
using System.Collections.Generic;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Saturn.Auditing
{
    public class Audit : Entity
    {
        public string EventType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }

        public List<ChangeLog> diff { get; set; }
        public List<ListChangeLog> listdiff { get; set; }

        public string EntityId { get; set; }
        public Ref<Entity> Entity { get; set; }
    }
}
