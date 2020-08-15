﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Proffy.Business.POCO;
using Proffy.RepositoryEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proffy.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherInfoController : ControllerBase
    {
        public interface IRepository
        {
            void Add<T>(T entity) where T : class;
            void Update<T>(T entity) where T : class;
            void Delete<T>(T entity) where T : class;
            Task<bool> SaveChangesAsync();
        }

        public class EFCoreRepository : IRepository
        {
            private readonly ProffyContext context;

            public EFCoreRepository(ProffyContext context)
            {
                this.context = context;
            }
            public void Add<T>(T entity) where T : class
            {
                context.Add(entity);
            }

            public void Delete<T>(T entity) where T : class
            {
                context.Remove(entity);
            }

            public void Update<T>(T entity) where T : class
            {
                context.Update(entity);
            }

            public async Task<bool> SaveChangesAsync()
            {
                return await context.SaveChangesAsync() > 0;
            }
        }

        int ConvertHourToMinutes(string time)
        {
            int hourInMinutes = time.Split(':')
                                    .Select(c => int.Parse(c))
                                    .Aggregate((hour, minute) => (hour * 60) + minute);
            return hourInMinutes;
        }

        public class LessonScheduleDTO
        {
            public int WeekDay { get; set; }
            public string From { get; set; }
            public string To { get; set; }
        }
        public class LessonFilterDTO
        {
            public string Subject { get; set; }
            public int WeekDay { get; set; }
            public string Time { get; set; }
        }

        public class TeacherInfoDTO
        {
            public string Name { get; set; }
            public string Avatar { get; set; }
            public string WhatsApp { get; set; }
            public string Bio { get; set; }
            public string Subject { get; set; }
            public decimal Cost { get; set; }
            public List<LessonScheduleDTO> Schedule { get; set; }
        }

        public readonly ProffyContext context;
        public TeacherInfoController(ProffyContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] LessonFilterDTO filter)
        {
            var time = ConvertHourToMinutes(filter.Time);
            var teachers = context.Lesson
                .Where(c => c.Subject == filter.Subject &&
                            c.LessonSchedule.WeekDay == filter.WeekDay &&
                            c.LessonSchedule.From <= time && c.LessonSchedule.To > time
                )
                .Select(c => new
                {
                    c.TeacherID,
                    c.Subject,
                    c.Cost,
                    c.Teacher.Name,
                    c.Teacher.Avatar,
                    c.Teacher.WhatsApp,
                    c.Teacher.Bio
                })
                .ToList();

            return Ok(teachers);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TeacherInfoDTO teacherInfoDTO)
        {
            // using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var objTeacher = new Teacher()
                    {
                        Name = teacherInfoDTO.Name,
                        Avatar = teacherInfoDTO.Avatar,
                        Bio = teacherInfoDTO.Bio,
                        WhatsApp = teacherInfoDTO.WhatsApp
                    };
                    context.Add(objTeacher);
                    context.SaveChanges();

                    var objLesson = new Lesson()
                    {
                        Subject = teacherInfoDTO.Subject,
                        Cost = teacherInfoDTO.Cost,
                        Teacher = objTeacher
                    };
                    context.Add(objLesson);
                    context.SaveChanges();

                    // var objLstLessonSchedule = new List<LessonSchedule>();
                    foreach (var LessonScheduleItem in teacherInfoDTO.Schedule)
                    {
                        var classSchedulePOCO = new LessonSchedule()
                        {
                            To = ConvertHourToMinutes(LessonScheduleItem.To),
                            From = ConvertHourToMinutes(LessonScheduleItem.From),
                            WeekDay = LessonScheduleItem.WeekDay,
                            Lesson = objLesson
                        };
                        context.Add(classSchedulePOCO);
                        context.SaveChanges();
                        // objLstLessonSchedule.Add(classSchedulePOCO);
                    }
                    // context.AddRange(objLstLessonSchedule);

                    context.SaveChanges();
                    // transaction.Commit();
                    return Ok("TeacherInfo createad successfully");
                }
                catch (Exception ex)
                {
                    // transaction.Rollback();
                    return BadRequest($"Erro: {ex}");
                }
            }
        }
    }
}

