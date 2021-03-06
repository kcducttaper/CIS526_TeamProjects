namespace CIS726_Assignment2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DegreePlansAndMoreMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        planName = c.String(nullable: false),
                        degreeProgramID = c.Int(nullable: false),
                        userID = c.Int(nullable: false),
                        semesterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DegreePrograms", t => t.degreeProgramID, cascadeDelete: false)
                .ForeignKey("dbo.Semesters", t => t.semesterID, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.userID, cascadeDelete: true)
                .Index(t => t.degreeProgramID)
                .Index(t => t.semesterID)
                .Index(t => t.userID);
            
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        semesterTitle = c.String(nullable: false),
                        semesterYear = c.Int(nullable: false),
                        standard = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PlanCourses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        notes = c.String(),
                        planID = c.Int(nullable: false),
                        courseID = c.Int(),
                        electiveListID = c.Int(),
                        semesterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.courseID)
                .ForeignKey("dbo.ElectiveLists", t => t.electiveListID)
                .ForeignKey("dbo.Semesters", t => t.semesterID, cascadeDelete: false)
                .ForeignKey("dbo.Plans", t => t.planID, cascadeDelete: true)
                .Index(t => t.courseID)
                .Index(t => t.electiveListID)
                .Index(t => t.semesterID)
                .Index(t => t.planID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlanCourses", new[] { "planID" });
            DropIndex("dbo.PlanCourses", new[] { "semesterID" });
            DropIndex("dbo.PlanCourses", new[] { "electiveListID" });
            DropIndex("dbo.PlanCourses", new[] { "courseID" });
            DropIndex("dbo.Plans", new[] { "userID" });
            DropIndex("dbo.Plans", new[] { "semesterID" });
            DropIndex("dbo.Plans", new[] { "degreeProgramID" });
            DropForeignKey("dbo.PlanCourses", "planID", "dbo.Plans");
            DropForeignKey("dbo.PlanCourses", "semesterID", "dbo.Semesters");
            DropForeignKey("dbo.PlanCourses", "electiveListID", "dbo.ElectiveLists");
            DropForeignKey("dbo.PlanCourses", "courseID", "dbo.Courses");
            DropForeignKey("dbo.Plans", "userID", "dbo.Users");
            DropForeignKey("dbo.Plans", "semesterID", "dbo.Semesters");
            DropForeignKey("dbo.Plans", "degreeProgramID", "dbo.DegreePrograms");
            DropTable("dbo.PlanCourses");
            DropTable("dbo.Semesters");
            DropTable("dbo.Plans");
        }
    }
}
