using BusinessObject.Data;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public int Get()
        {
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
            Context context = new Context();

            Group group = new Group();
            group.Name = "a";
            group.CreatedDate = DateTime.Now;
            group.GroupSize = 10;
            group.MemberCount = 1;
            group.SchoolName = "b";
            group.ClassName = "c";
            group.SubjectName = "d";
            group.Description = "e";
            group.Skill = "f";
            group.Status = GroupStatus.ACTIVE;
            group.CurrentMilestone = null;

            context.Add(group);
            context.SaveChanges();

            Milestone milestone = new Milestone();
            milestone.Name = "a";
            milestone.Description = "b";
            milestone.Order = 1;
            milestone.GroupId = context.Groups.FirstOrDefault(g => g.Name == "a").Id;

            context.Add(milestone);
            context.SaveChanges();

            Group group1 = context.Groups.FirstOrDefault(g => g.Name == "a");
            group1.CurrentMilestoneId = context.Milestones.FirstOrDefault(m => m.Name == "a").Id;

            context.Update(group1);
            context.SaveChanges();

            Group group2 = context.Groups.Include(g => g.CurrentMilestone).FirstOrDefault(g => g.Name == "a");
            return group2.CurrentMilestone.Order;
        }
    }
}