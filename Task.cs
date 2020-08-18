using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    enum TaskStatus
    {
        Open = 1,
        Completed = 2,
        Trashed = 8,
        Archived = 18
    }

    class Task
    {
        public long Id { get; private set; }
        public long ProjectId { get; private set; }
        public long SectionId { get; private set; }
        public string SectionName { get; private set; }
        public string Token { get; private set; }
        public string Name { get; private set; }
        public string Notes { get; private set; }
        public TaskStatus Status { get; private set; }
        public DateTime StatusUpdated { get; private set; }
        public Decimal Sequence { get; private set; }
        public Decimal TrackedTime { get; private set; }
        public long AssigneeId { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Task(dynamic json)
        {
            Id = json.id;
            ProjectId = json.project_id;
            SectionId = json.section_id;
            SectionName = json.section_name;
            Token = json.token;
            Name = json.name;
            Notes = json.notes;
            Status = (TaskStatus)json.status;
            StatusUpdated = DateTime.Parse(json.status_updated_at);
            Sequence = json.sequence;
            TrackedTime = json.tracked_time;
            AssigneeId = json.assigned_to_id == null ? -1 : json.assigned_to_id;
            Created = DateTime.Parse(json.created_at);
            Updated = DateTime.Parse(json.updated_at);
        }

        public override string ToString()
        {
            return "TASK: " + Json.Encode(this);
        }
    }
}
