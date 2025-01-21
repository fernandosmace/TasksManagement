﻿namespace TasksManagement.API.Models.OutputModels.Task;

public class TaskReportModel
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public DateTime CompletionDate { get; set; }
    public int CommentCount { get; set; }
}