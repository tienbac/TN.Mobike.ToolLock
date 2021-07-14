using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;
using Quartz.Impl;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock.Core
{
    class JobOpenLock
    {
        private static IScheduler scheduler;
        public static RichTextBox RichTextBox;
        public static void Start()
        {
            try
            {
                RichTextBox = Form1.RtbMessage;

                scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                //Job capture frames

                IJobDetail job = JobBuilder.Create<OpenAuto>().Build();
                ITrigger restartTrigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithDailyTimeIntervalSchedule
                    (s =>
                        s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(6, 30, 00))
                    )
                    //.WithSimpleSchedule(x => x
                    //    .WithIntervalInSeconds(10000)
                    //    .RepeatForever())
                    .Build();
                scheduler.ScheduleJob(job, restartTrigger);

            }
            catch (Exception ex)
            {
                Utilities.WriteErrorLog("JobRePushScheduler_Start", ex.ToString());
            }
        }

        public static void Stop()
        {
            try
            {
                scheduler?.Shutdown();
            }
            catch (Exception ex)
            {
                Utilities.WriteErrorLog("JobRePushScheduler_Stop", ex.ToString());
            }
        }
    }

    class OpenAuto : IJob
    {
        public static bool IsRunning = false;
        public void Execute(IJobExecutionContext context)
        {
            try
            {

                if (IsRunning)
                {
                    return;
                }

                IsRunning = true;

                List<string> list = new List<string>()
                {
                    "861123053530521","861123053529176","861123053526958","861123053783187","861123053757108","861123053783161","861123053757694"
                };

                foreach (var imei in list)
                {
                    MinaControl.UnLock(JobOpenLock.RichTextBox, "L0", imei, "", true);
                    Thread.Sleep(2000);
                }

                IsRunning = false;
            }
            catch (Exception e)
            {
                IsRunning = false;
                Utilities.WriteErrorLog("[ OpenAuto.Execute ]", $"[ ERROR: {e} ]");
            }
        }
    }
}
