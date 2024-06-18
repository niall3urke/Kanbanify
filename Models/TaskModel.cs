using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public class TaskModel : DataModel, IGeneralProperties, IStateProperties, ICloneable 
    {

        // Fields

        private string description;
        private string name;
        private Guid id;

        private DateTime dateCompleted;
        private DateTime dateModified;
        private DateTime dateCreated;
        private DateTime dateDue;
        private bool isCompleted;
        private bool isArchived;
        private bool hasDueDate;
        private bool editing;

        private Guid itemId;

        // Constructors

        public TaskModel()
        {
            id = Guid.NewGuid();
            dateModified = DateTime.Now;
            dateCreated = DateTime.Now;
        }

        // Properties

        public Guid Id
        {
            get => id;
            set => SetField(ref id, value);
        }

        public Guid ItemId
        {
            get => itemId;
            set => SetField(ref itemId, value);
        }

        public string Name
        {
            get => name;
            set => SetField(ref name, value);
        }

        public string Description
        {
            get => description;
            set => SetField(ref description, value);
        }

        public DateTime DateCompleted
        {
            get => dateCompleted;
            set => SetField(ref dateCompleted, value);
        }

        public DateTime DateModified
        {
            get => dateModified;
            set => SetField(ref dateModified, value);
        }

        public DateTime DateCreated
        {
            get => dateCreated;
            set => SetField(ref dateCreated, value);
        }

        public DateTime DateDue
        {
            get => dateDue;
            set => SetField(ref dateDue, value);
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set => SetField(ref isCompleted, value);
        }

        public bool IsArchived
        {
            get => isArchived;
            set => SetField(ref isArchived, value);
        }

        public bool HasDueDate
        {
            get => hasDueDate;
            set => SetField(ref hasDueDate, value);
        }

        public bool Editing
        {
            get => editing;
            set => SetField(ref editing, value);
        }

        // Methods

        public object Clone()
        {
            var clone = new TaskModel
            {
                Description = Description,
                HasDueDate = HasDueDate,
                DateDue = DateDue,
                Name = Name,
            };

            return clone;
        }

    }
}
