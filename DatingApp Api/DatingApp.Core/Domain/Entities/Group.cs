using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatingApp.Core.Domain.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = [];
        public Group(string name)
        {
            Name = name;
        }

        public Group()
        {

        }
        

    }
}
