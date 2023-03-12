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
            return string.Format("{1}{2}\t{0}: {3}{2}\t{4}: {5}{2}\t{6}: {7}{2}\t{8}: {9}{10}"
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