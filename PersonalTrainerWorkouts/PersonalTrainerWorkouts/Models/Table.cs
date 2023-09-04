using System;
using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    [Table("sqlite_master")]
    public class Table : BaseModel
    {
        [Column("name")]
        public new string Name { get; set; }

        public string Type     { get; set; }
        // ReSharper disable once InconsistentNaming
        public string Tbl_Name { get; set; }
        public int    RootPage { get; set; }
        public string Sql      { get; set; }

        public override string ToString()
        {
            return string.Format("{1}{2}\t{3}: {4}{2}\t{7}: {8}{2}\t{9}:{2}{10}{2}"
                               , nameof(Name)        //0;  Name
                               , Name                //1;  Ex: Workouts
                               , Environment.NewLine //2
                               , nameof(Type)        //3;  Type
                               , Type                //4;  Ex: table, index
                               , nameof(Tbl_Name)    //5;  Tbl_Name
                               , Tbl_Name            //6;  Ex: Workouts
                               , nameof(RootPage)    //7;  RootPage
                               , RootPage.ToString() //8;  Ex: 2
                               , nameof(Sql)         //9;  Sql
                               , Sql                 //10; SQL code to generate the table
            );
        }
    }
}
