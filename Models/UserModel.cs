using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Models
{
    public class UserModel : DataModel, IGeneralProperties, IAuthenticationProperties
    {

        // Fields

        private Guid id;
        private string name;
        private string description;

        private string email;
        private string password;

        private List<Guid> projectIds;

        // Constructors

        public UserModel()
        {
            id = Guid.NewGuid();
            projectIds = new List<Guid>();
            email = "";
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
        
        public string Email
        {
            get => email;
            set => SetField(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetField(ref password, value);
        }

        public List<Guid> ProjectIds
        {
            get => projectIds;
            set => SetField(ref projectIds, value);
        }


    }
}
