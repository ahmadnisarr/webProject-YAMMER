using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain_Core;
using Application;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace webApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostingController : ControllerBase
    {
        private readonly ILogger<PostingController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IPostingService postingService;
        private readonly IUserProfileService userProfile1;
        private readonly IRepositoryService<userProfile> userProfilegenericRepo;
        private readonly IRepositoryService<Posting> postingGenericRepo;

        public PostingController(ILogger<PostingController> logger, IWebHostEnvironment env, IPostingService postingService, IUserProfileService userProfile, IRepositoryService<userProfile> repo, IRepositoryService<Posting> postingGenericRepo)
        {
            _logger = logger;
            _env = env;
            this.postingService = postingService;
            this.userProfile1 = userProfile;
            this.userProfilegenericRepo = repo;
            this.postingGenericRepo = postingGenericRepo;
        }

        private void Shuffle<T>(List<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        [HttpGet("Index")]
        [Authorize]
        public IActionResult Index()
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<postANDimage> posts = postingService.getAll();
            Shuffle<postANDimage>(posts);

            //string userName = postingService.getUserName(userId);
            //var profilePath = postingService.getUserProfilePictureFromDb(userId);

            //postingService.checkAndAddUserId(userId, userName);

            //var Id = userProfilegenericRepo.GetId(userId);
            //var userProfilesDetails = userProfilegenericRepo.GetById(Id);
            //userProfilesDetails.profilePath = postingService.setPath(userProfilesDetails.profilePath);

            var indexView = new userIndexViewModel
            {
                //UserProfile = userProfilesDetails,
                Posts = posts
            };

            return Ok(indexView);
        }

        //[HttpPost("AddProfilePicture")]
        //public async Task<IActionResult> AddProfilePicture(IFormFile image)
        //{
        //    string wwwRootPath = _env.WebRootPath;
        //    string path = Path.Combine(wwwRootPath, "userProfileImage");

        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    if (image?.Length > 0)
        //    {
        //        string filePath = Path.Combine(path, image.FileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await image.CopyToAsync(fileStream);
        //        }

        //        string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //        postingService.addUserProfilePictureInDb(filePath, userId);

        //        return Ok("Profile picture uploaded successfully.");
        //    }

        //    return BadRequest("Invalid image file.");
        //}

        //[HttpPost("AddPost")]
        //public async Task<IActionResult> AddPost(string publication, List<IFormFile> picture)
        //{
        //    if (string.IsNullOrEmpty(publication) || picture == null || !picture.Any())
        //    {
        //        return BadRequest("Invalid post data.");
        //    }

        //    string wwwRootPath = _env.WebRootPath;
        //    string path = Path.Combine(wwwRootPath, "uploadFiles");

        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    Posting post = new Posting
        //    {
        //        userId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
        //        publication = publication,
        //        postDate = DateTime.Now
        //    };

        //    List<string> images = new List<string>();
        //    foreach (var file in picture)
        //    {
        //        if (file.Length > 0)
        //        {
        //            string filePath = Path.Combine(path, file.FileName);
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(fileStream);
        //                images.Add(filePath);
        //            }
        //        }
        //    }

        //    postingService.add(post, images);
        //    return Ok("Post uploaded successfully.");
        //}

        [HttpDelete("DeletePost")]
        public IActionResult DeletePost(int Id, string userId)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            postingService.deletePost(Id, userId);
            return Ok("Post deleted successfully.");
        }

        [HttpPut("UpdatePost")]
        public IActionResult UpdatePost(int Id, string publication)
        {
            if (string.IsNullOrEmpty(publication))
            {
                return BadRequest("Invalid publication content.");//400
            }

            postingService.updatePost(Id, publication);
            return Ok("Post updated successfully."); //200
        }

        [HttpGet("EditPost")]
        public IActionResult EditPost(int Id)
        {
            var postData = new postANDimage
            {
                posting = postingGenericRepo.GetById(Id),
                imagePath = postingService.getPostImages(Id)
            };

            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //postData.profilePath = postingService.getUserProfilePictureFromDb(userId);
            //postData.userName = postingService.getUserName(userId);

            return Ok(postData); //200
        }

        [HttpGet("ViewProfile")]
        public IActionResult ViewProfile(int Id)
        {
            
               userProfile user = userProfile1.UserDeatils(Id);
                user.profilePath = postingService.setPath(user.profilePath);

            return Ok(user);
        }

        [HttpGet("EditProfile")]
        [Authorize]
        public IActionResult EditProfile(int Id)
        {
            var userProfiles = userProfile1.UserDeatils(Id);
            userProfiles.profilePath = postingService.setPath(userProfiles.profilePath);
            return Ok(userProfiles);
        }

        [HttpPut("EditProfile")]
        public IActionResult EditProfile(int Id, [FromBody] userProfile updatedProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //updatedProfile.userId = userId;

            userProfile1.Add(updatedProfile);
            return Ok("Profile updated successfully.");
        }

        //[HttpGet("Messages")]
        //public IActionResult Messages()
        //{

        //    //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    //ViewBag.senderName = postingService.getUserName(userId);
        //    //ViewBag.senderId = userId;
        //    //ViewBag.profilePath = postingService.getUserProfilePictureFromDb(userId);

        //    List<userProfile> users = userProfilegenericRepo.GetAll().ToList();
        //    foreach (var user in users)
        //    {
        //        user.profilePath = postingService.setPath(user.profilePath);
        //    }
        //    return Ok(users);
        //}

        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages([FromQuery] string userId, [FromQuery] string receiverId)
        {
            try
            {
                List<messages> msgs = await postingService.GetMsg(userId, receiverId);
                return Ok(msgs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
