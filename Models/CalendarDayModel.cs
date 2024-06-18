using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public class CalendarDayModel
    {
        // Fields

        private string _buttonType;

        // Properties
        public string ButtonType
        {
            get
            {
                if (IsSelected)
                    return "is-primary";

                return _buttonType;
            }
            set { _buttonType = value; }
        }

        public bool IsSelected { get; set; }

        public bool IsActive { get; set; }

        public int Number { get; set; }

        // Constructors

        public CalendarDayModel(string buttonType, int number)
        {
            ButtonType = buttonType;
            Number = number;
            IsActive = true;
        }

        public CalendarDayModel() { }
    }
}
