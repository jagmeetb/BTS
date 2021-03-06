﻿/*
The following functions were coded by Jagmeet:
LoadData()
loadDataInstructor()
loadDataCourse()
loadDataStudents())
loadDataIncidents()

IncidentEdit()
IncidentEditForm()

The following functions were coded by Mona:
IncidentGetAll()
IncidentGetOne()
IncidentGetById()

The following functions were coded by Shawn:
StudentSearch()

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
// new...
using AutoMapper;
using BTS.Models;
using System.Security.Claims;
using System.Net;
using System.IO;
using System.Web.UI.WebControls;


namespace BTS.Controllers
{
	public class Manager
	{
		// Reference to the data context
		private ApplicationDbContext ds = new ApplicationDbContext();

		// Declare a property to hold the user account for the current request
		// Can use this property here in the Manager class to control logic and flow
		// Can also use this property in a controller 
		// Can also use this property in a view; for best results, 
		// near the top of the view, add this statement:
		// var userAccount = new ConditionalMenu.Controllers.UserAccount(User as System.Security.Claims.ClaimsPrincipal);
		// Then, you can use "userAccount" anywhere in the view to render content
		public UserAccount UserAccount { get; private set; }

		public Manager()
		{
			// If necessary, add constructor code here

			// Initialize the UserAccount property
			UserAccount = new UserAccount(HttpContext.Current.User as ClaimsPrincipal);

			// Turn off the Entity Framework (EF) proxy creation features
			// We do NOT want the EF to track changes - we'll do that ourselves
			ds.Configuration.ProxyCreationEnabled = false;

			// Also, turn off lazy loading...
			// We want to retain control over fetching related objects
			ds.Configuration.LazyLoadingEnabled = false;
		}

		// ############################################################
		// RoleClaim

		public List<string> RoleClaimGetAllStrings()
		{
			return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
		}

		// Add methods below
		// Controllers will call these methods
		// Ensure that the methods accept and deliver ONLY view model objects and collections
		// The collection return type is almost always IEnumerable<T>

		// Suggested naming convention: Entity + task/action
		// For example:
		// ProductGetAll()
		// ProductGetById()
		// ProductAdd()
		// ProductEdit()
		// ProductDelete()
        
		// Add some programmatically-generated objects to the data store
		// Can write one method, or many methods - your decision
		// The important idea is that you check for existing data first
		// Call this method from a controller action/method

		public bool LoadData()
		{/*
			// User name
			var user = HttpContext.Current.User.Identity.Name;

			// Monitor the progress
			bool done = false;

			// ############################################################
			// Genre

			if (ds.RoleClaims.Count() == 0)
			{
				// Add role claims

				//ds.SaveChanges();
				//done = true;
			}*/
      //    RemoveDatabase();
       //     RemoveData();
       
            if (loadDataInstructor())
            {
                if (loadDataCourse())
                {
                    if (loadDataStudents()){
                        loadDataIncidents();
                    }
                }
            }

            ds.SaveChanges();
			return true;
		}

        public bool loadDataCourse()
        {
            if (ds.Courses.Count() > 0) { return false; }

            var eden = ds.Instructors.SingleOrDefault(a => a.name == "Eden Burton");

            ds.Courses.Add(new Course
            {
                courseName = "Major Project: Planning & Design",
                courseCode = "BTS530",
                sectionId = "A",
                semester = "Fall 2016",
                Instructor = eden
            });

            ds.SaveChanges();

            var BTS = ds.Courses.SingleOrDefault(a => a.courseCode == "BTS530");
            eden.Courses = new List<Course> { BTS };

            ds.SaveChanges();
            return true;
        }

        public bool loadDataInstructor()
        {
            if (ds.Instructors.Count() > 0) { return false; }

            ds.Instructors.Add(new Instructor
            {
                name = "Eden Burton",
                emailAddress = "senecafaculty@gmail.com"
            });

            ds.SaveChanges();
            return true;
        }

        public bool loadDataStudents()
        {
            if (ds.Students.Count() > 0) { return false; }
            var BTS = ds.Courses.SingleOrDefault(a => a.courseCode == "BTS530");

            ds.Students.Add(new Student
            {
                Courses = new List<Course> { BTS },
                name = "Jagmeet Bhamber",
                emailAddress = "jsbhamber2@myseneca.ca",
                year = "Fall 2016",
                studentId = "022967145"
            });

            ds.Students.Add(new Student
            {
                Courses = new List<Course> { BTS },
                name = "Shawn Matthew",
                emailAddress = "sjmatthew@myseneca.ca",
                year = "Fall 2016",
                studentId = "069669142"
            });

            ds.Students.Add(new Student
            {
                Courses = new List<Course> { BTS },
                name = "Mona Alkhulaqi",
                emailAddress = "msalkhulaqi@myseneca.ca",
                year = "Fall 2016",
                studentId = "016081085"
            });

            ds.SaveChanges();

            BTS = ds.Courses.SingleOrDefault(a => a.courseCode == "BTS530");
            var jagmeet = ds.Students.SingleOrDefault(a => a.studentId == "022967145");
            var mona = ds.Students.SingleOrDefault(a => a.studentId == "016081085");
            var shawn = ds.Students.SingleOrDefault(a => a.studentId == "069669142");
            BTS.Students = new List<Student> { jagmeet, mona, shawn };

            ds.SaveChanges();
            return true;
        }
        
        public bool loadDataIncidents()
        {
            if (ds.Incidents.Count() > 0) { return false; }

            var eden = ds.Instructors.SingleOrDefault(a => a.name == "Eden Burton");
            var jagmeet = ds.Students.SingleOrDefault(a => a.studentId == "022967145");
            var mona = ds.Students.SingleOrDefault(a => a.studentId == "016081085");
            var shawn = ds.Students.SingleOrDefault(a => a.studentId == "069669142");

            ds.Incidents.Add(new Incident
            {
                dateReported = DateTime.Now,
                description = "plagiarism",
                status = "open",
                Students = new List<Student> { jagmeet, shawn },
                Instructor = eden,
                program = "BSD",
                campus = "Seneca@York",
                offence = "minor"
            });

            ds.SaveChanges();

            var plag = ds.Incidents.SingleOrDefault(a => a.description == "plagiarism");

            eden = ds.Instructors.SingleOrDefault(b => b.name == "Eden Burton");
            jagmeet = ds.Students.SingleOrDefault(c => c.studentId == "022967145");

            eden.Incidents = new List<Incident> { plag };
            jagmeet.Incidents = new List<Incident> { plag };

            ds.SaveChanges();
            return true;
        }

        public bool RemoveData()
		{
			try
			{
				foreach (var e in ds.RoleClaims)
				{
					ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
				}
				ds.SaveChanges();

                foreach (var e in ds.Courses)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Instructors)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Students)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Incidents)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool RemoveDatabase()
		{
			try
			{
				return ds.Database.Delete();
			}
			catch (Exception)
			{
				return false;
			}
		}
        //******************************************************************************************************************************//
        //******************************************************************************************************************************//
        public IncidentDocs IncidentDocGetById(int id)
        {
            var o = ds.Incidents.Find(id);

            IncidentDocs x = new IncidentDocs();
            x.Doc = o.Doc;
            x.DocContentType = o.DocContentType;

            if (x.Doc == null)
            {
                return null;
            }
            else
            {
                return x;
            }
        }

        public bool closeIncident(int id, string message)
        {

            var o = ds.Incidents.Include("Instructor").Include("Students").SingleOrDefault(a => a.Id == id);
            var p = ds.Instructors.Find(o.Instructor.Id);
            if (o != null)
            {
                o.status = "closed";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                MailMessage msg = new MailMessage();

                msg.To.Add(p.emailAddress);
                msg.Subject = "Seneca Academic Honesty Notice";
                string body = "Hello " + p.name;
                body += "\n\n";
                body += "\tYour Seneca Academic Honesty report for \"" + o.description + "\" has been closed.";
                body += "\nHere is the report:";
                body += "\n\n" + message;
                msg.Body = body;
                smtpClient.Send(msg);

                ds.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }


        public IncidentWithDetails IncidentAdd(IncidentAdd newItem)
        {
            ICollection<Student> mystudent = new List<Student>();
            foreach (var x in newItem.StudentId)
            {
                if (x != "")
                {
                    mystudent.Add(ds.Students.Include("Incidents").SingleOrDefault(s => s.studentId == x));
                }
            }

            //= ds.Students.SingleOrDefault(s => s.studentId == newItem.StudentId);
            var instruct = ds.Instructors.SingleOrDefault(e => e.name == newItem.InstructorName);
            var myCourse = ds.Courses.SingleOrDefault(c => c.courseCode == newItem.coursecode);
            //var addedItem = ds.Incidents.Add(Mapper.Map<Incident>(newItem));
            var first = mystudent.First();

            // var streamLength = newItem.DocUpload.InputStream.Length;
            // var fileBytes  = new byte[streamLength];
            // newItem.DocUpload.InputStream.Read(fileBytes, 0, fileBytes.Length);



            //one of them wasn't found from Database

            for (int i = 0; i < mystudent.Count(); i++)
            {
                if (mystudent.ElementAt(i).name != newItem.StudentName.ElementAt(i))
                {
                    return null;
                }
            }

            if (mystudent == null || instruct == null || myCourse == null)
            {
                return null;
            }
            //student name + number dont match what is on Database


            else
            {             //create incident
                Incident incident = new Incident();
                incident.dateReported = newItem.IncidentDate;
                incident.description = newItem.description;
                incident.Instructor = instruct;
                foreach (var x in mystudent)
                {
                    incident.Students.Add(x);
                }
                //                incident.Doc = docBytes;
                //                incident.DocContentType = newItem.DocUpload.ContentType;

                incident.status = "open";
                incident.campus = newItem.campus;
                incident.program = newItem.program;
                if (newItem.isMinor)
                {
                    
                    bool checkall = false;
                    foreach (var a in mystudent)
                    {
                        if (a.Incidents.Count() > 0)
                        {
                            checkall = true;
                        }
                    }

                    if (!checkall)
                    {
                        incident.offence = "Minor";
                    }
                    else
                    {
                        incident.offence = "Minor (Repeat)";
                    }
                }
                else
                {
                    incident.offence = "Major";
                }


                byte[] docBytes = null;

                if (newItem.DocUpload != null)
                {

                    docBytes = new byte[newItem.DocUpload.ContentLength];
                    newItem.DocUpload.InputStream.Read(docBytes, 0, newItem.DocUpload.ContentLength);

                    // Then, configure the new object's properties
                    incident.Doc = docBytes;
                    incident.DocContentType = newItem.DocUpload.ContentType;
                }


                ds.Incidents.Add(incident);

                ds.SaveChanges();

                //After saving in DB send email
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                MailMessage msg = new MailMessage();

                string body;

                foreach (var x in mystudent)
                {
                    msg.To.Clear();
                    msg.To.Add(x.emailAddress);
                    msg.Subject = "Seneca Academic Honesty Notice";
                    body = "Hello " + x.name;
                    body += "\n\n";
                    body += "\tYour Seneca Academic Honesty record has been updated. Please log into the site to check it.";
                    msg.Body = body;
                    smtpClient.Send(msg);
                }


                msg = new MailMessage();
                msg.To.Add(first.emailAddress);
                msg.Subject = "Seneca Academic Honesty Notice";
                body = "Hello " + instruct.name;
                body += "\n\n";
                body += "\tYour Seneca Academic Honesty Report has been successfully recorded to our database.";
                msg.Body = body;
                smtpClient.Send(msg);

                msg = new MailMessage();
                msg.To.Add(first.emailAddress);
                msg.Subject = "Seneca Academic Honesty Notice";
                body = "Hello Academic Honesty Chair of " + incident.campus;
                body += "\n\n";
                body += "\tAn Academic Honesty report has recently been submitted. Please login to the site to check it.";
                msg.Body = body;
                smtpClient.Send(msg);

                return Mapper.Map<IncidentWithDetails>(incident);
            }
        }
        // ############################################################


        // ############################################################
        public IEnumerable<IncidentBase> IncidentGetAll()
        {
            if (UserAccount.HasRoleClaim("Faculty"))
            {
                var inst = ds.Instructors.Where(a => a.name == (UserAccount.GivenName + " " + UserAccount.Surname));
                var b = Mapper.Map<IEnumerable<IncidentBase>>(inst.First().Incidents);
                return b;
            }
            else if (UserAccount.HasRoleClaim("Student"))
            {
                var stud = ds.Students.Where(a => a.name == (UserAccount.GivenName + " " + UserAccount.Surname));
                var b = Mapper.Map<IEnumerable<IncidentBase>>(stud.First().Incidents);
                return b;
            }

            var x = Mapper.Map<IEnumerable<IncidentBase>>(ds.Incidents);
            return x;
        }

        public IEnumerable<InstructorBase> InstructorGetAll()
        {
            var x = Mapper.Map<IEnumerable<InstructorBase>>(ds.Instructors);
            return x;
        }

        public IEnumerable<StudentBase> StudentGetAll()
        {
            var x = Mapper.Map<IEnumerable<StudentBase>>(ds.Students);
            return x;
        }

        public IEnumerable<IncidentBase> IncidentGetAllOpen()
        {

            var babcadv = Mapper.Map<IEnumerable<Incident>>(ds.Incidents);
            var w = Mapper.Map<IEnumerable<IncidentBase>>(ds.Incidents);
            var x = w.Where(a => a.offence.ToLower() == "minor");
            var y = w.Where(a => a.status.ToLower() == "open");

            IEnumerable<IncidentBase> z = Mapper.Map<IEnumerable<IncidentBase>>(y);
            return z;
        }

        // ############################################################
        public IncidentWithDetails IncidentGetOne(int id)
        {
            var o = ds.Incidents.Include("Instructor").Include("Students").SingleOrDefault(a => a.Id == id);

            var p = Mapper.Map<IncidentWithDetails>(o);
            //p.IncidentDoc = File(o.Doc, o.DocContentType);
            if (o.Doc == null)
            {
                p.IncidentDoc = null;
            }
            else
            {
                p.IncidentDoc = $"/file/{id}";
            }

            if (p == null)
            {
                return null;
            }
            else
            {
                return p;
            }

            //return (o == null) ? null : Mapper.Map<IncidentWithDetails>(o);
        }
        // ############################################################
        public IncidentBase IncidentGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Incidents.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : Mapper.Map<IncidentBase>(o);
        }
        // ############################################################
        public IncidentWithDetails IncidentEditForm(IncidentEditForm newItem)
        {
            // Attempt to fetch the object
            var o = ds.Incidents.Find(newItem.InstructorId);
            o = ds.Incidents.Include("Students").Include("Instructor").SingleOrDefault(a => a.Id == newItem.id);

            if (o == null)
            {
                // Problem - item was not found, so return
                return null;
            }
            else
            {
                // Update the object with the incoming values
                ds.Entry(o).CurrentValues.SetValues(newItem);
                ds.SaveChanges();

                // Prepare and return the object
                return Mapper.Map<IncidentWithDetails>(o);
            }
        }
        // ############################################################
        public IEnumerable<StudentBase> StudentSearch(StudentSearch newItem)
        {
           // var o = ds.Students.SingleOrDefault(a => a.name == newItem.searchTerm);
            var o = ds.Students.Where(a => a.name.Contains(newItem.searchTerm));
            if (o == null)
            {
                return null;
            }
            else
            {
                IEnumerable<StudentBase> x = Mapper.Map<IEnumerable<StudentBase>>(o);
                return x;
            }
        }

        public IEnumerable<IncidentBase> IncidentSearch(IncidentSearch newItem)
        {
            var o = ds.Incidents.Where(a => a.description.Contains(newItem.searchTerm));
            var p = ds.Incidents.Where(a => a.Instructor.name.Contains(newItem.searchTerm));
            var q = ds.Students.Where(a => a.name.Contains(newItem.searchTerm));
            o = o.Concat(p);

            if (o == null)
            {
                return null;
            }
            else
            {
                IEnumerable<IncidentBase> x = Mapper.Map<IEnumerable<IncidentBase>>(o);
                return x;
            }


        }

        // ############################################################
        public IncidentWithDetails IncidentEdit(IncidentEdit newItem)
        {
            var o = ds.Incidents.Include("Students").Include("Instructor").SingleOrDefault(a => a.Id == newItem.Id);

            if (o == null)
            {
                return null;
            }
            else
            {
                //update
                o.Students.Clear();

                o.description = newItem.description;
                o.campus = newItem.campus;
                o.program = newItem.program;

                
                if (newItem.DocUpload != null)
                {
                    byte[] docBytes = null;
                    docBytes = new byte[newItem.DocUpload.ContentLength];
                    newItem.DocUpload.InputStream.Read(docBytes, 0, newItem.DocUpload.ContentLength);

                    // Then, configure the new object's properties
                    o.Doc = docBytes;
                    o.DocContentType = newItem.DocUpload.ContentType;
                }

                if (newItem.status.ToLower() == "open" || newItem.status.ToLower() == "closed")
                {
                    o.status = newItem.status;
                }

                foreach (var tuple in newItem.StudentIds.Zip(newItem.StudentNames, Tuple.Create))
                {
                    var a = ds.Students.SingleOrDefault(x => x.studentId == tuple.Item1);
                    if (a != null)
                    {
                        if (a.name == tuple.Item2)
                        {
                            o.Students.Add(a);
                        }
                    }
                }
                /*
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                MailMessage msg = new MailMessage();
                string body;

                Instructor myInstructor;
                myInstructor = ds.Instructors.SingleOrDefault(i => i.Id == newItem.Id);

                if(myInstructor != null)
                {
                    msg.To.Add(myInstructor.emailAddress);
                    msg.Subject = "Seneca Academic Honesty Notice";
                    body = "Hello Professor " + myInstructor.name;
                    body += "\n\n";
                    body += "\tThe student has lost marks";
                    body += "\n\n";
                    body += "\tCase has been closed";

                    msg.Body = body;

                    smtpClient.Send(msg);
                }

                newItem.status = "closed";
                */

                ds.SaveChanges();
                return Mapper.Map<IncidentWithDetails>(o);
            }
        }
    }
    //******************************************************************************************************************************//
    //******************************************************************************************************************************//

    // New "UserAccount" class for the authenticated user
    // Includes many convenient members to make it easier to render user account info
    // Study the properties and methods, and think about how you could use it
    public class UserAccount
	{
		// Constructor, pass in the security principal
		public UserAccount(ClaimsPrincipal user)
		{
			if (HttpContext.Current.Request.IsAuthenticated)
			{
				Principal = user;

				// Extract the role claims
				RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

				// User name
				Name = user.Identity.Name;

				// Extract the given name(s); if null or empty, then set an initial value
				string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
				if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
				GivenName = gn;

				// Extract the surname; if null or empty, then set an initial value
				string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
				if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
				Surname = sn;

				IsAuthenticated = true;
				IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
			}
			else
			{
				RoleClaims = new List<string>();
				Name = "anonymous";
				GivenName = "Unauthenticated";
				Surname = "Anonymous";
				IsAuthenticated = false;
				IsAdmin = false;
			}

			NamesFirstLast = $"{GivenName} {Surname}";
			NamesLastFirst = $"{Surname}, {GivenName}";
		}

		// Public properties
		public ClaimsPrincipal Principal { get; private set; }
		public IEnumerable<string> RoleClaims { get; private set; }

		public string Name { get; set; }

		public string GivenName { get; private set; }
		public string Surname { get; private set; }

		public string NamesFirstLast { get; private set; }
		public string NamesLastFirst { get; private set; }

		public bool IsAuthenticated { get; private set; }

		// Add other role-checking properties here as needed
		public bool IsAdmin { get; private set; }

		public bool HasRoleClaim(string value)
		{
			if (!IsAuthenticated) { return false; }
			return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
		}

		public bool HasClaim(string type, string value)
		{
			if (!IsAuthenticated) { return false; }
			return Principal.HasClaim(type, value) ? true : false;
		}
	}

}