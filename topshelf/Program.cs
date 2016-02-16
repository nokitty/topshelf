using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Topshelf;

using Quartz;
using Quartz.Impl;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace topshelf
{
    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>                                 //1
            {
                x.Service<Scheduler>();
                x.RunAsLocalSystem();                            //6                

                x.SetDescription("测试服务描述");        //7
                x.SetDisplayName("Stuff");                       //8
                x.SetServiceName("Stuff");                       //9
            });                                                  //10
        }
    }

    public class Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var log = log4net.LogManager.GetLogger("loger");
            log.Debug(string.Format("It is {0} and all is well", DateTime.Now));
        }
    }

    public class Scheduler:ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            CreateJob(typeof(Job), SimpleScheduleBuilder.RepeatSecondlyForever(1), scheduler);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }

        void CreateJob(Type jobType, IScheduleBuilder scheduleBuider, IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create(jobType)
                .Build();

            var trigger = TriggerBuilder.Create()
                 .StartNow();

            if (scheduleBuider != null)
                trigger.WithSchedule(scheduleBuider);

            scheduler.ScheduleJob(job, trigger.Build());
        }
    }
}
