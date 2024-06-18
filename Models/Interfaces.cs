using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public interface IGeneralProperties
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public interface IAuthenticationProperties
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public interface IDisplayProperties
    {
        public string BackgroundColor { get; set; }

        public string BackgroundImageUrl { get; set; }

        public bool UseBackgroundImage { get; set; }
    }

    public interface IStateProperties
    {
        public DateTime DateCompleted { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateDue { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsArchived { get; set; }

        public bool HasDueDate { get; set; }
    }
}
