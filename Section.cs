using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    enum SectionStatus
    {
        Active = 1,
        Trashed = 2
    }

    class Section
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Color { get; private set; }
        public SectionStatus Status { get; private set; }
        public long ProjectId { get; private set; }
        public Decimal Sequence { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Section(dynamic json)
        {
            Id = json.id;
            Name = json.name;
            Description = json.description;
            Color = json.color;
            Status = (SectionStatus)json.status;
            ProjectId = json.project_id;
            Sequence = json.sequence;
            Created = DateTime.Parse(json.created_at);
            Updated = DateTime.Parse(json.updated_at);
        }

        public override string ToString()
        {
            return "SECTION: " + Json.Encode(this);
        }
    }
}
