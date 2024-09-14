using Domain_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUserProfileService
    {
        void Add(userProfile UserProfile);
        Domain_Core.userProfile UserDeatils(int Id);
    }
}
