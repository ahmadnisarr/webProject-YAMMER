using Domain_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    //Implement Bussiness Rule / USE CASES
    public class UserProfileService:IUserProfileService
    {
        private readonly IUserProfile userprofile;
        public UserProfileService(IUserProfile userprofile)
        {
            this.userprofile = userprofile;
        }
        public void Add(userProfile UserProfile)
        {     userprofile.Add(UserProfile);       }
        public Domain_Core.userProfile UserDeatils(int Id)
            { return userprofile.UserDeatils(Id); }
    }
}
