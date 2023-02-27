using System;
using PersonalTrainerWorkouts.ViewModels;
using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    [Table("sqlite_master")]
    public class Table : BaseModel
    {
        [Column("name")]
        public new string Name { get; set; }

        public string Type     { get; set; }
        public string Tbl_Name { get; set; }
        public int    RootPage { get; set; }
        public string Sql      { get; set; }

        public override string ToString()
        {
            return string.Format("{1}{2}\t{3}: {4}{2}\t{5}: {6}{2}\t{7}: {8}{2}\t{9}: {10}{2}"
                               , nameof(Name)        //0
                               , Name                //1
                               , Environment.NewLine //2
                               , nameof(Type)        //3
                               , Type                //4
                               , nameof(Tbl_Name)    //5
                               , Tbl_Name            //6
                               , nameof(RootPage)    //7
                               , RootPage.ToString() //8
                               , nameof(Sql)         //9
                               , Sql                 //10
            );
        }
    }
}