﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimetableCreationTool
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class timetableCreationEntities3 : DbContext
    {
        public timetableCreationEntities3()
            : base("name=timetableCreationEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }
        public virtual DbSet<courseTemp> courseTemps { get; set; }
        public virtual DbSet<lecturerTemp> lecturerTemps { get; set; }
        public virtual DbSet<moduleTemp> moduleTemps { get; set; }
        public virtual DbSet<roomTemp> roomTemps { get; set; }
    }
}
