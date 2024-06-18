using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public class ItemModel : DataModel, IGeneralProperties, IDisplayProperties, IStateProperties, ICloneable
    {

        // Fields

        private Guid id;
        private string name;
        private string description;

        private string backgroundColor;
        private string backgroundImageUrl;
        private bool useBackgroundImage;

        private DateTime dateCompleted;
        private DateTime dateModified;
        private DateTime dateCreated;
        private DateTime dateDue;
        private bool isCompleted;
        private bool isArchived;
        private bool hasDueDate;

        private ItemFrequency frequency;
        private ItemPriority priority;

        private List<Guid> taskIds;
        private Guid stageId;

        public ItemModel()
        {
            id = Guid.NewGuid();
            dateModified = DateTime.Now;
            dateCreated = DateTime.Now;
            taskIds = new List<Guid>();
        }

        // Properties

        public Guid Id
        {
            get => id;
            set => SetField(ref id, value);
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

        public string BackgroundColor
        {
            get => backgroundColor;
            set => SetField(ref backgroundColor, value);
        }

        public string BackgroundImageUrl
        {
            get => backgroundImageUrl;
            set => SetField(ref backgroundImageUrl, value);
        }

        public bool UseBackgroundImage
        {
            get => useBackgroundImage;
            set => SetField(ref useBackgroundImage, value);
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
            set
            {
                DateCompleted = DateTime.Now;
                SetField(ref isCompleted, value);
            }

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

        public ItemFrequency Frequency
        {
            get => frequency;
            set => SetField(ref frequency, value);
        }

        public ItemPriority Priority
        {
            get => priority;
            set => SetField(ref priority, value);
        }

        public Guid StageId
        {
            get => stageId;
            set => SetField(ref stageId, value);
        }

        public List<Guid> TaskIds
        {
            get => taskIds;
            set => SetField(ref taskIds, value);
        }

        // Methods

        public object Clone()
        {
            // Clone item
            var clone = new ItemModel
            {
                UseBackgroundImage = UseBackgroundImage,
                BackgroundImageUrl = BackgroundImageUrl,
                BackgroundColor = BackgroundColor,
                Description = Description,
                HasDueDate = HasDueDate,
                Name = Name + " (Copy)",
                Frequency = Frequency,
                Priority = Priority,
                DateDue = DateDue,
                StageId = StageId
            };

            // Clone tasks

            return clone;
        }
    }
}
