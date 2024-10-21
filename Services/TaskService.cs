using FAST_API_V2.Extenstions;
using FAST_API_V2.ViewModels;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace FAST_API_V2.Services
{
    public class TaskService : ITaskService
    {
        public async Task<Dictionary<string, object>> ProccessAssignTask(AssignTaskInputVM vm, string connect)
        {
            try
            {
                if (!vm.taskId.IsNullOrEmpty())
                {
                    var task = await SqlService.ReadDataSet(SqlQuery.findTaskById(vm.taskId), connect);
                    if (vm.staffId.IsNullOrEmpty())
                    {
                        if (task[0][0]["STATUS"] == null || Convert.ToInt32(task[0][0]["STATUS"]) == 0)
                        {
                            bool isReturn = (bool)task[0][0]["ISRETURN"];
                            string deadlineOnstring = DateTime.Now.ToString();

                            string deadline = task[0][0]["DEADLINE"].ToString();
                            if (!string.IsNullOrEmpty(deadline))
                            {
                                if (deadline.Contains("ASG"))
                                {
                                    string sign = "";
                                    string addingTime = "";
                                    string addingHours = "";
                                    string addingMinutes = "";
                                    DateTime? deadlineOn = null;
                                    DateTime onlyDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
                                    int hour = DateTime.Now.Hour;

                                    if (deadline.Contains("|"))
                                    {
                                        long nextTimes = 0;
                                        string[] deadlineArr = deadline.Split('|');
                                        if (deadlineArr.Length > 2)
                                        {
                                            try
                                            {
                                                string addingTimeTomorrow = deadlineArr[2];
                                                var addingHoursTomorrow = addingTimeTomorrow.Substring(0, addingTimeTomorrow.IndexOf(":"));
                                                var addingMinutesTomorrow = addingTimeTomorrow.Substring(addingTimeTomorrow.IndexOf(":") + 1);
                                                addingTime = deadlineArr[1];
                                                addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                                addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                                long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                                long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                                long totalMinutesTomorrow = long.Parse(addingMinutesTomorrow) + long.Parse(addingHoursTomorrow) * 60;
                                                if (addingTime != "00:00" && addingTimeTomorrow != "00:00")
                                                {
                                                    if (hour < 12)
                                                    {
                                                        nextTimes = currentTimes + totalMinutes * 60000;
                                                    }
                                                    else
                                                    {
                                                        nextTimes = currentTimes + totalMinutesTomorrow * 60000 + 24 * 60 * 60000;
                                                    }
                                                }
                                                else
                                                {
                                                    if (addingTime == "00:00" && addingTimeTomorrow != "00:00")
                                                    {
                                                        nextTimes = currentTimes + totalMinutesTomorrow * 60000 + 24 * 60 * 60000;
                                                    }
                                                    if (addingTime != "00:00" && addingTimeTomorrow == "00:00")
                                                    {
                                                        nextTimes = currentTimes + totalMinutes * 60000;
                                                    }
                                                }
                                                deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                            }
                                            catch (Exception e)
                                            {
                                                // Handle exception
                                            }
                                        }
                                        else
                                        {
                                            addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                            addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                            addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                            long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                            long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                            if (hour < 12)
                                            {
                                                nextTimes = currentTimes + totalMinutes * 60000;
                                            }
                                            else
                                            {
                                                nextTimes = currentTimes + totalMinutes * 60000 + 24 * 60 * 60000;
                                            }
                                            deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                        }
                                    }

                                    if (deadline.Contains("+"))
                                    {
                                        sign = "+";
                                        addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                        addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                        addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                        long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                        long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                        long nextTimes = currentTimes + totalMinutes * 60000;
                                        deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                    }

                                    if (deadline.Contains("-"))
                                    {
                                        sign = "-";
                                        addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                        addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                        addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                        long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                        long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                        long nextTimes = currentTimes - totalMinutes * 60000;
                                        deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                    }

                                    deadlineOnstring = deadlineOn.ToString();
                                }
                            }
                            if (isReturn)
                            {
                                var update = await SqlService.ExeNonQuery(SqlQuery.updateAssignIsReturnedMobileTask(task[0][0]["idKeyShipment"].ToString()), connect);
                            }
                            else
                            {
                                var updateAssignedMobileTask = await SqlService.ExeNonQuery(SqlQuery.updateAssignedMobileTask(task[0][0]["idKeyShipment"].ToString()), connect);

                            }
                            var updatetask = await SqlService.ExeNonQuery(SqlQuery.UpdateTask(vm.staffId, deadlineOnstring, task[0][0]["idKeyShipment"].ToString()), connect);

                        }
                        else
                        {
                            return new Dictionary<string, object>
                            {
                                { "resutl", "fail" } ,
                                {"message","Can't Assign" }
                            };
                        }
                        var remainAssignTaskList = await SqlService.ReadDataSet(SqlQuery.getRemainAssignTask(task[0][0]["idKeyShipment"].ToString()),connect);
                        if (!remainAssignTaskList[0].IsNullOrEmpty())
                        {
                            foreach(Dictionary<string,object> remainTask in remainAssignTaskList[0])
                            {
                                if (remainTask["STATUS"]==null|| Convert.ToInt32( remainTask["STATUS"]) == 0)
                                {
                                    string deadline = remainTask["Deadline"].ToString();
                                    string deadlineOnstring = DateTime.Now.ToString();
                                    if (!string.IsNullOrEmpty(deadline))
                                    {
                                        if (deadline.Contains("ASG"))
                                        {
                                            string sign = "";
                                            string addingTime = "";
                                            string addingHours = "";
                                            string addingMinutes = "";
                                            DateTime? deadlineOn = null;
                                            DateTime onlyDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
                                            int hour = DateTime.Now.Hour;

                                            if (deadline.Contains("|"))
                                            {
                                                long nextTimes = 0;
                                                string[] deadlineArr = deadline.Split('|');
                                                if (deadlineArr.Length > 2)
                                                {
                                                    try
                                                    {
                                                        string addingTimeTomorrow = deadlineArr[2];
                                                        var addingHoursTomorrow = addingTimeTomorrow.Substring(0, addingTimeTomorrow.IndexOf(":"));
                                                        var addingMinutesTomorrow = addingTimeTomorrow.Substring(addingTimeTomorrow.IndexOf(":") + 1);
                                                        addingTime = deadlineArr[1];
                                                        addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                                        addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                                        long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                                        long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                                        long totalMinutesTomorrow = long.Parse(addingMinutesTomorrow) + long.Parse(addingHoursTomorrow) * 60;
                                                        if (addingTime != "00:00" && addingTimeTomorrow != "00:00")
                                                        {
                                                            if (hour < 12)
                                                            {
                                                                nextTimes = currentTimes + totalMinutes * 60000;
                                                            }
                                                            else
                                                            {
                                                                nextTimes = currentTimes + totalMinutesTomorrow * 60000 + 24 * 60 * 60000;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (addingTime == "00:00" && addingTimeTomorrow != "00:00")
                                                            {
                                                                nextTimes = currentTimes + totalMinutesTomorrow * 60000 + 24 * 60 * 60000;
                                                            }
                                                            if (addingTime != "00:00" && addingTimeTomorrow == "00:00")
                                                            {
                                                                nextTimes = currentTimes + totalMinutes * 60000;
                                                            }
                                                        }
                                                        deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        // Handle exception
                                                    }
                                                }
                                                else
                                                {
                                                    addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                                    addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                                    addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                                    long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                                    long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                                    if (hour < 12)
                                                    {
                                                        nextTimes = currentTimes + totalMinutes * 60000;
                                                    }
                                                    else
                                                    {
                                                        nextTimes = currentTimes + totalMinutes * 60000 + 24 * 60 * 60000;
                                                    }
                                                    deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                                }
                                            }

                                            if (deadline.Contains("+"))
                                            {
                                                sign = "+";
                                                addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                                addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                                addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                                long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                                long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                                long nextTimes = currentTimes + totalMinutes * 60000;
                                                deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                            }

                                            if (deadline.Contains("-"))
                                            {
                                                sign = "-";
                                                addingTime = deadline.Substring(deadline.IndexOf(sign) + 1);
                                                addingHours = addingTime.Substring(0, addingTime.IndexOf(":"));
                                                addingMinutes = addingTime.Substring(addingTime.IndexOf(":") + 1);
                                                long currentTimes = (long)(onlyDate - DateTime.MinValue).TotalMilliseconds;
                                                long totalMinutes = long.Parse(addingMinutes) + long.Parse(addingHours) * 60;
                                                long nextTimes = currentTimes - totalMinutes * 60000;
                                                deadlineOn = DateTime.MinValue.AddMilliseconds(nextTimes);
                                            }

                                            deadlineOnstring = deadlineOn.ToString();
                                        }
                                    }
                                    var update = await SqlService.ExeNonQuery(SqlQuery.UpdateTask2(vm.staffId, deadlineOnstring, task[0][0]["idKeyShipment"].ToString(), vm.contactId), connect);
                                    return new Dictionary<string, object>
                                    {
                                        { "resutl", "success" } ,
                                        {"message","Assign Complete" }
                                    };
                                }
                                else
                                {
                                    return new Dictionary<string, object>
                                    {
                                        { "resutl", "fail" } ,
                                        {"message","Can't Assign" }
                                    };
                                }
                            }
                        }
                        Dictionary<string, object> dataAssignTaskNotification = new Dictionary<string, object>
                        {
                            {"title","New Task" },
                            { "body", "You have a new task"},
                            {"contactId", vm.staffId },
                            {"taskId", vm.taskId },
                            {"type", "new-task" }
                        };
                        Dictionary<string, object> notifidJsonIOS = new Dictionary<string, object>
                        {
                            {"sound", "default" },
                            {"click_action", "com.adobe.phonegap.push.background.MESSAGING_EVENT" },
                            { "title", "New Task"},
                            {"body", "You have a new task" }
                        };
                        Dictionary<string, object> jsonAssignTaskNotification = new Dictionary<string, object>
                        {
                            {"condition","'logistics' in topics && "+ "'" + vm.staffId + "' in topics" },
                            {"data",dataAssignTaskNotification },
                            {"notification", notifidJsonIOS }
                        };
                        return jsonAssignTaskNotification;
                    }
                    else
                    {
                        var updateunassignTask = await SqlService.ExeNonQuery(SqlQuery.UpdateUnassignTask(vm.taskId), connect);
                        return new Dictionary<string, object>
                        {
                            { "Resutl", "Success" } ,
                            {"message","Unassign Complete" }
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "error", ex.Message },
                    { "inputObject",""}
                };
            }


           
        }

        public async Task<Dictionary<string, object>> ProccessGetDetailTask(TaskIdInputV vm, string connect)
        {

            try
            {
                var detailTask = await SqlService.ReadDataSet(SqlQuery.getDetailTask(vm.taskId), connect);
                string hblNo = detailTask[0][0]["hwbno"].ToString();
                var attachedFileListByHBLNo = await SqlService.ReadDataSet(SqlQuery.getInfoTransactionInfoDetail(hblNo), connect);
                List<Dictionary<string, object>> jsonFileList = new List<Dictionary<string, object>>();
                if (attachedFileListByHBLNo[0] != null)
                {
                    foreach (var attachedFile in attachedFileListByHBLNo[0])
                    {
                        Dictionary<string, object> file = new Dictionary<string, object>
                    {
                        {"fieldKey",attachedFile["FieldKey"].ToString()},
                        {"fileName",attachedFile["FileName"].ToString()+"."+attachedFile["FileExt"].ToString()  }
                    };
                        jsonFileList.Add(file);
                    }
                }
                else
                {
                    var attachedFileListByIDKeyShipment = await SqlService.ReadDataSet(SqlQuery.getInfoTransactionInfoDetailByIDKeyShipment(detailTask[0][0]["IDKEYSHIPMENT"].ToString()), connect);
                    if (attachedFileListByIDKeyShipment[0] != null)
                    {
                        foreach (var attachedFile in attachedFileListByIDKeyShipment[0])
                        {
                            Dictionary<string, object> file = new Dictionary<string, object>
                    {
                        {"fieldKey",attachedFile["FieldKey"].ToString()},
                        {"fileName",attachedFile["FileName"].ToString()+"."+attachedFile["FileExt"].ToString()  }
                    };
                            jsonFileList.Add(file);
                        }
                    }
                }
                detailTask[0][0].Add("attachedFile", jsonFileList);
                return detailTask[0][0];
                
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    {"error",ex.Message },
                    {"inputObject","" }
                };
            }

        }

        public async Task<Dictionary<string, object>> ProccessGetMyTask(GetMyTaskInputVM vm, string connect)
        {
            
            string firstDay = "";
            string lastDay = "";
            if(vm.isFinished)
            {
                firstDay = vm.firstDay;
                lastDay =vm.lastDay;
                lastDay += " 23:59:59";
            }
            var listTask = await SqlService.ReadDataSet(SqlQuery.getAssignedTaskInPeriod(firstDay,lastDay,vm.contactId,vm.isFinished,vm.isMobile,vm.jobId,vm.cdsNo), connect);
            var reponse = new Dictionary<string, object>
            {
                {"taskJsonList",listTask[0] }
            };
            
            return reponse;
        }

        public async Task<Dictionary<string, object>> ProccessGetUnassignedTask(ContactIdAndIsMobileInputVM vm, string connect)
        {
            var unsigedListTask = await SqlService.ReadDataSet(SqlQuery.getUnAssignedTask(vm.contactId, vm.isMobile), connect);
            var reponse = new Dictionary<string, object>();

            reponse.Add("taskJsonList", unsigedListTask[0]);
            return reponse;
        }
    }
}
