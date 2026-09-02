using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivitySystem.Models;
using ActivitySystem.ViewModel;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace ActivitySystem.Data
{
 
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            // Connect
        }

        //Model
        public DbSet<Employee> Employee { get; set; }       //Delete
        public DbSet<Departments> Departments { get; set; } //Delete

        
        public DbSet<Kaizen> Kaizen { get; set; } //New Kaizen
        public DbSet<Library> Library { get; set; } //History
        public DbSet<Process> Process { get; set; } //Activity
        public DbSet<Awards> Awards { get; set; } //Std. Award

        public DbSet<ActType_Choose> ActType_Choose { get; set; } //Choose in Form
        public DbSet<TypeKaizen_Choose> TypeKaizen_Choose { get; set; } //Choose in Form
        public DbSet<LevelKaizen_Choose> LevelKaizen_Choose { get; set; } //Choose in Form
        public DbSet<EnvKaizen_Choose> EnvKaizen_Choose { get; set; } //Choose in Form

        public DbSet<Administrator> Administrator { get; set; } //Admin for input award

        // View Model
        public DbSet<View_ApprovalFlow> View_ApprovalFlow { get; set; } //Ref. Emp. from HR Base
        public DbSet<View_GrpDeptTab> View_GrpDeptTab { get; set; }     //Ref. Dept. from HR Base
        public DbSet<View_Kaizens> View_Kaizens { get; set; }           //Join
        public DbSet<View_Leader> View_Leader { get; set; }             //Get Leader
        public DbSet<View_Manager> View_Manager { get; set; }           //Get Manager
        public DbSet<DueDate> DueDate { get; set; }
    }
}