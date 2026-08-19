using System;

namespace CodeReviewer.Mvc.Models
{
    public class TeacherDashboardItem
    {
        public int Id {get;set;}
        public string CodeText {get;set;} = string.Empty;
        public DateTime SubmissionDate {get;set;}
        public UserDto? Student {get;set;}
        public AssignmentDto? Assignment {get;set;}
    }

    public class UserDto
    {
        public string Name {get;set;} = string.Empty;
        public string Email {get;set;} = string.Empty;
    }

    public class AssignmentDto
    {
        public string Title {get;set;} = string.Empty;
    }
}