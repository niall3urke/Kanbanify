using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public class ProjectModel : DataModel, IGeneralProperties, IDisplayProperties , IStateProperties, ICloneable
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

        private List<Guid> stageIds;
        private List<Guid> itemIds;

        // Constructors

        public ProjectModel()
        {
            id = Guid.NewGuid();
            stageIds = new List<Guid>();
            itemIds = new List<Guid>();
            dateModified = DateTime.Now;
            dateCreated = DateTime.Now;
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

        public List<Guid> StageIds
        {
            get => stageIds;
            set => SetField(ref stageIds, value);
        }

        public List<Guid> ItemIds
        {
            get => itemIds;
            set => SetField(ref itemIds, value);
        }

        // Methods

        public object Clone()
        {
            var clone = new ProjectModel
            {
                UseBackgroundImage = UseBackgroundImage,
                BackgroundImageUrl = BackgroundImageUrl,
                BackgroundColor = BackgroundColor,
                Name = Name + "(Copy)"
            };

            // Clone stages

            // Clone items

            // Clone item tasks

            return clone;

        }
    }
}
