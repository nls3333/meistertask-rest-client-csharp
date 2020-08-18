using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    enum ProjectStatus
    {
        Active = 1,
        Trashed = 4,
        Archived = 5
    }

    class Project
    {
        public long Id { get; private set; }
        public string Token { get; private set; }
        public ProjectStatus Status { get; private set; }
        public string Name { get; private set; }
        public string Notes { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Project(dynamic json)
        {
            Id = json.id;
            Token = json.token;
            Status = (ProjectStatus)json.status;
            Name = json.name;
            Notes = json.notes;
            Created = DateTime.Parse(json.created_at);
            Updated = DateTime.Parse(json.updated_at);
        }

        public override string ToString()
        {
            return "PROJECT: " + Json.Encode(this);
        }
    }
}
