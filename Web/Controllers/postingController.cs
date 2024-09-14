using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using Domain_Core;
using Application;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Controllers
{
    [Authorize]
    public class postingController : Controller
    {
        private readonly IMemoryCache _cache;//for im-memory cache
        private readonly PerformanceCounter _cpuCounter; //profiling log
        private readonly PerformanceCounter _memoryCounter;
        private string connectionString = "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = YammerDB; Integrated Security = True;";
        private readonly ILogger<postingController> _logger;
        private readonly IWebHostEnvironment _env;
        private IPostingService postingService;
        private IUserProfileService userProfile1;
        private IRepositoryService<userProfile> userProfilegenericRepo;
        private IRepositoryService<Posting> postingGenericRepo;
        public postingController(IMemoryCache cache, ILogger<postingController> logger, IWebHostEnvironment env, IPostingService postingService, IUserProfileService userProfile, IRepositoryService<userProfile> repo, IRepositoryService<Posting> postingGenericRepo)
        {
            _logger = logger;
            _env = env;
            this.postingService = postingService;
            this.userProfile1 = userProfile;
            this.userProfilegenericRepo = repo;
            this.postingGenericRepo = postingGenericRepo;
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            _cache = cache;
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

        //log perfomance meterices
        public IActionResult Index()
        {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<postANDimage> posts = postingService.getAll();
                Shuffle<postANDimage>(posts);

                string userName = postingService.getUserName(userId);
                ViewBag.UserId = userId;
                ViewBag.UserName = userName;
                ViewBag.profilePath = postingService.getUserProfilePictureFromDb(userId);

                postingService.checkAndAddUserId(userId, userName);

            var Id = userProfilegenericRepo.GetId(userId);
            var userProfilesDetails = userProfilegenericRepo.GetById(Id);
            userProfilesDetails.profilePath = postingService.setPath(userProfilesDetails.profilePath);

            userIndexViewModel indexView;
                indexView = new userIndexViewModel();
                indexView.UserProfile = userProfilesDetails;
                indexView.Posts = posts;
            string key = "welcomeMsg";
            string msg = string.Empty;
            if (HttpContext.Request.Cookies.ContainsKey(key))
            {

                var name = HttpContext.Request.Cookies[key];

                msg = $"Welcome back {name} !";
            }
            else
            {
                HttpContext.Response.Cookies.Append(key, userName);
                msg = $"Welcome {userName} to our Website !";
            }
            ViewBag.cookieMsg = msg;
            ViewBag.DBmsg = TempData["msg"];

            var cpuUsage = _cpuCounter.NextValue();
            var availableMemory = _memoryCounter.NextValue();
            _logger.LogInformation("CPU Usage: {CpuUsage}%", cpuUsage);
            _logger.LogInformation("Available Memory: {AvailableMemory} MB ", availableMemory);

            return View(indexView);
        }
    

        //async
        [HttpPost]
        public async Task<IActionResult> addProfilePicture(IFormFile image)//This method involves file I/O operations
        {
            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "userProfileImage");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var file = image;
            //removing postingRepository line after implement DI
            if (file.Length > 0)
            {
                string filePath = Path.Combine(path, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);//async
                }

                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                postingService.addUserProfilePictureInDb(filePath, userId);
            }

            return RedirectToAction("Index", "posting");
        }

        //async 

        [HttpPost]
        async public Task<IActionResult> AddPost(string publication, List<IFormFile> picture)
        {
            TempData["postUploaded"] = false;
            if (publication!=null && picture!=null)
            {

            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "uploadFiles");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Posting post = new Posting();
            post.userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            post.publication = publication;
            post.postDate = DateTime.Now;

            List<string> images = new List<string>();
                foreach (var file in picture)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(path, file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            images.Add(filePath);
                        }
                    }
                }

                //removing postingRepository line after implement DI PostingRepository postingRepository = new PostingRepository(connectionString);
                postingService.add(post, images);
            TempData["postUploaded"] = true;
            TempData["msg"] = "Post Uploaded Successfully !";
            }

            return RedirectToAction("index", "posting");
        }
        public IActionResult deletePost(int Id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //removing postingRepository line after implement DI PostingRepository postingRepository = new PostingRepository(connectionString);
            
            postingService.deletePost(Id, UserId);
            TempData["postUploaded"] = true;
            TempData["msg"] = "post deleted successfully !";
            return RedirectToAction("index", "posting");
        }

        [HttpPost]
        public IActionResult updatePost(int Id, string publication)
        {
            //removing postingRepository line after implement DI PostingRepository postingRepository = new PostingRepository(connectionString);
           
            postingService.updatePost(Id, publication);
            TempData["postUploaded"] = true;
            TempData["msg"] = "post updated successfully !";
            return RedirectToAction("index", "posting");
            
        }

        public ViewResult editPost(int Id)
        {
            postANDimage postData = new postANDimage();
            
            postData.posting = postingGenericRepo.GetById(Id);

            //removing postingRepository line after implement DI PostingRepository postingRepository = new PostingRepository(connectionString);

            postData.imagePath = postingService.getPostImages(Id);

            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            postData.profilePath = postingService.getUserProfilePictureFromDb(UserId);
            postData.userName = postingService.getUserName(UserId);

            return View(postData);
        }

        public IActionResult viewProfile(int Id)
        {
            // Define a cache key based on the user profile Id
            var cacheKey = $"UserProfile_{Id}";

            // Try to get the user profile from the cache
            if (!_cache.TryGetValue(cacheKey, out userProfile user))
            {
                // If not found in cache, retrieve the profile from the database
                user = userProfile1.UserDeatils(Id);
                user.profilePath = postingService.setPath(user.profilePath);

                // Set cache options (e.g., cache for 30 minutes)
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1)); // Cache for 30 minutes

                // Save data in cache
                _cache.Set(cacheKey, user, cacheEntryOptions);
            }

            // Return the user profile view
            return View(user);
        }


        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "Id" })]//for 5 seconds
        public IActionResult editProfile(int Id)
        {
           
            userProfile userProfiles = userProfile1.UserDeatils(Id);
            userProfiles.profilePath =postingService.setPath(userProfiles.profilePath);
            TempData["ProfileUpdated"] = false;
            return View(userProfiles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult editProfile(int Id, string UserName, string Fname, string Lname, string OrgName, string country, string phone, string DOB, string lives, string depart, string about, string profilePath)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data provided.";
                return RedirectToAction("editProfile", "posting", new { Id });
            }
            userProfile UserProfile = new userProfile();
            UserProfile.Id = Id;
            UserProfile.Fname = Fname;
            UserProfile.Lname = Lname;
            UserProfile.UserName = Fname + Lname;
            UserProfile.OrganizationName = OrgName;
            UserProfile.country = country;
            UserProfile.PhoneNumber = phone;
            UserProfile.DOB = DOB;
            UserProfile.lives = lives;
            UserProfile.department = depart;
            UserProfile.about = about;
            UserProfile.userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserProfile.profilePath = profilePath;

            userProfile1.Add(UserProfile);
            TempData["ProfileUpdated"] = true;
            TempData["msg"] = "Profile Updated Successfully !";
            return RedirectToAction("index", "posting");
        }

        //log performance meterices (profile and trace(timimg issue)
        public IActionResult messages()
        {
            Trace.TraceInformation("Entering messages method at {0}", DateTime.Now);

            var stopwatch = Stopwatch.StartNew(); // Start timing

            _logger.LogInformation("Get messages from database");

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Remove postingRepository
            ViewBag.senderName = postingService.getUserName(userId);
            ViewBag.senderId = userId;
            ViewBag.profilePath = postingService.getUserProfilePictureFromDb(userId);

            List<userProfile> users = userProfilegenericRepo.GetAll().ToList();
            foreach (var i in users)
            {
                i.profilePath = postingService.setPath(i.profilePath);
            }

            // Measure and log CPU and memory usage
            var cpuUsage = _cpuCounter.NextValue();
            var availableMemory = _memoryCounter.NextValue();
            _logger.LogInformation("CPU Usage: {CpuUsage}%", cpuUsage);
            _logger.LogInformation("Available Memory: {AvailableMemory} MB", availableMemory);

            stopwatch.Stop(); // Stop timing
            _logger.LogInformation($"messages method executed in {stopwatch.ElapsedMilliseconds} ms");

            Trace.TraceInformation("Exiting messages method at {0}, Execution Time: {1} ms", DateTime.Now, stopwatch.ElapsedMilliseconds);

            return View(users);
        }


        //async & response cache " Messages between users may not change frequently,
        //so caching them can reduce the load on the database for repeated requests"
        [ResponseCache(Duration = 10)]
        [HttpGet]
        [Route("GetMessages")]
        public async Task<JsonResult> GetMessages([FromQuery] string userId, [FromQuery] string receiverId)
        {
            try
            {
                List<messages> msgs = await postingService.GetMsg(userId, receiverId);
                return Json(msgs);
            }
            catch (Exception ex)
            {
                return Json($"Internal server error: {ex.Message}");
            }
        }

    }
}
